using DemoApi.Domain.Entities;
using WebApplication1.Interfaces;
using AutoMapper;

namespace DemoApi.Service.Services
{
    public class PokemonPCService : IPokemonPCService
    {
        private readonly IPokemonPCRepository _pokemonPCRepository;
        private readonly IMapper _mapper;
        public PokemonPCService(IPokemonPCRepository pokemonPCRepository, IMapper mapper) 
        {
            _pokemonPCRepository = pokemonPCRepository;
            _mapper = mapper;
        }

        public async Task<Team> GetTeamAsync()
        {
            
            var pokemons = await _pokemonPCRepository.GetTeamAsync();
            var pokemonsInTeam = _mapper.Map<List<Pokemon>>(pokemons);
            var team = new Team() 
            {
                TeamSize = pokemons.Count,
                Pokemons = pokemonsInTeam
            };
            return team;
        }

        public async Task<List<Pokemon>> GetPokemonInPCAsync()
        {

            var pokemons = await _pokemonPCRepository.GetPokemonInPCAsync();
            var result = _mapper.Map<List<Pokemon>>(pokemons);
            return result;
        }

        public async Task<Pokemon> GetPokemonInPCByIDAsync(int id) 
        {
            var pokemon = await _pokemonPCRepository.GetPokemonInPCByIDAsync(id);
            var result = _mapper.Map<Pokemon>(pokemon);
            return result;
        }


    }
}
