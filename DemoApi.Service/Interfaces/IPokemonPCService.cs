using DemoApi.Domain.Entities;
using DemoApi.Service.Common;
using DemoApi.Service.Requests;

public interface IPokemonPCService
{
    Task<Team> GetTeamAsync();

    Task<List<Pokemon>> GetPokemonInPCAsync();

    Task<Pokemon> GetPokemonInPCByIDAsync(int id);

    Task<Pokemon> GetPokemonInPCByGuidAsync(Guid guid);

    Task<Result<Pokemon>> CreatePokemonToTeamAsync(CreatePokemonToTeamRequest request);

    Task<Result<Team>> SwapPokemonFromTeamToPC(Guid pokemonInTeamGuid, Guid pokemonInPcGuid);

    Task<Result<Pokemon>> ReleasePokemon(int id);
}

