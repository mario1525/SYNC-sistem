using System.Threading.RateLimiting;
using Data;
using Services;
using Sync.Common.Web;

// =====================================================================================
// SYNC user-service: autenticación (JWT), usuarios, credenciales, cuadrillas, turnos y horarios.
// Configuración por appsettings + variables de entorno:
//   ConnectionStrings__DefaultConnection, Jwt__SecretKey, Jwt__Issuer, Jwt__Audience, Cors__Origins
// =====================================================================================

var builder = WebApplication.CreateBuilder(args);
builder.AddSyncDefaults();

// Límite de intentos de login por IP (mitiga fuerza bruta)
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

// Data
builder.Services.AddSingleton<DaoUsers>();
builder.Services.AddSingleton<DaoUsuarioCredential>();
builder.Services.AddSingleton<DaoCuad>();
builder.Services.AddSingleton<DaoTurno>();
builder.Services.AddSingleton<DaoHorario>();

// Lógica
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<UsersLogical>();
builder.Services.AddSingleton<CredentialLogical>();
builder.Services.AddSingleton<CuadLogical>();
builder.Services.AddSingleton<TurnoLogical>();
builder.Services.AddSingleton<HorarioLogical>();

var app = builder.Build();
app.UseSyncDefaults(a => a.UseRateLimiter());
app.Run();

public partial class Program;
