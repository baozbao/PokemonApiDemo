using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Service.Services;
using DemoApi.Service.Requests;
using Moq; // 引入 Moq 库
using Xunit;
using AutoMapper;

namespace DemoApi.Tests.Unit
{
    // ==========================================
    // 单元测试类: PokemonPCServiceTests
    // ==========================================
    // 这里的重点是 "Mock" (模拟)。
    // 我们只测试 Service 的逻辑，不应该真的去连数据库。
    // 所以由于 Service 依赖 Repository，我们要用假的 Repository 骗过它。
    public class PokemonPCServiceTests
    {
        private readonly Mock<IPokemonPCRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PokemonPCService _service;

        public PokemonPCServiceTests()
        {
            // 1. 创建 Mock 对象 (假的依赖项)
            _mockRepo = new Mock<IPokemonPCRepository>();
            _mockMapper = new Mock<IMapper>();

            // 2. 把假的依赖项注入到 Service 里
            // 这样 Service 调用的 "数据库" 其实是我们控制的假对象
            _service = new PokemonPCService(_mockRepo.Object, _mockMapper.Object);
        }

        // ==========================================
        // 测试 1: 验证 "队伍满员时，不能添加宝可梦"
        // ==========================================
        [Fact (DisplayName = "CreatePokemon_Should_Fail_When_Team_Is_Full")]
        public async Task CreatePokemonToTeam_Should_Fail_If_Team_Full()
        {
            // 1. Arrange (剧本设定)
            // 当 Service 问 Repo 要队伍信息时，Repo 返回一个满员的队伍。
            var fullTeamList = new List<Pokemon> 
            { 
                 new Pokemon(), new Pokemon(), new Pokemon(), 
                 new Pokemon(), new Pokemon(), new Pokemon() 
            };

            // Setup: 只要调用 GetTeamAsync，就返回这个列表
            _mockRepo.Setup(repo => repo.GetTeamAsync())
                     .ReturnsAsync(fullTeamList);

            var request = new CreatePokemonToTeamRequest { Name = "Pikachu" };

            // 2. Act
            var result = await _service.CreatePokemonToTeamAsync(request);

            // 3. Assert (验证)
            // 既然队伍满了，Service 应该返回 Failure，并且 ErrorCode 必须是 TeamIsFull
            Assert.False(result.Success);
            Assert.Equal("TeamIsFull", result.ErrorCode);

            // 验证：Repo 的 AddAsync 方法 **绝对没有** 被调用过 (因为逻辑被拦截了)
            _mockRepo.Verify(repo => repo.CreatePokemonToTeamAsync(It.IsAny<Pokemon>()), Times.Never);
        }

        // ==========================================
        // 测试 2: 验证 "正常添加宝可梦"
        // ==========================================
        [Fact (DisplayName = "CreatePokemon_Should_Succeed_When_Slot_Available")]
        public async Task CreatePokemonToTeam_Should_Succeed_If_Not_Full()
        {
            // 1. Arrange
            // 队伍只有 5 只
            var notFullTeamList = new List<Pokemon> 
            { 
                new Pokemon(), new Pokemon(), new Pokemon(), new Pokemon(), new Pokemon() 
            };

            _mockRepo.Setup(repo => repo.GetTeamAsync())
                     .ReturnsAsync(notFullTeamList);

            // Mapper 正常工作，把 Request 变成 Entity
            var newPokemon = new Pokemon { Name = "Pikachu" };
            _mockMapper.Setup(m => m.Map<Pokemon>(It.IsAny<CreatePokemonToTeamRequest>()))
                       .Returns(newPokemon);
            
            // Repo 添加成功后返回这个 Pokemon
            _mockRepo.Setup(repo => repo.CreatePokemonToTeamAsync(It.IsAny<Pokemon>()))
                     .ReturnsAsync(newPokemon);

            var request = new CreatePokemonToTeamRequest { Name = "Pikachu" };

            // 2. Act
            var result = await _service.CreatePokemonToTeamAsync(request);

            // 3. Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Pikachu", result.Data.Name);

            // 验证：Repo 的 AddAsync 方法 **被调用了一次**
            _mockRepo.Verify(repo => repo.CreatePokemonToTeamAsync(It.IsAny<Pokemon>()), Times.Once);
        }

        // ==========================================
        // 测试 3: 验证 "释放宝可梦 - 未找到ID"
        // ==========================================
        [Fact (DisplayName = "ReleasePokemon_Should_Fail_If_Id_Not_Found")]
        public async Task ReleasePokemon_Should_Return_NotFound_If_Pokemon_Missing()
        {
            // 1. Arrange
            // Repo 查 ID=999 时，返回 null (没找到)
            _mockRepo.Setup(repo => repo.GetPokemonInPCByIDAsync(999))
                     .ReturnsAsync((Pokemon)null);

            // 2. Act
            var result = await _service.ReleasePokemon(999);

            // 3. Assert
            Assert.False(result.Success);
            Assert.Equal("NotFound", result.ErrorCode);
        }
    }
}
