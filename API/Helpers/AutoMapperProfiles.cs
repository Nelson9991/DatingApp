using API.Models;
using API.Models.Dtos.User;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, UserReadDto>().ReverseMap();
        }
    }
}