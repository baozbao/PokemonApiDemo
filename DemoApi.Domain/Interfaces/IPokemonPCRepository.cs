using DemoApi.Domain.Entities;

namespace WebApplication1.Interfaces
{
    public interface IPokemonPCRepository
    {
        Task<List<Pokemon>> GetTeamAsync();

        Task<List<Pokemon>> GetPokemonInPCAsync();

        Task<Pokemon> GetPokemonInPCByIDAsync(int id);

        Task<Pokemon> CreatePokemonToTeamAsync(Pokemon pokemon);
    }
}
