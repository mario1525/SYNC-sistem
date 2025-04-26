using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuraci�n de Ocelot
builder.Configuration.AddJsonFile("ocelot.jsonc", optional: false, reloadOnChange: true);

// Configurar servicios
builder.Services.AddOcelot();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // necesario si usas cookies o auth
    });
});

var app = builder.Build();

// [OPCIONAL] Forzar redirecci�n HTTPS solo si se est� usando HTTPS en el frontend
// app.UseHttpsRedirection();

// IMPORTANTE: Usar CORS ANTES de cualquier middleware
app.UseRouting(); // esto mejora manejo de OPTIONS con CORS
app.UseCors("AllowAngularApp");

// Usa Ocelot (esperar la tarea)
await app.UseOcelot();

app.Run();
