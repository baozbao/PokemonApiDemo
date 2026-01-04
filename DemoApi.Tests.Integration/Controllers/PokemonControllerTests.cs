using DemoApi.Domain.Entities;
using DemoApi.Service.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DemoApi.Tests.Integration.Controllers
{
    // ===============================================
    // 集成测试类: PokemonPCControllerTests
    // ===============================================
    // 继承 IClassFixture<CustomWebApplicationFactory> 表示：
    // 这个测试类里的所有测试方法，都共用同一个 "测试服务器工厂"。
    public class PokemonControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public PokemonControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            // 创建一个模拟的 HTTP 客户端，用来发请求 (就像 Postman)
            _client = factory.CreateClient();
        }

        // ===============================================
        // 测试 1: 验证能否成功抓获一只新宝可梦
        // ===============================================
        [Fact (DisplayName = "Should_Add_Pokemon_To_Team_When_Slot_Available")]
        public async Task CreatePokemonToTeam_Should_Add_Pokemon_To_Database()
        {
            // 1. Arrange (准备数据)
            var request = new CreatePokemonToTeamRequest
            {
                Name = "Gengar",
                Gender = "Male",
                Level = 50,
                Type = "Ghost/Poison"
            };

            // 2. Act (执行 - 发 POST 请求)
            // 路由要和 Controller 里的一模一样
            var response = await _client.PostAsJsonAsync("/api/PokemonPC/team/pokemons", request);

            // 3. Assert (验证 - 检查结果)
            
            // 3.1 验证 API 返回 200 OK
            response.EnsureSuccessStatusCode(); 
            
            // 3.2 验证返回的 JSON 数据
            var returnedPokemon = await response.Content.ReadFromJsonAsync<Pokemon>();
            Assert.NotNull(returnedPokemon);
            Assert.Equal("Gengar", returnedPokemon.Name);

            // 3.3 (进阶) 直接去内存数据库里查一下，确保数据真的落地了
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();
                
                var createdPokemon = await db.Pokemons.FirstOrDefaultAsync(p => p.Name == "Gengar");
                
                Assert.NotNull(createdPokemon); // 必须能查到
                Assert.True(createdPokemon.IsInTeam); // 业务逻辑验证：通过这个接口加的应该在 Team 里
            }
        }

        // ===============================================
        // 测试 2: 验证释放宝可梦功能 (Soft Delete)
        // ===============================================
        [Fact (DisplayName = "Should_Soft_Delete_Pokemon")]
        public async Task ReleasePokemon_Should_Mark_As_Released()
        {
            // 1. Arrange: 从预设数据里随便找一只 (比如 Gardevoir)
            int targetId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();
                var target = await db.Pokemons.FirstOrDefaultAsync(p => p.Name.Contains("Gardevoir"));
                Assert.NotNull(target); // 确保预设数据加载成功
                targetId = target.Id;
            }

            // 2. Act: 发 DELETE 请求
            var response = await _client.DeleteAsync($"/api/PokemonPC/{targetId}");

            // 3. Assert
            response.EnsureSuccessStatusCode(); // 200 OK

            // 验证数据库状态
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();
                var deletedPokemon = await db.Pokemons.FindAsync(targetId);
                
                Assert.NotNull(deletedPokemon); // 还在库里 (因为是 Soft Delete)
                Assert.True(deletedPokemon.IsReleased); // 关键验证：IsReleased 应该是 True
            }
        }

        // ===============================================
        // 测试 3: 验证队伍和 PC 交换 (Atomic Swap)
        // ===============================================
        [Fact (DisplayName = "Should_Swap_Pokemon_Between_Team_And_PC")]
        public async Task SwapPokemon_Should_Exchange_Status()
        {
            // 1. Arrange: 找一只在队伍里的(如果是新建的通常默认在)，和一只在PC里的
            Guid teamMonGuid, pcMonGuid;
            
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();
                
                // 为了测试稳健，我们手动造两只状态确定的
                var p1 = new Pokemon { Name = "TeamMember", IsInTeam = true, GuidId = Guid.NewGuid() };
                var p2 = new Pokemon { Name = "PCMember", IsInTeam = false, GuidId = Guid.NewGuid() };
                
                db.Pokemons.AddRange(p1, p2);
                await db.SaveChangesAsync();

                teamMonGuid = p1.GuidId;
                pcMonGuid = p2.GuidId;
            }

            // 2. Act: 发 POST 请求交换
            // 注意: 你的 Controller 接受的是 Query Parameters，不是 Body
            var url = $"/api/PokemonPC/teampc/swap?pokemonInTeamGuid={teamMonGuid}&pokemonInPcId={pcMonGuid}";
            var response = await _client.PostAsync(url, null); // Body 是 null

            // 3. Assert
            response.EnsureSuccessStatusCode();

            // 验证交换结果
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();
                
                var newPcMon = await db.Pokemons.FirstOrDefaultAsync(p => p.GuidId == teamMonGuid);
                var newTeamMon = await db.Pokemons.FirstOrDefaultAsync(p => p.GuidId == pcMonGuid);

                Assert.False(newPcMon.IsInTeam); // 原来的 TeamMember 现在应该不在队伍了
                Assert.True(newTeamMon.IsInTeam); // 原来的 PCMember 现在应该进队了
            }
        }


        // ===============================================
        // TODO: 练习题 - 失败场景测试 (Failure Scenarios)
        // ===============================================


        [Fact (DisplayName = "should_fail_to_swap_if_target_already_in_team")]
        public async Task swappokemon_should_return_conflict_if_target_in_team()
        {

                // 1. Arrange: Two pokemon but both in team
                Guid teamMonGuid, teamTwoMonGuid;

                using (var scope = _factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DemoApi.Infrastructure.Data.PokemonDbContext>();

                    // 为了测试稳健，我们手动造两只状态确定的
                    var p1 = new Pokemon { Name = "TeamMember", IsInTeam = true, GuidId = Guid.NewGuid() };
                    var p2 = new Pokemon { Name = "TeamMember2", IsInTeam = true, GuidId = Guid.NewGuid() };

                    db.Pokemons.AddRange(p1, p2);
                    await db.SaveChangesAsync();

                    teamMonGuid = p1.GuidId;
                    teamTwoMonGuid = p2.GuidId;
                }

                // 2. act
                // 注意: 你的 Controller 接受的是 Query Parameters，不是 Body
                var url = $"/api/PokemonPC/teampc/swap?pokemonInTeamGuid={teamMonGuid}&pokemonInPcId={teamTwoMonGuid}";
                var response = await _client.PostAsync(url, null); // Body 是 null

                // 3. response
                // 注意： 其实我们在写failed case的时候我们是不需要 response.EnsureSuccessStatusCode();
                // 因为他本身就会fail 我们只要查response里面的东西就可以了
                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        }


        /*
        [Fact (DisplayName = "Should_Fail_To_Release_Unknown_Pokemon")]
        public async Task ReleasePokemon_Should_Return_NotFound_If_Id_Invalid()
        {
            // 提示 1: Arrange
            // 随便编一个不存在的 ID，比如 999999。

            // 提示 2: Act
            // 发 DELETE 请求。

            // 提示 3: Assert
            // 验证 response.StatusCode 是不是 HttpStatusCode.NotFound (404)。
        }
        */
    }
}
