using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;

namespace tenis_pro_back.Repositories
{
    public class ParameterRepository:IParameter
    {
        private readonly IMongoCollection<Parameter> _parameter;

        public ParameterRepository(IMongoDatabase database)
        {
            _parameter = database.GetCollection<Parameter>("Parameters");
        }

        public async Task<IEnumerable<Parameter>> GetAll()
        {
            return await _parameter.Find(u => true).ToListAsync();
        }

        public async Task<Parameter> GetById(string id)
        {
            return await _parameter.Find(u => u.Id == id).FirstOrDefaultAsync();
        }


        public async Task<Parameter> Post(Parameter user)
        {
            await _parameter.InsertOneAsync(user);
            return user;
        }

        public async Task Put(string id, Parameter param)
        {
            await _parameter.ReplaceOneAsync(u => u.Id == id, param);
        }

        public async Task Delete(string id)
        {
            await _parameter.DeleteOneAsync(u => u.Id == id);
        }
    }
}
