using DemoApi.Domain.Entities;

public interface IPokemonPCService
{
    Task<Team> GetTeamAsync();
    Task<List<Pokemon>> GetPokemonInPCAsync();

    Task<Pokemon> GetPokemonInPCByIDAsync(int id);
}

