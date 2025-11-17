using System;
using System.IO;
using System.Linq;
using DbUp;

internal static class Program
{
    private static int Main(string[] args)
    {
        // Usage examples:
        // dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db user --connection "Data Source=localhost,1433;Initial Catalog=DB_SYNC_USER;User ID=SA;Password=****;TrustServerCertificate=True"
        // dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db maintenance
        //
        // Environment variables fallback (recommended for secrets):
        //   DB_CONNECTION ............ full connection string
        //   DB_CHOICE ................ "user" | "maintenance" (default: "user")
        //
        // Scripts location (convention first, but overridable):
        //   Default: ./Backend/db-migrator/sql/{user|maintenance}
        //   Override: --scripts "absolute/or/relative/path"

        var dbChoice = GetArg(args, "--db") ?? (Environment.GetEnvironmentVariable("DB_CHOICE") ?? "user");
        if (!new[] { "user", "maintenance" }.Contains(dbChoice, StringComparer.OrdinalIgnoreCase))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid --db value '{dbChoice}'. Use 'user' or 'maintenance'.");
            Console.ResetColor();
            return -1;
        }

        // Connection string resolution
        var connection =
            GetArg(args, "--connection")
            ?? Environment.GetEnvironmentVariable("DB_CONNECTION")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"); // compatibility with existing envs

        if (string.IsNullOrWhiteSpace(connection))
        {
            // As a last resort, provide a sensible default for Docker network names
            // NOTE: Prefer setting DB_CONNECTION in your environment or docker-compose
            connection = dbChoice.Equals("user", StringComparison.OrdinalIgnoreCase)
                ? "Data Source=user-sqlserver,1433;Initial Catalog=DB_SYNC_USER;Persist Security Info=True;TrustServerCertificate=True;User ID=SA;Password=logPaSs#123$32"
                : "Data Source=user-sqlserver,1433;Initial Catalog=DB_SYNC_MAINTENANCE;Persist Security Info=True;TrustServerCertificate=True;User ID=SA;Password=logPaSs#123$32";
        }

        // Scripts path resolution
        var baseDir = AppContext.BaseDirectory;
        var scriptsArg = GetArg(args, "--scripts");
        var defaultScripts = Path.GetFullPath(Path.Combine(baseDir, "sql", dbChoice.ToLowerInvariant()));
        var scriptsPath = string.IsNullOrWhiteSpace(scriptsArg)
            ? defaultScripts
            : Path.GetFullPath(scriptsArg);

        Console.WriteLine("DbMigrator starting...");
        Console.WriteLine($"  Database:        {dbChoice}");
        Console.WriteLine($"  Scripts folder:  {scriptsPath}");
        Console.WriteLine($"  Connection:      {(connection?.Contains("Password=", StringComparison.OrdinalIgnoreCase) == true ? "******** (hidden)" : connection)}");

        if (!Directory.Exists(scriptsPath))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Scripts directory not found: {scriptsPath}");
            Console.WriteLine("Create the folder and add ordered *.sql files, e.g.:");
            Console.WriteLine($"  ./Backend/db-migrator/sql/{dbChoice}/001__create_tables.sql");
            Console.WriteLine($"  ./Backend/db-migrator/sql/{dbChoice}/002__seed_data.sql");
            Console.ResetColor();
            return -2;
        }

        var sqlFiles = Directory.EnumerateFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (sqlFiles.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"No *.sql files found in: {scriptsPath}");
            Console.ResetColor();
            return -3;
        }

        try
        {
            // Ensure database exists (creates DB if missing)
            EnsureDatabase.For.SqlDatabase(connection);

            // Ensure DB exists and run migrations
            var journalTable = $"DbUpJournal_{dbChoice.ToLowerInvariant()}";

            var upgrader = DeployChanges.To
                .SqlDatabase(connection)
                .WithVariablesDisabled()
                .WithTransactionPerScript()
                .JournalToSqlTable("dbo", journalTable)
                .WithScriptsFromFileSystem(scriptsPath)
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -10;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database migration succeeded.");
            Console.ResetColor();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Database migration failed:");
            Console.WriteLine(ex);
            Console.ResetColor();
            return -20;
        }
    }

    private static string? GetArg(string[] args, string name)
    {
        // Simple arg parser for "--name value" or "--name=value"
        var idx = Array.FindIndex(args, a => a.Equals(name, StringComparison.OrdinalIgnoreCase) || a.StartsWith(name + "=", StringComparison.OrdinalIgnoreCase));
        if (idx < 0) return null;
        var token = args[idx];

        if (token.Contains("="))
        {
            var parts = token.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length == 2 ? parts[1] : null;
        }

        if (idx + 1 < args.Length && !args[idx + 1].StartsWith("--"))
        {
            return args[idx + 1];
        }

        return null;
    }
}