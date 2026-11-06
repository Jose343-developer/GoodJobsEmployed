using Microsoft.EntityFrameworkCore;
using GoodJobsEmployed.API.Data;
using GoodJobsEmployed.API.BL;

// Cargar variables de entorno desde archivo .env si existe
var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFilePath))
{
    foreach (var line in File.ReadAllLines(envFilePath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

        var parts = trimmed.Split('=', 2);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cliente HTTP tipado/configurado con Timeout para llamadas a IA
builder.Services.AddHttpClient("GeminiClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Inyección del DbContext (DL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias (BL)
builder.Services.AddScoped<Perfil>();
builder.Services.AddScoped<CandidatoAnalisis>();

// Configuración de CORS para permitir peticiones desde Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS antes de la redirección y autorización
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapear los controladores
app.MapControllers();

app.Run();
