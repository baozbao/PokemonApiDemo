using DemoApi.Service.Requests;
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

            //TODO: make a not found result
            var pokemon = await _pokemonPcService.GetPokemonInPCByIDAsync(id);

            return Ok(pokemon);
        }

        [HttpPost("team/pokemons")]
        // Trainer caught a pokemon in the WILD
        // DOTO: make seperate controllers
        // And later on, add pokemons should be checking from PC or from wild
        public async Task<IActionResult> CreatePokemonToTeamAsync([FromBody] CreatePokemonToTeamRequest request)
        {
            var result = await _pokemonPcService.CreatePokemonToTeamAsync(request);

            if (result.Success)
            {
                // Happy path: 200 OK with data
                return Ok(result.Data); // result.Data is the Pokemon
            }

            // "Check the value of result.ErrorCode"
            return result.ErrorCode switch
            {
                // CASE 1: exact match "TeamIsFull"
                // Return HTTP 409 (Conflict) -> Good for "state rules" like capacity limits
                "TeamIsFull" => Conflict(result.ErrorMessage),
                // CASE DEFAULT (_): anything else
                // Return HTTP 400 (Bad Request) -> Generic client error
                _ => BadRequest(result.ErrorMessage)
            };
        }

        [HttpPost("pc/pokemons")]
        // Trainer move pokemon to PC
        // TODO: think about move from team to pc

        public async Task<IActionResult> AddPokemonToPC([FromBody] CreatePokemonToTeamRequest request) 
        {
            // Check if this pokemon already has guid, if already has guid that means it was caught
            // else, it is a wild pokemon

            throw new NotImplementedException();
        
        }

    }
}
