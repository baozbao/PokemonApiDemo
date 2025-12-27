using DemoApi.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonPCController : ControllerBase
    {
        private readonly IPokemonPCService _pokemonPcService;

        public PokemonPCController(IPokemonPCService pokemonPcService)
        {
            _pokemonPcService = pokemonPcService;
        }

        [HttpGet("team")]
        //Get Current Team
        public async Task<IActionResult> GetTeamAsync()
        {
            var team = await _pokemonPcService.GetTeamAsync();

            return Ok(team);
        }

        [HttpGet("box")]
        // Get the pokemons in the Box
        public async Task<IActionResult> GetPokemonInPCAsync()
        {
            var pokemons = await _pokemonPcService.GetPokemonInPCAsync();

            return Ok(pokemons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPokemonInPCByIDAsync(int id)
        {
            var pokemon = await _pokemonPcService.GetPokemonInPCByIDAsync(id);

            return Ok(pokemon);
        }
    }
}
