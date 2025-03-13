using MongoDB.Driver;

namespace tenis_pro_back.MongoDBService
{
	public class MongoDbService
	{
		private readonly IMongoDatabase _database;

		public MongoDbService(IMongoClient client)
		{
			_database = client.GetDatabase("ElClu"); // Cambia a tu nombre de base de datos
		}

		public async Task<bool> TestConnectionAsync()
		{
			try
			{
				var collectionNames = await _database.ListCollectionNames().ToListAsync();
				return collectionNames.Any(); // Retorna true si hay colecciones
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error al conectar a MongoDB: {ex.Message}");
				return false; // Retorna false si hay un error
			}
		}
	}
}
