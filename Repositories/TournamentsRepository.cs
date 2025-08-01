// TournamentRepository.cs
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using tenis_pro_back.Interfaces;
using tenis_pro_back.Models;
using tenis_pro_back.Models.Dto;
using tenis_pro_back.Models.Enums;

namespace tenis_pro_back.Repositories
{
	public class TournamentsRepository: ITournament
    {
        private readonly IMongoCollection<Tournament> _tournaments;
        private readonly ICategory  _categoriesRepository;
        private readonly ILocation _locationsRepository;
        private readonly IUser _usersRepository;

        public TournamentsRepository(IMongoDatabase database, ICategory categoriesRepository, ILocation locationsRepository, IUser usersRepository)
        {
            _tournaments = database.GetCollection<Tournament>("Tournaments");
            _categoriesRepository = categoriesRepository;
            _locationsRepository = locationsRepository;
            _usersRepository = usersRepository;
        }

        public async Task ResetTournamentDraw(string tournamentId)
        {
            var tournament = await GetById(tournamentId); // O agregar await si es async

            if (tournament == null)
                throw new ApplicationException("Tournament ID is required.");

            // Crear el filtro por ID
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, tournamentId);

            // Crear el update combinado
            var update = Builders<Tournament>.Update.Combine(
                Builders<Tournament>.Update.Set(t => t.Status, TournamentStatusEnum.Programming),
                Builders<Tournament>.Update.Set(t => t.Zones, new List<Zone>()) // suponiendo que Zones es una lista de Zone
            );

            var result = await _tournaments.UpdateOneAsync(filter, update);
        }

        public async Task<IEnumerable<TournamentDetailDto>> GetTournamentsWithOpenRegistrations()
        {
            //var today = DateTime.UtcNow.Date;

            //var openTournaments = await _tournaments
            //    .Find(t => t.CloseDate >= today && t.Status == Models.Enums.TournamentStatusEnum.Pending)
            //    .ToListAsync();

            
            var openTournaments = await _tournaments
                .Find(t => t.Status == Models.Enums.TournamentStatusEnum.Pending)
                .ToListAsync();

            return await GenerateDtoList(openTournaments);
        }

        public async Task<IEnumerable<TournamentDetailDto>> GetTournamentsToProgramming()
        {
            var today = DateTime.UtcNow;

            var tournaments = await _tournaments
                .Find(t => t.CloseDate < today && t.Status== Models.Enums.TournamentStatusEnum.Completed)
                .ToListAsync();

            return await GenerateDtoList(tournaments);
        }

        private async Task<List<TournamentDetailDto>> GenerateDtoList(List<Tournament> tournaments)
        {
            var result = new List<TournamentDetailDto>();

            List<Category> categories = await _categoriesRepository.GetAll();

            List<Location> locations = await _locationsRepository.GetAll();

            var categoriesMap = categories.ToDictionary(u => u.Id!, u => u.Description);
            var locationsMap = locations.ToDictionary(u => u.Id!, u => u.name);

            foreach (var tournament in tournaments)
            {
                result.Add(new TournamentDetailDto()
                {
                    Id = tournament.Id,
                    Description = tournament.Description,
                    CategoryId = tournament.CategoryId,
                    LocationId = tournament.LocationId,
                    CategoryDescription = categoriesMap.ContainsKey(tournament.CategoryId) ? categoriesMap[tournament.CategoryId] : "N/A",
                    LocationDescription = locationsMap.ContainsKey(tournament.LocationId) ? locationsMap[tournament.LocationId] : "N/A",
                    Status = tournament.Status,
                    StatusName = tournament.Status.ToString(),
                    CloseDate = tournament.CloseDate.HasValue ? DateTime.SpecifyKind(tournament.CloseDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    InitialDate = tournament.InitialDate.HasValue ? DateTime.SpecifyKind(tournament.InitialDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    EndDate = tournament.EndDate.HasValue ? DateTime.SpecifyKind(tournament.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                    TournamentType = tournament.TournamentType,
                    TournamentTypeDescription = tournament.TournamentType.ToString(),
                    Image = tournament.Image,
                    Participants = tournament.Participants,
                    PlayersPerZone = tournament.PlayersPerZone,
                    QualifiersPerZone = tournament.QualifiersPerZone,
                    IncludePlata = tournament.IncludePlata,
                    Zones = tournament.Zones,
                    MainBracket = tournament.MainBracket,
                    SilverCupBracket = tournament.SilverCupBracket
                });
            }

            return result;
        }

        private async Task<TournamentDetailDto> GenerateDtoElement(Tournament tournament)
        {
            List<Category> categories = await _categoriesRepository.GetAll();

            List<Location> locations = await _locationsRepository.GetAll();

            var categoriesMap = categories.ToDictionary(u => u.Id!, u => u.Description);
            var locationsMap = locations.ToDictionary(u => u.Id!, u => u.name);

            return new TournamentDetailDto()
            {
                Id = tournament.Id,
                Description = tournament.Description,
                CategoryId = tournament.CategoryId,
                LocationId = tournament.LocationId,
                CategoryDescription = categoriesMap.ContainsKey(tournament.CategoryId) ? categoriesMap[tournament.CategoryId] : "N/A",
                LocationDescription = locationsMap.ContainsKey(tournament.LocationId) ? locationsMap[tournament.LocationId] : "N/A",
                Status = tournament.Status,
                StatusName = tournament.Status.ToString(),
                CloseDate = tournament.CloseDate.HasValue ? DateTime.SpecifyKind(tournament.CloseDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                InitialDate = tournament.InitialDate.HasValue ? DateTime.SpecifyKind(tournament.InitialDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                EndDate = tournament.EndDate.HasValue ? DateTime.SpecifyKind(tournament.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                TournamentType = tournament.TournamentType,
                TournamentTypeDescription = tournament.TournamentType.ToString(),
                Image = tournament.Image,
                Participants = tournament.Participants,
                PlayersPerZone = tournament.PlayersPerZone,
                QualifiersPerZone = tournament.QualifiersPerZone,
                IncludePlata = tournament.IncludePlata,
                Zones = tournament.Zones,
                MainBracket = tournament.MainBracket,
                SilverCupBracket = tournament.SilverCupBracket
            };
        }

        public async Task<IEnumerable<TournamentDetailDto>> GetTournamentsActives()
        {
            var tournaments = await _tournaments
                .Find(t => t.Status != Models.Enums.TournamentStatusEnum.Pending || t.Status== Models.Enums.TournamentStatusEnum.Initiated)
                .ToListAsync();

            return await GenerateDtoList(tournaments);
        }

        public async Task<IEnumerable<TournamentBoardDto>> GetTournamentsBoard()
        {
            IEnumerable<TournamentBoardDto> tournaments = await _tournaments
                .Find(t => t.Status != Models.Enums.TournamentStatusEnum.Completed)
                .Project(t => new TournamentBoardDto
                {
                    Id = t.Id,
                    Image = t.Image
                })            
                .SortBy(s=>s.CloseDate)
                .ToListAsync();

            return tournaments;
        }


        public async Task<IEnumerable<TournamentDetailDto>> GetAll()
		{
			var tournaments = await _tournaments.Find(t => true).ToListAsync();

            return await GenerateDtoList(tournaments);
        }



        public async Task<TournamentDetailDto?> GetDtoById(string id)
		{
            var tournament = await _tournaments.Find(t => t.Id == id).FirstOrDefaultAsync();

            if (tournament == null) return null;

            return await GenerateDtoElement(tournament);
		}

		public async Task Post(Tournament tournament)
		{
			await _tournaments.InsertOneAsync(tournament);
		}

        public async Task UpdateDraw(TournamentUpdateDrawDto tournament, IClientSessionHandle session)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, tournament.Id);

            var update = Builders<Tournament>.Update
                .Set(t => t.Status, tournament.Status)
                .Set(t => t.PlayersPerZone, tournament.PlayersPerZone)
                .Set(t => t.QualifiersPerZone, tournament.QualifiersPerZone)
                .Set(t => t.IncludePlata, tournament.IncludePlata)
                .Set(t => t.Zones, tournament.Zones);

            var result = await _tournaments.UpdateOneAsync(session,filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Tournament not found.");
            }
        }

        public async Task UpdateMainBraket(TournamentUpdateBracketDto tournament, IClientSessionHandle session)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, tournament.Id);

            var update = Builders<Tournament>.Update
                .Set(t => t.Status, tournament.Status)
                .Set(t => t.MainBracket, tournament.DrawBracket);

            var result = await _tournaments.UpdateOneAsync(session, filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Tournament not found.");
            }
        }

        public async Task UpdateSilverCupBraket(TournamentUpdateBracketDto dto, IClientSessionHandle session)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, dto.Id);

            var update = Builders<Tournament>.Update
                .Set(t => t.Status, dto.Status)
                .Set(t => t.SilverCupBracket, dto.DrawBracket);

            var result = await _tournaments.UpdateOneAsync(session, filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception("Tournament not found.");
            }
        }
        public async Task UpdateTournament(string id, TournamentUpdateDto dto)
		{
            var update = Builders<Tournament>.Update;

            var updates = new List<UpdateDefinition<Tournament>>();

            if (dto.Description != null)
                updates.Add(update.Set(t => t.Description, dto.Description));

            if (dto.CloseDate != null)
                updates.Add(update.Set(t => t.CloseDate, dto.CloseDate));

            if (dto.InitialDate != null)
                updates.Add(update.Set(t => t.InitialDate, dto.InitialDate));

            if (dto.EndDate != null)
                updates.Add(update.Set(t => t.EndDate, dto.EndDate));

            if (dto.LocationId != null)
                updates.Add(update.Set(t => t.LocationId, dto.LocationId));

            if (dto.CategoryId != null)
                updates.Add(update.Set(t => t.CategoryId, dto.CategoryId));

            if (dto.TournamentType != null)
                updates.Add(update.Set(t => t.TournamentType, dto.TournamentType));

            if (dto.Image != null)
                updates.Add(update.Set(t => t.Image, dto.Image));

            if (updates.Count == 0)
                return; // Nada para actualizar

            var combined = update.Combine(updates);

            await _tournaments.UpdateOneAsync(t => t.Id == id, combined);
        }

        public async Task Delete(string id)
		{
			await _tournaments.DeleteOneAsync(t => t.Id == id);
		}

        public async Task<Tournament?> GetById(string id)
        {
            return  await _tournaments.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddParticipant(string id, Participant participant)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, id);
            var update = Builders<Tournament>.Update.Push(t => t.Participants, participant);

            await _tournaments.UpdateOneAsync(filter, update);
        }

        public async Task<List<User>> GetParticipantsToRegister(string categoryId, string tournamentId)
        {
            var tournament = await _tournaments.Find(t => t.Id == tournamentId).FirstOrDefaultAsync();
            if (tournament == null)
                throw new KeyNotFoundException("Torneo no encontrado");

            var allUsers = await _usersRepository.GetByCategory(categoryId);

            var registeredPlayerIds = tournament.Participants
                .SelectMany(p => p.PlayerIds)
                .Distinct()
                .ToHashSet();

            var availableUsers = allUsers
                .Where(u => !registeredPlayerIds.Contains(u.Id))
                .ToList();

            return availableUsers;
        }


        public async Task<bool> RemoveParticipant(string tournamentId, string participantId)
        {
            var update = Builders<Tournament>.Update.PullFilter(t => t.Participants, p => p.Id == participantId);

            var result = await _tournaments.UpdateOneAsync(
                t => t.Id == tournamentId,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<List<Participant>> GetParticipants(string tournamentId)
        {
            var tournament = await _tournaments.Find(t => t.Id == tournamentId).FirstOrDefaultAsync();
            return tournament?.Participants;
        }

        public async Task<Participant> GetParticipant(string tournamentId, string participantId)
        {
            var tournament = await _tournaments.Find(t => t.Id == tournamentId).FirstOrDefaultAsync();
            return tournament?.Participants.FirstOrDefault(p => p.Id == participantId);
        }

        public async Task UpdateStatus(string tournamentId, TournamentStatusEnum newStatus)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, tournamentId);
            var update = Builders<Tournament>.Update.Set(t => t.Status, newStatus);
            await _tournaments.UpdateOneAsync(filter, update);
        }

        public async Task UpdateStatus(string tournamentId, TournamentStatusEnum newStatus, IClientSessionHandle session)
        {
            var filter = Builders<Tournament>.Filter.Eq(t => t.Id, tournamentId);
            var update = Builders<Tournament>.Update.Set(t => t.Status, newStatus);
            await _tournaments.UpdateOneAsync(session, filter, update);
        }

        
    }
}
