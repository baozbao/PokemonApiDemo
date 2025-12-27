using DemoApi.Domain.Entities;

namespace WebApplication1.Interfaces
{
    public interface IPokemonPCRepository
    {
        Task<List<PokemonTeam>> GetTeamAsync();

        Task<List<PokemonPC>> GetPokemonInPCAsync();

        Task<PokemonPC> GetPokemonInPCByIDAsync(int id);
    }
}
