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

        public async Task<PokemonTeam> CreatePokemonToTeamAsync(PokemonTeam pokemon) 
        {
            // 1. Add to the DbSet (tracks the entity as 'Added')
            await _pokemonDbContext.PokemonTeam.AddAsync(pokemon);

            // 2. Save Changes (executes INSERT command)
            // EF Core AUTOMATICALLY populates 'pokemon.Id' here!
            await _pokemonDbContext.SaveChangesAsync();
            // 3. Return the updated entity
            // No need to query again; 'pokemon' now has the new ID.
            return pokemon;
        }
    }
}
