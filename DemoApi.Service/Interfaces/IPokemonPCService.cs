using DemoApi.Domain.Entities;
using DemoApi.Service.Common;
using DemoApi.Service.Requests;

public interface IPokemonPCService
{
    Task<Team> GetTeamAsync();
    Task<List<Pokemon>> GetPokemonInPCAsync();

    Task<Pokemon> GetPokemonInPCByIDAsync(int id);

    Task<Result<Pokemon>> CreatePokemonToTeamAsync(CreatePokemonToTeamRequest request);
}

