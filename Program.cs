
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;
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


// Registro del repositorio
builder.Services.AddScoped(typeof(ICategory), typeof(CategoriesRepository));
builder.Services.AddScoped(typeof(ILocation), typeof(LocationsRepository));
builder.Services.AddScoped(typeof(ITournament), typeof(TournamentsRepository));
builder.Services.AddScoped(typeof(IUser), typeof(UsersRepository));
builder.Services.AddScoped(typeof(IProfile), typeof(ProfilesRepository));
builder.Services.AddScoped(typeof(IFunctionality), typeof(FunctionalitiesRepository));
builder.Services.AddScoped(typeof(IRegistration), typeof(RegistrationRepository));
builder.Services.AddScoped(typeof(IMatch), typeof(MatchesRepository));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Configuro CORS para que pueda acceder a traves del iis desde react native
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReactNativeApp", builder =>
	{
		builder
			.AllowAnyOrigin()   // Permite cualquier origen
			.AllowAnyMethod()   // Permite cualquier método (GET, POST, etc.)
			.AllowAnyHeader();  // Permite cualquier encabezado
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
