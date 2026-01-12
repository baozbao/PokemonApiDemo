using DemoApi.Domain.Entities;
using DemoApi.Service.Common;
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

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetPokemonInPCByIDAsync(int id)
        {

            var pokemon = await _pokemonPcService.GetPokemonInPCByIDAsync(id);
            if (pokemon == null)
            {
                return NotFound($"Pokemon with ID {id} not found.");
            }
            return Ok(pokemon);
        }

        [HttpGet("guid/{guid}")]
        public async Task<IActionResult> GetPokemonInPCByGuidAsync(Guid guid)
        {

            var pokemon = await _pokemonPcService.GetPokemonInPCByGuidAsync(guid);
            if (pokemon == null)
            {
                return NotFound($"Pokemon with GUID {guid} not found.");
            }
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

            return result.ErrorCode switch
            {

                "TeamIsFull" => Conflict(result.ErrorMessage),

                _ => BadRequest(result.ErrorMessage)
            };
        }

        [HttpPost("teampc/swap")]
        // Trainer Swap pokemon from team to pc
        public async Task<IActionResult> SwapPokemonFromTeamToPC(Guid pokemonInTeamGuid, Guid pokemonInPcId) 
        {

            var result = await _pokemonPcService.SwapPokemonFromTeamToPC(pokemonInTeamGuid, pokemonInPcId);

            if (result.Success)
            {
                return Ok(result.Data);
            }

            return result.ErrorCode switch
            {
                "NotFound" => NotFound(result.ErrorMessage),    // 404

                "InvalidOperation" => Conflict(result.ErrorMessage),// 409 (状态冲突)
                _ => BadRequest(result.ErrorMessage)            // 400
            };

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ReleasePokemon(int id) 
        {
            var result = await _pokemonPcService.ReleasePokemon(id);

            if (result.Success)
            {
                // 这里的 result.Data 就是刚刚在 Service 里返回的那个被软删除的 Pokemon
                return Ok(result.Data);
            }
            // 失败
            return result.ErrorCode switch
            {
                // 如果是 NotFound，返回 404
                "NotFound" => NotFound(result.ErrorMessage),

                // 其他错误 (比如数据库挂了)，返回 400
                _ => BadRequest(result.ErrorMessage)
            };


        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPokemonAsync([FromQuery] SearchPokemonRequest request)
        {
            var result = await _pokemonPcService.SearchPokemonAsync(request);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
