using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DemoApi.Domain.Entities;


namespace DemoApi.Service.Mapping
{
    public class MappingProfiles : Profile
    {
        // Mapping profiles 
        public MappingProfiles()
        {
            // Team → Pokemon
            CreateMap<PokemonTeam, Pokemon>()
                .ForMember(dest => dest.IsInTeam, opt => opt.MapFrom(src => true));

            // PC → Pokemon
            CreateMap<PokemonPC, Pokemon>()
                .ForMember(dest => dest.IsInTeam, opt => opt.MapFrom(src => false));
        }
    }
}
