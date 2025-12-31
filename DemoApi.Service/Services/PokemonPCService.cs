using AutoMapper;
using DemoApi.Domain.Entities;
using DemoApi.Service.Common;
using DemoApi.Service.Requests;
using System;
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

        public async Task<Pokemon> GetPokemonInPCByGuidAsync(Guid guid)
        {
            var pokemon = await _pokemonPCRepository.GetPokemonInPCByGuidAsync(guid);

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

        public async Task<Result<Team>> SwapPokemonFromTeamToPC(Guid pokemonInTeamGuid, Guid pokemonInPcGuid)
        {
            //Check Guid
            var pokemonInTeam = await _pokemonPCRepository.GetPokemonInPCByGuidAsync(pokemonInTeamGuid);
            var pokemonInPC = await _pokemonPCRepository.GetPokemonInPCByGuidAsync(pokemonInPcGuid);

            #region operation validation
            // ===========================
            // Validations
            // ===========================
            if (!pokemonInTeam.IsInTeam)
                return Result<Team>.Fail("InvalidOperation", $"Pokemon {pokemonInTeam.Name} is not in the team.");

            if (pokemonInPC.IsInTeam)
                return Result<Team>.Fail("InvalidOperation", $"Pokemon {pokemonInPC.Name} is already in the team.");

            if (pokemonInTeam == null)
                return Result<Team>.Fail("NotFound", $"Pokemon (Team) with ID {pokemonInTeamGuid} not found.");

            if (pokemonInPC == null)
                return Result<Team>.Fail("NotFound", $"Pokemon (PC) with ID {pokemonInPcGuid} not found.");

            #endregion

            // ===========================
            // 修改 (Modify) - 内存操作
            // ===========================
            pokemonInTeam.IsInTeam = false;
            pokemonInPC.IsInTeam = true;

            // 告诉 Repo：这俩变了 (还未提交)
            _pokemonPCRepository.Update(pokemonInTeam);
            _pokemonPCRepository.Update(pokemonInPC);
            // ===========================
            // 原子操作
            // ===========================
            // 只有这一行会连数据库。如果失败，上面两个 Update 都会作废。
            await _pokemonPCRepository.SaveChangesAsync();

            var newTeam = await GetTeamAsync();
            return Result<Team>.OK(newTeam);
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
                return Result<Pokemon>.Fail("InvalidOperation", "You cannot release a pokemon who is currently in team.");
            }
            var releasedPokemon = await _pokemonPCRepository.ReleasePokemon(id);
                
            return Result<Pokemon>.OK(releasedPokemon);
        }
    }
}
