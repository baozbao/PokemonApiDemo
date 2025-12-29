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

        public async Task<List<Pokemon>> GetTeamAsync()
        {
            var team = await _pokemonDbContext.Pokemons
                .Where(p => p.IsInTeam == true)
                .ToListAsync();

            return team;
        }

        public async Task<List<Pokemon>> GetPokemonInPCAsync()
        {
            var pokemons = await _pokemonDbContext.Pokemons
                .Where(p => p.IsInTeam != true)
                .ToListAsync();

            return pokemons;
        }

        public async Task<Pokemon> GetPokemonInPCByIDAsync(int id) 
        {
            var pokemon = await _pokemonDbContext.Pokemons
                .FirstOrDefaultAsync(p => p.Id == id && p.IsReleased == false);


            return pokemon;
        }

        public async Task<Pokemon> CreatePokemonToTeamAsync(Pokemon pokemon) 
        {
            // 1. Add to the DbSet (tracks the entity as 'Added')
            await _pokemonDbContext.Pokemons.AddAsync(pokemon);

            // 2. Save Changes (executes INSERT command)
            // EF Core AUTOMATICALLY populates 'pokemon.Id' here!
            await _pokemonDbContext.SaveChangesAsync();
            // 3. Return the updated entity
            // No need to query again; 'pokemon' now has the new ID.
            return pokemon;
        }

        public async Task<Pokemon> AddPokemonToPC(Pokemon pokemon) 
        {
            throw new NotImplementedException();    
        }


        public async Task<Pokemon> ReleasePokemon(int id)
        {
            var pokemon = await _pokemonDbContext.Pokemons
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pokemon == null) 
            {
                return null;
            }

            pokemon.IsReleased = true;

            await _pokemonDbContext.SaveChangesAsync();

            ///TODO: check if the pokemon's info is updated in the return
            return pokemon;
        }
    }
}
