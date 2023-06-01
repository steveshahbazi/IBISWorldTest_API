using AutoMapper;
using IBISWorld_API.Models;
using IBISWorld_API.Models.DTO;

namespace IBISWorld_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Term, TermDTO>();
            CreateMap<TermDTO, Term>();
        }
    }
}
