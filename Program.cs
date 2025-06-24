using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Serilog;
using System.Text;
using tenis_pro_back.Helpers;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.MongoDBService;
using tenis_pro_back.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Log con Serilog
var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/log-.txt");
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddHttpContextAccessor();

// Configuración de MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");
    return client.GetDatabase(databaseName);
});

// Registro del repositorio
builder.Services.AddScoped(typeof(ICategory), typeof(CategoriesRepository));
builder.Services.AddScoped(typeof(ILocation), typeof(LocationsRepository));
builder.Services.AddScoped(typeof(ITournament), typeof(TournamentsRepository));
builder.Services.AddScoped(typeof(IUser), typeof(UsersRepository));
builder.Services.AddScoped(typeof(IProfile), typeof(ProfilesRepository));
builder.Services.AddScoped(typeof(IFunctionality), typeof(FunctionalitiesRepository));
builder.Services.AddScoped(typeof(IMatch), typeof(MatchesRepository));
builder.Services.AddScoped(typeof(IUserToken), typeof(UserTokenRepository));
builder.Services.AddScoped(typeof(IParameter), typeof(ParameterRepository));

// Services
builder.Services.AddScoped(typeof(ITournamentGeneratorService), typeof(tenis_pro_back.Services.TournamentGeneratorService));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSingleton<EncryptionHelper>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<EmailHelper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "https://tenis-pro-react.netlify.app") // Agrega ambos puertos
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Authorization") // Permite el encabezado Authorization
                  .AllowCredentials(); // Permite el envío de credenciales (cookies, cabeceras de autorización)
        });
});


// Configuración de autenticación con JWT
var encryptionHelper = new EncryptionHelper(builder.Configuration);

var encryptedKey = builder.Configuration["JwtSettings:Secret"];
var secretKey = encryptionHelper.Decrypt(encryptedKey); // Desencripta la clave antes de usarla


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Aquí puedes registrar el error si lo deseas
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Personaliza la respuesta cuando el token es inválido
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new { message = "Token inválido o expirado" });
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5213);
});

var app = builder.Build();

app.UseRouting();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
HandleErrorHelper.Initialize(loggerFactory);

app.UseCors("AllowFrontend");

// Configurar autenticación y autorización
app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization();  // Habilita la autorización

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });
}


app.MapControllers();
app.Run();