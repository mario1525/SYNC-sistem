using Data;
using Services;
using Sync.Common.Web;

// =====================================================================================
// SYNC maintenance-service: maestros de mantenimiento (compañías, ubicaciones, equipos,
// guías y actividades). Configuración por appsettings + variables de entorno:
//   ConnectionStrings__DefaultConnection, Jwt__SecretKey, Jwt__Issuer, Jwt__Audience, Cors__Origins
// =====================================================================================

var builder = WebApplication.CreateBuilder(args);
builder.AddSyncDefaults();

// Data
builder.Services.AddSingleton<DaoComp>();
builder.Services.AddSingleton<DaoEsp>();
builder.Services.AddSingleton<DaoPlanta>();
builder.Services.AddSingleton<DaoTipoArea>();
builder.Services.AddSingleton<DaoArea>();
builder.Services.AddSingleton<DaoSeccionArea>();
builder.Services.AddSingleton<DaoEquipo>();
builder.Services.AddSingleton<DaoGuia>();
builder.Services.AddSingleton<DaoProced>();
builder.Services.AddSingleton<DaoValid>();
builder.Services.AddSingleton<DaoGuiaEquipo>();
builder.Services.AddSingleton<DaoTipoActividad>();
builder.Services.AddSingleton<DaoActividad>();
builder.Services.AddSingleton<DaoActividadEquipo>();

// Lógica
builder.Services.AddSingleton<CompLogical>();
builder.Services.AddSingleton<EspLogical>();
builder.Services.AddSingleton<PlantaLogical>();
builder.Services.AddSingleton<TipoAreaLogical>();
builder.Services.AddSingleton<AreaLogical>();
builder.Services.AddSingleton<SeccionAreaLogical>();
builder.Services.AddSingleton<EquipoLogical>();
builder.Services.AddSingleton<GuiaLogical>();
builder.Services.AddSingleton<ProcedLogical>();
builder.Services.AddSingleton<ValidLogical>();
builder.Services.AddSingleton<GuiaEquipoLogical>();
builder.Services.AddSingleton<TipoActividadLogical>();
builder.Services.AddSingleton<ActividadLogical>();
builder.Services.AddSingleton<ActividadEquipoLogical>();

var app = builder.Build();
app.UseSyncDefaults();
app.Run();

public partial class Program;
