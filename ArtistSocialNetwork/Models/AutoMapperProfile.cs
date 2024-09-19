using AutoMapper;
using Business;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArtistSocialNetwork.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DocumentInfo, DocumentInfoDTO>();
            CreateMap<Account, AccountDTO>();
        }
    }
}
