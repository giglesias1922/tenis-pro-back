using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Interfaces
{
    public interface IRegistration
    {
        Task<List<RegistrationsDto>> GetAll(string tournamenId );
        Task<Registration> GetById(string id);
        Task Post(Registration registration);
        Task Delete(string id);

        Task<List<User>> GetUsersToRegistrationAsync(string categoryId, string tournamentId);
        Task<List<User>> GetUsersRegistered(string tournamentId);

    }
}
