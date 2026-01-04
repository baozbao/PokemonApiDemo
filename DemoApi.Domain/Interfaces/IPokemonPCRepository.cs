using DemoApi.Domain.Entities;

namespace DemoApi.Domain.Interfaces
{
    public interface IPokemonPCRepository
    {
        Task<List<Pokemon>> GetTeamAsync();

        Task<List<Pokemon>> GetPokemonInPCAsync();

        Task<Pokemon> GetPokemonInPCByIDAsync(int id);

        Task<Pokemon> GetPokemonInPCByGuidAsync(Guid guid);

        Task<Pokemon> CreatePokemonToTeamAsync(Pokemon pokemon);

        //Task<Pokemon> SwapPokemonFromTeamToPC(Guid pokemonInTeamGuid, Guid pokemonInPcGuid);

        Task<Pokemon> ReleasePokemon(int id);

        // 纯内存操作，无需 Async
        void Update(Pokemon pokemon);

        // 真正的提交
        Task SaveChangesAsync();
    }
}
