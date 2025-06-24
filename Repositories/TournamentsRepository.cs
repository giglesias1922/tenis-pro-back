// TournamentRepository.cs
using tenis_pro_back.Models;
using MongoDB.Driver;
using tenis_pro_back.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using tenis_pro_back.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace tenis_pro_back.Repositories
{
	public class TournamentsRepository: ITournament
    {
        private readonly IMongoCollection<Tournament> _tournaments;
        private readonly ICategory  _categoriesRepository;
        private readonly ILocation _locationsRepository;

        public TournamentsRepository(IMongoDatabase database, ICategory categoriesRepository, ILocation locationsRepository)
        {
            _tournaments = database.GetCollection<Tournament>("Tournaments");
            _categoriesRepository = categoriesRepository;
            _locationsRepository = locationsRepository;
        }



        public async Task<IEnumerable<TournamentDetailDto>> GetTournamentsWithOpenRegistrations()
        {
            var today = DateTime.UtcNow.Date;

            var openTournaments = await _tournaments
                .Find(t => t.CloseDate >= today)
                .ToListAsync();

            return await GenerateDtoList(openTournaments);
        }

        public async Task<IEnumerable<TournamentDetailDto>> GetTournamentsToProgramming()
        {
            var today = DateTime.UtcNow;

            var tournaments = await _tournaments
                .Find(t => t.CloseDate < today && t.Status!= Models.Enums.TournamentStatusEnum.Finalized)
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
                    TournamentTypeDescription = tournament.TournamentType.ToString()
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
                .Find(t => t.Status != Models.Enums.TournamentStatusEnum.Finalized)
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



        public async Task<TournamentDetailDto?> GetById(string id)
		{
            var tournament = await _tournaments.Find(t => t.Id == id).FirstOrDefaultAsync();

            if (tournament == null) return null;

            return await GenerateDtoElement(tournament);
		}

		public async Task Post(Tournament tournament)
		{
			await _tournaments.InsertOneAsync(tournament);
		}

		public async Task Put(string id, Tournament tournament)
		{
			await _tournaments.ReplaceOneAsync(t => t.Id == id, tournament);
		}

		public async Task Delete(string id)
		{
			await _tournaments.DeleteOneAsync(t => t.Id == id);
		}
	}
}
