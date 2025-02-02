using Data;
using Entity;
using System.Text;
using Services;
using Data.SQLClient;
using Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


// Configuraci?n de las variables de entorno 
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

//DATA
builder.Services.AddSingleton<DaoUsers>();
builder.Services.AddSingleton<DaoUsuarioCredential>();

// ENTITY
builder.Services.AddSingleton<Users>(); 
builder.Services.AddSingleton<Mensaje>();
builder.Services.AddSingleton<UsuarioCredential>();
builder.Services.AddSingleton<Login>();
builder.Services.AddSingleton<Token>();
builder.Services.AddSingleton<Cuad>();
builder.Services.AddSingleton<Horario>();
builder.Services.AddSingleton<Turno>();



// SERVICES
builder.Services.AddSingleton<UsersLogical>();
builder.Services.AddSingleton<UsersCredentialLogical>();

//Middlewares
builder.Services.AddSingleton<HashPassword>();
builder.Services.AddSingleton<GenerateToken>(new GenerateToken(SecretKey, Issuer, Audience));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200 , http://localhost:3000")
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("MyPolicy");

// Middleware de autenticaci?n y autorizaci?n
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// swagger 
app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
