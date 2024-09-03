using AutoMapper;
using Business;
using DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArtistSocialNetwork.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Role, RoleDTO>();
            CreateMap<Account, AccountDTO>();
        }
    }
}
