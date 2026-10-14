using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Sync.Common.Web;

// =====================================================================================
// SYNC api-gateway (Ocelot). Punto de entrada único: /apigateway/{recurso}/...
// Valida el JWT antes de reenviar (ADR-06); los servicios lo vuelven a validar.
// Configuración: Jwt__SecretKey, Jwt__Issuer, Jwt__Audience, Cors__Origins
// =====================================================================================

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var jwt = JwtOptions.From(builder.Configuration);
builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = jwt.ValidationParameters();
    });

var origins = (builder.Configuration["Cors:Origins"] ?? "http://localhost:4200")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(o => o.AddPolicy(ServiceDefaults.CorsPolicy, p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors(ServiceDefaults.CorsPolicy);
app.MapGet("/health", () => Results.Ok("OK"));
app.UseWhen(ctx => !ctx.Request.Path.StartsWithSegments("/health"), branch => branch.UseOcelot().Wait());

app.Run();
