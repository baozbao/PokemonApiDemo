using DemoApi.Domain.Entities;
using DemoApi.Domain.Models;
using System.Threading.Tasks;

namespace DemoApi.Domain.Interfaces
{
    public interface IPokemonPCRepository
    {
        Task<List<Pokemon>> GetTeamAsync();

        Task<List<Pokemon>> GetPokemonInPCAsync();

        Task<Pokemon> GetPokemonInPCByIDAsync(int id);

        Task<Pokemon> GetPokemonInPCByGuidAsync(Guid guid);

        Task<Pokemon> CreatePokemonToTeamAsync(Pokemon pokemon);

        Task<Pokemon> ReleasePokemon(int id);

        // 纯内存操作，无需 Async
        void Update(Pokemon pokemon);

        // 真正的提交
        Task SaveChangesAsync();

        Task<PagedResult<Pokemon>> SearchAsync(PokemonSearchFilter filter);
    }
}
