using DemoApi.Domain.Entities;
using DemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interfaces;

namespace DemoApi.Infrastructure.Repositories
{
    public class PokemonPCRepository : IPokemonPCRepository
    {
        private readonly PokemonDbContext _pokemonDbContext;

        public PokemonPCRepository(PokemonDbContext pokemonDbContext)
        {
            _pokemonDbContext = pokemonDbContext;
        }

        public async Task<List<PokemonTeam>> GetTeamAsync()
        {
            var team = await _pokemonDbContext.PokemonTeam
                .Where(p => p.IsInTeam == true)
                .ToListAsync();

            return team;
        }

        public async Task<List<PokemonPC>> GetPokemonInPCAsync()
        {
            var pokemons = await _pokemonDbContext.PokemonPC
                .Where(p => p.IsInTeam != true)
                .ToListAsync();

            return pokemons;
        }

        public async Task<PokemonPC> GetPokemonInPCByIDAsync(int id) 
        {
            var pokemon = await _pokemonDbContext.PokemonPC
                .FirstOrDefaultAsync(p => p.Id == id);


            return pokemon;
        }
    }
}
