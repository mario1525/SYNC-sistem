using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.ScriptProviders;
using Microsoft.Data.SqlClient;

// =====================================================================================
// SYNC DbMigrator
//   1. sql/migrations/*.sql  -> se ejecutan UNA vez (registradas en dbo.SchemaVersions)
//   2. sql/procedures/*.sql  -> se ejecutan SIEMPRE (CREATE OR ALTER, idempotentes)
//   3. Seed opcional del usuario Root si la BD no tiene credenciales.
//
// Configuración (variables de entorno o argumentos):
//   DB_CONNECTION / --connection    cadena de conexión (obligatoria)
//   SQL_SCRIPTS   / --scripts       carpeta raíz de scripts (por defecto ./sql)
//   SEED_ROOT_USER, SEED_ROOT_PASSWORD  credenciales del Root inicial (opcional)
//   SEED_ROOT_COMP                       nombre de la compañía inicial (por defecto "SYNC")
// =====================================================================================

var connection = Arg("--connection") ?? Env("DB_CONNECTION") ?? Env("ConnectionStrings__DefaultConnection");
if (string.IsNullOrWhiteSpace(connection))
    return Fail("Falta la cadena de conexión. Use --connection o la variable DB_CONNECTION.", -1);

var scriptsRoot = Path.GetFullPath(Arg("--scripts") ?? Env("SQL_SCRIPTS") ?? Path.Combine(AppContext.BaseDirectory, "sql"));
var migrations = Path.Combine(scriptsRoot, "migrations");
var procedures = Path.Combine(scriptsRoot, "procedures");

Console.WriteLine("SYNC DbMigrator");
Console.WriteLine($"  Scripts: {scriptsRoot}");
if (!Directory.Exists(migrations) || !Directory.Exists(procedures))
    return Fail($"No se encontraron las carpetas 'migrations' y 'procedures' en {scriptsRoot}", -2);

try
{
    // SQL Server en contenedor puede tardar en aceptar conexiones: reintentos
    await WaitForServerAsync(connection);
    EnsureDatabase.For.SqlDatabase(connection);

    var once = DeployChanges.To
        .SqlDatabase(connection)
        .WithScriptsFromFileSystem(migrations, new FileSystemScriptOptions { IncludeSubDirectories = false })
        .WithTransactionPerScript()
        .WithVariablesDisabled()
        .JournalToSqlTable("dbo", "SchemaVersions")
        .LogToConsole()
        .Build();
    if (!Check(once.PerformUpgrade()))
        return -10;

    var always = DeployChanges.To
        .SqlDatabase(connection)
        .WithScriptsFromFileSystem(procedures, new FileSystemScriptOptions { IncludeSubDirectories = false })
        .WithTransactionPerScript()
        .WithVariablesDisabled()
        .JournalTo(new NullJournal())
        .LogToConsole()
        .Build();
    if (!Check(always.PerformUpgrade()))
        return -11;

    await SeedRootAsync(connection);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Migración completada.");
    Console.ResetColor();
    return 0;
}
catch (Exception ex)
{
    return Fail("La migración falló: " + ex, -20);
}

// ------------------------------------------------------------------------------------

static async Task SeedRootAsync(string connectionString)
{
    var user = Env("SEED_ROOT_USER");
    var password = Env("SEED_ROOT_PASSWORD");
    if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("Seed Root omitido (SEED_ROOT_USER / SEED_ROOT_PASSWORD no definidos).");
        return;
    }

    await using var cn = new SqlConnection(connectionString);
    await cn.OpenAsync();

    await using (var check = new SqlCommand("SELECT COUNT(*) FROM dbo.User_Cred WHERE Eliminado = 0", cn))
    {
        if ((int)(await check.ExecuteScalarAsync() ?? 0) > 0)
        {
            Console.WriteLine("Seed Root omitido: ya existen credenciales.");
            return;
        }
    }

    const string sql = """
        INSERT INTO dbo.Comp (Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Eliminado)
        VALUES (@CompId, @CompNombre, '', '', '', 'Administración', 1, 0);
        INSERT INTO dbo.Users (Id, Nombre, Apellido, Identificacion, Correo, IdComp, Cargo, Rol, Estado, Eliminado)
        VALUES (@UserId, 'Administrador', 'SYNC', 0, '', @CompId, 'Administrador del sistema', 'Root', 1, 0);
        INSERT INTO dbo.User_Cred (Id, Usuario, Contrasenia, IdUser, Estado, Eliminado)
        VALUES (CONVERT(VARCHAR(36), NEWID()), @Usuario, @Hash, @UserId, 1, 0);
        """;
    await using var tx = (SqlTransaction)await cn.BeginTransactionAsync();
    await using var cmd = new SqlCommand(sql, cn, tx);
    cmd.Parameters.AddWithValue("@CompId", Guid.NewGuid().ToString());
    cmd.Parameters.AddWithValue("@CompNombre", Env("SEED_ROOT_COMP") ?? "SYNC");
    cmd.Parameters.AddWithValue("@UserId", Guid.NewGuid().ToString());
    cmd.Parameters.AddWithValue("@Usuario", user);
    cmd.Parameters.AddWithValue("@Hash", BCrypt.Net.BCrypt.HashPassword(password));
    await cmd.ExecuteNonQueryAsync();
    await tx.CommitAsync();
    Console.WriteLine($"Seed Root creado: usuario '{user}'.");
}

static async Task WaitForServerAsync(string connectionString)
{
    var master = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master", ConnectTimeout = 5 }.ConnectionString;
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await using var cn = new SqlConnection(master);
            await cn.OpenAsync();
            return;
        }
        catch (SqlException) when (attempt < 30)
        {
            Console.WriteLine($"SQL Server no disponible aún (intento {attempt}/30)...");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

static bool Check(DatabaseUpgradeResult result)
{
    if (result.Successful) return true;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error en script '{result.ErrorScript?.Name}': {result.Error}");
    Console.ResetColor();
    return false;
}

static int Fail(string message, int code)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    return code;
}

static string? Env(string name) => Environment.GetEnvironmentVariable(name) is { Length: > 0 } v ? v : null;

string? Arg(string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}
