
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using tenis_pro_back.MongoDBService;
using tenis_pro_back.Repositories;

var builder = WebApplication.CreateBuilder(args);



// Configuración de MongoDB
builder.Services.Configure<MongoDbSettings>(
	builder.Configuration.GetSection("MongoDbSettings"));

// Registro de IMongoClient para la conexión a MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
	var connectionString = builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString");
	return new MongoClient(connectionString);
});

// Registro de la base de datos de MongoDB
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
	var client = sp.GetRequiredService<IMongoClient>();
	var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");
	return client.GetDatabase(databaseName);
});


// Registro del repositorio CategoryRepository
builder.Services.AddScoped<CategoriesRepository>();
builder.Services.AddScoped<LocationsRepository>();
builder.Services.AddScoped<TournamentsRepository>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<ProfilesRepository>();
builder.Services.AddScoped<FunctionalitiesRepository>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Configuro CORS para que pueda acceder a traves del iis desde react native
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactNativeApp", builder =>
	{
		builder
			.WithOrigins(
				"http://localhost:3000", // React.js local
				"http://localhost:5174", // Vite (React.js con Vite)
				"http://elclu.back", // React Native Expo en red local
				"http://10.0.2.2:19000" // Emulador Android con Expo
			)
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5213); // Escuchar en el puerto 5213 para HTTP
});

var app = builder.Build();



app.UseCors("AllowReactNativeApp");


var serviceProvider = builder.Services.BuildServiceProvider();

var objUser = serviceProvider.GetRequiredService<UsersRepository>();




//// Validar conexión a MongoDB después de construir la aplicación
//using (var scope = app.Services.CreateScope())
//{
//	var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
//	var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");
//	var database = mongoClient.GetDatabase(databaseName);
//	var command = new BsonDocument { { "ping", 1 } };

//	try
//	{
//		await database.RunCommandAsync<BsonDocument>(command);
//		Console.WriteLine("Conexión a MongoDB exitosa.");
//	}
//	catch (Exception ex)
//	{
//		Console.WriteLine($"Error de conexión a MongoDB: {ex.Message}");
//	}
//}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
		c.RoutePrefix = "swagger"; // Esto define que la ruta sea "/swagger"
	});
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
