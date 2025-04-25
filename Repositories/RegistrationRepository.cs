using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;

namespace tenis_pro_back.Repositories
{
    public class RegistrationRepository:IRegistration
    {
        private readonly IMongoCollection<Registration> _registration;
        private readonly IUser _userRepository;
        private readonly ITournament _tournamentRepository;

        public RegistrationRepository(IMongoDatabase database, IUser userRepository, ITournament tournament)
        {
            _registration = database.GetCollection<Registration>("registrations");
            _userRepository = userRepository;
            _tournamentRepository = tournament;
        }

        public async Task<List<User>> GetUsersToRegistrationAsync(string categoryId, string tournamentId)
        {
            // Obtener todos los usuarios de la categoría
            IEnumerable<User> listUsers = await _userRepository.GetByCategory(categoryId);

            // Obtener todas las inscripciones del torneo
            var registrations = await _registration
                .Find(r => r.TournamentId == tournamentId)
                .ToListAsync();

            // Obtener todos los IDs de jugadores ya inscritos
            var registeredUserIds = registrations
                .SelectMany(r => r.Players) // aplanar todos los arrays de jugadores
                .ToHashSet();

            // Devolver usuarios que no están registrados
            var unregisteredUsers = listUsers
                .Where(u => !registeredUserIds.Contains(u.Id))
                .ToList();

            return unregisteredUsers;
        }


        public async Task<List<RegistrationsDto>> GetAll(string tournamentId)
        {
            var response = new List<RegistrationsDto>();

            var data = await _registration.Find(c => c.TournamentId == tournamentId).ToListAsync();
            var oTournament = await _tournamentRepository.GetById(tournamentId);

            var allUserIds = data
                .SelectMany(r => r.Players.Append(r.CreatedBy))
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var listUsers = await _userRepository.GetAll();

            var users = listUsers.Where(u => allUserIds.Contains(u.Id)).ToList();
            var userMap = users.ToDictionary(u => u.Id, u => $"{u.Name ?? ""} {u.LastName ?? ""}".Trim());

            foreach (var obj in data)
            {
                // Combinar los nombres de los jugadores
                var userNames = obj.Players
                    .Select(pid => userMap.GetValueOrDefault(pid, "Desconocido"))
                    .ToList();

                string combinedUserName = string.Join(" / ", userNames); // jugador1 / jugador2

                response.Add(new RegistrationsDto()
                {
                    Id = obj.Id,
                    TournamentId = obj.TournamentId,
                    UserId = obj.Players.FirstOrDefault(),
                    CreatedAt = obj.CreatedAt,
                    CreatedBy = obj.CreatedBy,
                    TournamentDescription = oTournament?.Description,
                    CreatedByName = obj.CreatedBy != null ? userMap.GetValueOrDefault(obj.CreatedBy, "Desconocido") : "",
                    UserName = combinedUserName
                });
            }

            return response;
        }


        public async Task<Registration> GetById(string id)
        {
            return await _registration.Find(c => c.Id == id).FirstOrDefaultAsync();
        }


        public async Task Post(Registration registration)
        {
            await _registration.InsertOneAsync(registration);
        }

        public async Task Delete(string id)
        {
            await _registration.DeleteOneAsync(c => c.Id == id);
        }
    }
}