using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Service.Services;
using DemoApi.Service.Requests;
using Moq; // 引入 Moq 库
using Xunit;
using AutoMapper;
using Microsoft.Extensions.DependencyModel;
using FluentAssertions;

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
            // 这样 Service 调用的 "数据库" 其实是控制的假对象
            _service = new PokemonPCService(_mockRepo.Object, _mockMapper.Object);
        }
        #region Get Pokemon Team Test
        [Fact (DisplayName = "GetTeamAsync_Should_Return_Team_When_Success")]
        public async Task GetTeamAsync_Should_Return_Team_When_Success() 
        {
            // 1. Arrange
            var fullTeamList = new List<Pokemon>
            {
                 new Pokemon(){Name = "1"}, new Pokemon(){Name = "2"}, new Pokemon(){Name = "3"},
                 new Pokemon(){Name = "4"}, new Pokemon(){Name = "5"}, new Pokemon(){Name = "6"}
            };
            var expectedTeam = new Team()
            { 
                Pokemons = fullTeamList, 
                TeamSize = fullTeamList.Count 
            };

            _mockRepo.Setup(repo => repo.GetTeamAsync())
                .ReturnsAsync(fullTeamList);
            
            // Mock Mapper: List<Pokemon> -> List<Pokemon>
            _mockMapper.Setup(m => m.Map<List<Pokemon>>(fullTeamList))
                .Returns(fullTeamList);

            // 2. Act
            var result = await _service.GetTeamAsync();

            // 3. Assert
            // use FluentAssertions to compare object content
            result.Should().BeEquivalentTo(expectedTeam);
        }
        #endregion

        #region Get Pokemon By Id
        [Fact(DisplayName = "GetPokemonInPCGuidIDAsync_Should_Return_Pokemon_When_Success")]
        public async Task GetPokemonInPCByGuidAsync_Should_Return_Pokemon_When_Success() 
        {
            // Arrange
            var thisGuid = Guid.NewGuid();
            var pokemon = new Pokemon() {Name = "Test", Id = 666, GuidId = thisGuid};
            _mockRepo.Setup(repo => repo.GetPokemonInPCByGuidAsync(pokemon.GuidId))
                .ReturnsAsync(pokemon);

            // Act
            var result = await _service.GetPokemonInPCByGuidAsync(thisGuid);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(pokemon);
        }
        #endregion

        #region Add Pokemon To Team Tests
        // ==========================================
        // 测试 1: 验证 "队伍满员时，不能添加宝可梦"
        // ==========================================
        [Fact(DisplayName = "CreatePokemon_Should_Fail_When_Team_Is_Full")]
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
            _mockMapper.Setup(m => m.Map<List<Pokemon>>(fullTeamList))
                     .Returns(fullTeamList);

            var request = new CreatePokemonToTeamRequest { Name = "Pikachu" };

            // 2. Act
            var result = await _service.CreatePokemonToTeamAsync(request);

            // 3. Assert (验证)
            // 既然队伍满了，Service 应该返回 Failure，并且 ErrorCode 必须是 TeamIsFull
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be("TeamIsFull");

            // 验证：Repo 的 AddAsync 方法 **绝对没有** 被调用过 (因为逻辑被拦截了)
            _mockRepo.Verify(repo => repo.CreatePokemonToTeamAsync(It.IsAny<Pokemon>()), Times.Never);
        }

        // ==========================================
        // 测试 2: 验证 "正常添加宝可梦"
        // ==========================================
        [Fact(DisplayName = "CreatePokemon_Should_Succeed_When_Slot_Available")]
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
            _mockMapper.Setup(m => m.Map<List<Pokemon>>(notFullTeamList))
                     .Returns(notFullTeamList);


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
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be("Pikachu");

            // 验证：Repo 的 AddAsync 方法 **被调用了一次**
            _mockRepo.Verify(repo => repo.CreatePokemonToTeamAsync(It.IsAny<Pokemon>()), Times.Once);
        }
        #endregion

        #region Release Pokemon Tests
        // ==========================================
        // 测试 3: 验证 "释放宝可梦 - 未找到ID"
        // ==========================================
        [Fact(DisplayName = "ReleasePokemon_Should_Fail_If_Id_Not_Found")]
        public async Task ReleasePokemon_Should_Return_NotFound_If_Pokemon_Missing()
        {
            // 1. Arrange
            // Repo 查 ID=999 时，返回 null (没找到)
            _mockRepo.Setup(repo => repo.GetPokemonInPCByIDAsync(999))
                     .ReturnsAsync((Pokemon)null);

            // 2. Act
            var result = await _service.ReleasePokemon(999);

            // 3. Assert
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be("NotFound");
        }

        // ==========================================
        // 测试 4: 验证 "释放宝可梦 - Should return Pokemon Entity"
        // ==========================================

        [Fact(DisplayName = "ReleasePokemon_Should_Return_Pokemon_If_Success")]
        public async Task ReleasePokemon_Should_Return_Pokemon_If_Success() 
        {
            // 1 . Arrange
            var pokemon = new Pokemon()
            {
                Name = "Test",
                IsReleased = true,
                Id = 123
            };
            _mockRepo.Setup(repo => repo.GetPokemonInPCByIDAsync(pokemon.Id))
                .ReturnsAsync(pokemon);
            _mockRepo.Setup(repo => repo.ReleasePokemon(pokemon.Id))
                .ReturnsAsync(pokemon);

            // 2. Act
            var result = await _service.ReleasePokemon(pokemon.Id);

            // 3. Assert
            result.Success.Should().BeTrue();
            result.Data.IsReleased.Should().BeTrue();
        }
        #endregion
    }
}
