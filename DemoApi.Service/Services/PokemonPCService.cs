using AutoMapper;
using DemoApi.Domain.Entities;
using DemoApi.Service.Common;
using DemoApi.Service.Requests;
using WebApplication1.Interfaces;

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

            // Map result from PokemonTeam to Pokemon
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
            // Map result from PokemonPC to Pokemon
            var result = _mapper.Map<List<Pokemon>>(pokemons);
            return result;
        }

        public async Task<Pokemon> GetPokemonInPCByIDAsync(int id)
        {
            var pokemon = await _pokemonPCRepository.GetPokemonInPCByIDAsync(id);

            return pokemon;
        }

        public async Task<Result<Pokemon>> CreatePokemonToTeamAsync(CreatePokemonToTeamRequest request)
        {
            // TODO: 
            // Validation Check
            var team = await GetTeamAsync();

            if (team.TeamSize >= 6)
            {
                return Result<Pokemon>.Fail("TeamIsFull", "Team is already full.");
            }
            // Success path

            // Convert request to entity
            var pokemonEntity = _mapper.Map<Pokemon>(request);

            if (pokemonEntity.GuidId == Guid.Empty)
            {
                pokemonEntity.GuidId = Guid.NewGuid();
            }
            pokemonEntity.IsInTeam = true; // We are adding to "Team" specifically
            var savedEntity = await _pokemonPCRepository.CreatePokemonToTeamAsync(pokemonEntity);

            return Result<Pokemon>.OK(savedEntity);

        }

        public async Task<Result<Pokemon>> AddPokemonToPC(CreatePokemonToTeamRequest request) 
        {
            //Check Guid

            throw new NotImplementedException();
        }

        public async Task<Result<Pokemon>> ReleasePokemon(int id) 
        {
            var pokemon = await GetPokemonInPCByIDAsync(id);

            if (pokemon == null) 
            {
                return Result<Pokemon>.Fail("NotFound", $"Pokemon with ID {id} not found.");
            }
            if (pokemon.IsInTeam) 
            {
                return Result<Pokemon>.Fail("InvalidAction", "You cannot release a pokemon who is currently in team.");
            }
            var releasedPokemon = await _pokemonPCRepository.ReleasePokemon(id);
                
            return Result<Pokemon>.OK(releasedPokemon);
        }
    }
}
