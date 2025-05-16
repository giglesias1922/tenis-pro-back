
using AutoMapper;

namespace tenis_pro_back.Models.Dto
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Registration, RegistrationUserDto>();
            CreateMap<Match.MatchResult, MatchResultDto>();
        }
    }
}
