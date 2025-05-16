using AutoMapper;
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
        private readonly IMapper _mapper;

        public RegistrationRepository(IMongoDatabase database, IUser userRepository, ITournament tournament, IMapper mapper)
        {
            _registration = database.GetCollection<Registration>("registrations");
            _userRepository = userRepository;
            _tournamentRepository = tournament;
            _mapper = mapper;
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
            .SelectMany(r => r.Players)
            .ToHashSet();

            // Devolver usuarios que no están registrados
            var unregisteredUsers = listUsers
                .Where(u => !registeredUserIds.Contains(u.Id))
                .ToList();

            return unregisteredUsers;
        }

        public async Task<List<RegistrationUserDto>> GetRegistratedUsers(string tournamentId)
        {
            var registrations = await _registration
                .Find(r => r.TournamentId == tournamentId)
                .SortBy(r => r.DisplayName)
                .ToListAsync();

            var registrationDtos = _mapper.Map<List<RegistrationUserDto>>(registrations);

            return registrationDtos;
        }

        public async Task<List<RegistrationsDto>> GetAll(string tournamentId)
        {
            var response = new List<RegistrationsDto>();

            var data = await _registration.Find(c => c.TournamentId == tournamentId).ToListAsync();
            var oTournament = await _tournamentRepository.GetById(tournamentId);


            var listUsers = await _userRepository.GetAll();

            var userMap = listUsers.ToDictionary(u => u.Id, u => $"{u.Name ?? ""} {u.LastName ?? ""}".Trim());

            foreach (var obj in data)
            {
                response.Add(new RegistrationsDto()
                {
                    Id = obj.Id,
                    TournamentId = obj.TournamentId,
                    DisplayName = obj.DisplayName,
                    CreatedAt = obj.CreatedAt,
                    CreatedBy = obj.CreatedBy,
                    TournamentDescription = oTournament?.Description,
                    CreatedByName = obj.CreatedBy != null ? userMap.GetValueOrDefault(obj.CreatedBy, "Desconocido") : ""
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