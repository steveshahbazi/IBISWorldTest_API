using AutoMapper;
using IBISWorldTest.Models;
using IBISWorldTest.Models.DTO;

namespace IBISWorldTest
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
