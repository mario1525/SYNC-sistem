using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sync.Common.Data;

namespace Sync.Common.Web
{
    /// <summary>Opciones JWT compartidas por emisor (user-service) y validadores.</summary>
    public sealed class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationHours { get; set; } = 8;

        public static JwtOptions From(IConfiguration configuration)
        {
            var options = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
            if (Encoding.UTF8.GetByteCount(options.SecretKey) < 32)
                throw new InvalidOperationException("Jwt:SecretKey no está configurada o tiene menos de 32 bytes. Defínala con la variable de entorno Jwt__SecretKey.");
            if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
                throw new InvalidOperationException("Jwt:Issuer y Jwt:Audience son obligatorios.");
            return options;
        }

        public TokenValidationParameters ValidationParameters() => new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }

    public static class ServiceDefaults
    {
        public const string CorsPolicy = "SyncFrontend";

        /// <summary>
        /// Configuración base de un microservicio SYNC. La configuración se toma de
        /// appsettings.json + variables de entorno (p. ej. ConnectionStrings__DefaultConnection, Jwt__SecretKey).
        /// </summary>
        public static WebApplicationBuilder AddSyncDefaults(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            // Logging estructurado (JSON) fuera de desarrollo
            if (!builder.Environment.IsDevelopment())
            {
                builder.Logging.ClearProviders();
                builder.Logging.AddJsonConsole(o => o.IncludeScopes = true);
            }

            // Acceso a datos
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            builder.Services.AddSingleton(new SqlClient(connectionString));

            // Errores → ProblemDetails
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ApiExceptionHandler>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // CORS (orígenes separados por coma en Cors:Origins)
            var origins = (configuration["Cors:Origins"] ?? "http://localhost:4200")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

            // JWT
            var jwt = JwtOptions.From(configuration);
            builder.Services.AddSingleton(jwt);
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = jwt.ValidationParameters();
                });
            builder.Services.AddAuthorization();

            // Health checks: /health (vivo) y /health/ready (con base de datos)
            builder.Services.AddHealthChecks().AddCheck<SqlHealthCheck>("sql", tags: ["ready"]);

            return builder;
        }

        /// <param name="despuesDeRouting">Middleware adicional que debe ir tras UseRouting (p. ej. UseRateLimiter).</param>
        public static WebApplication UseSyncDefaults(this WebApplication app, Action<WebApplication>? despuesDeRouting = null)
        {
            app.UseExceptionHandler();
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseRouting();
            despuesDeRouting?.Invoke(app);
            app.UseCors(CorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
            app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains("ready") }).AllowAnonymous();
            return app;
        }
    }

    internal sealed class SqlHealthCheck(SqlClient sql) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
        {
            try
            {
                return await sql.PingAsync(ct) ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("SELECT 1 falló");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Sin conexión a SQL Server", ex);
            }
        }
    }
}
