using DemoApi.Domain.Entities;
using DemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DemoApi.Domain.Interfaces;
using DemoApi.Domain.Models;

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

        public async Task<Pokemon> GetPokemonInPCByGuidAsync(Guid guid)
        {
            var pokemon = await _pokemonDbContext.Pokemons
                .FirstOrDefaultAsync(p => p.GuidId == guid && p.IsReleased == false);

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


        // 只是告诉 EF Core：这个对象变了，等会儿记得存。
        public void Update(Pokemon pokemon)
        {
            _pokemonDbContext.Pokemons.Update(pokemon);
        }

        // 手动 “原子性操作”， 这样的话我们可以update 几十个然后再savechange去保证原子性
        public async Task SaveChangesAsync()
        {
            await _pokemonDbContext.SaveChangesAsync();
        }

        public async Task<PagedResult<Pokemon>> SearchPokemonAsync(PokemonSearchFilter filter)
        {
            // Prepare the query as IQueryable
            var query = _pokemonDbContext.Pokemons.AsQueryable();

            // 1. Apply Filtering
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(p => p.Name.Contains(filter.Name));
            }
            if (filter.MinLevel.HasValue)
            {
                query = query.Where(p => p.Level >= filter.MinLevel.Value);
            }
            if (filter.MaxLevel.HasValue)
            {
                query = query.Where(p => p.Level <= filter.MaxLevel.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(p => p.Type.Contains(filter.Type));
            }
            if (!string.IsNullOrWhiteSpace(filter.Specie))
            {
                query = query.Where(p => p.Specie.Contains(filter.Specie));
            }
            if (!string.IsNullOrWhiteSpace(filter.Gender))
            {
                query = query.Where(p => p.Gender == filter.Gender);
            }

            // 2. Count Total (before paging)
            var totalCount = await query.CountAsync();

            // 3. Apply Sorting (Default by ID for stability)
            query = query.OrderBy(p => p.Id);

            // 4. Apply Pagination (Skip & Take)
            // Ensure Page is at least 1 to avoid negative skip
            var pageIndex = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 5. Return Paged Result
            return new PagedResult<Pokemon>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
