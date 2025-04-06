using Data;
using Data.SQLClient;
using Entity;
using Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;

// Configuraci�n de las variables de entorno 
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

string connectionString = configuration["Configuracion:connectionString"];
string SecretKey = configuration["Jwt:SecretKey"];
string Issuer = configuration["Jwt:Issuer"];
string Audience = configuration["Jwt:Audience"];

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SqlClient>(new SqlClient(connectionString));

// DATA 
builder.Services.AddSingleton<DaoComp>();
builder.Services.AddSingleton<DaoActividad>();
builder.Services.AddSingleton<DaoActividadEquipo>();
builder.Services.AddSingleton<DaoEquipo>();
builder.Services.AddSingleton<DaoEsp>();
builder.Services.AddSingleton<DaoGuia>();
builder.Services.AddSingleton<DaoGuiaEquipo>();
builder.Services.AddSingleton<DaoProced>();
builder.Services.AddSingleton<DaoTipoActividad>();
builder.Services.AddSingleton<DaoValid>();
builder.Services.AddSingleton<DaoAreaFuncional>();
builder.Services.AddSingleton<DaoBodega>();
builder.Services.AddSingleton<DaoPatio>();
builder.Services.AddSingleton<DaoPlanta>();
builder.Services.AddSingleton<DaoSeccionBodega>();
builder.Services.AddSingleton<DaoUbicacionEquipo>();



// ENTITY
builder.Services.AddSingleton<Comp>();
builder.Services.AddSingleton<Actividad>();
builder.Services.AddSingleton<ActividadEquipo>();
builder.Services.AddSingleton<Equipo>();
builder.Services.AddSingleton<Esp>();
builder.Services.AddSingleton<Guia>();
builder.Services.AddSingleton<Guia_Equipo>();
builder.Services.AddSingleton<Mensaje>();
builder.Services.AddSingleton<Proced>();
builder.Services.AddSingleton<TipoAct>();
builder.Services.AddSingleton<Valid>();
builder.Services.AddSingleton<AreaFuncional>();
builder.Services.AddSingleton<Bodega>();
builder.Services.AddSingleton<Patio>();
builder.Services.AddSingleton<Planta>();
builder.Services.AddSingleton<SeccionBodega>();
builder.Services.AddSingleton<UbicacionEquipo>();


//Logical
builder.Services.AddSingleton<CompLogical>();
builder.Services.AddSingleton<ActividadLogical>();
builder.Services.AddSingleton<ActividadEquipoLogical>();
builder.Services.AddSingleton<EquipoLogical>();
builder.Services.AddSingleton<EspLogical>();
builder.Services.AddSingleton<GuiaLogical>();
builder.Services.AddSingleton<GuiaEquipoLogical>();
builder.Services.AddSingleton<ProcedLogical>();
builder.Services.AddSingleton<TipoActividadLogical>();
builder.Services.AddSingleton<ValidLogical>();
builder.Services.AddSingleton<AreaFuncionalLogical>();
builder.Services.AddSingleton<BodegaLogical>();
builder.Services.AddSingleton<PatioLogical>();
builder.Services.AddSingleton<PlantaLogical>();
builder.Services.AddSingleton<SeccionBodegaLogical>();
builder.Services.AddSingleton<UbicacionEquipoLogical>();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
        });
});

// swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci?n de la autenticaci?n JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();               // 1️⃣ Primero enrutar
app.UseCors("MyPolicy");        // 2️⃣ Luego aplicar CORS justo después de routing
app.UseAuthentication();        // 3️⃣ Luego autenticación
app.UseAuthorization();         // 4️⃣ Finalmente autorización

app.MapRazorPages();

// swagger 
app.UseSwagger();
app.UseSwaggerUI();



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
