using tenis_pro_back.Models;

namespace tenis_pro_back.Interfaces
{
    public interface ICategory
    {
        Task<List<Category>> GetAll();
        Task<Category> GetById(string id);
        Task<List<Category>> GetEnabledCategories();
        Task Post(Category category);
        Task Put(string id, Category category);

        Task Delete(string id);


    }
}
