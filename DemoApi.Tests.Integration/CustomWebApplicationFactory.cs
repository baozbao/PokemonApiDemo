using System; // For Guid
using System.Linq; // For Any()
using DemoApi.Infrastructure.Data; // 引用你的 DbContext
using Microsoft.AspNetCore.Hosting; // 引用 WebHost 配置
using Microsoft.AspNetCore.Mvc.Testing; // 引用测试工厂基类
using Microsoft.EntityFrameworkCore; // 引用 EF Core
using Microsoft.Extensions.DependencyInjection; // 引用依赖注入

namespace DemoApi.Tests.Integration
{
    // ==========================================
    // 自定义 Web 工厂类 (Custom Web Application Factory)
    // ==========================================
    // 继承自 WebApplicationFactory<Program>
    // 泛型参数 <Program> 指的是 API 项目入口的那个 Program 类。
    // 作用是启动一个“内存中”的 API 服务器，所有配置默认跟真实项目一模一样
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        // 核心方法：配置 Web 主机
        // 我们在这里 "重写" (Override) 启动流程，把不想用的组件（比如 SQL Server）换掉。
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // ==========================================
                // 第一步：移除旧数据库 (SQL Server)
                // ==========================================
                // 去服务容器(Services)里找一下，目前的数据库配置是什么？
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PokemonDbContext>));

                // 如果找到了，把它移除掉。
                // 这样测试运行的时候，绝对不会连到你原本的 SQL Server，保护数据安全。
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // ==========================================
                // 第二步：注入新数据库 (In-Memory Database)
                // ==========================================
                // 添加一个新的 DbContext，配置为使用 "内存数据库"。
                // "PokemonTestDb" 是这个内存库的名字，每次跑测试它都是空的、干净的。
                services.AddDbContext<PokemonDbContext>(options =>
                {
                    options.UseInMemoryDatabase("PokemonTestDb");
                });

                // ==========================================
                // 第三步：确保数据库已创建 (初始化)
                // ==========================================
                // 这一步是为了确保内存数据库结构被建立起来。
                var sp = services.BuildServiceProvider();

                // 创建一个临时的 Scope (作用域) 来获取 DbContext
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<PokemonDbContext>();

                    // EnsureCreated 会根据你的 Entity 类自动创建表结构
                    db.Database.EnsureCreated();

                    // ==========================================
                    // [数据预设] 注入热门宝可梦 (Seeding)
                    // ==========================================
                    // 如果数据库是空的，那就塞点老婆进去 (咳咳，宝可梦)

                    #region Inject into Pokemons
                    if (!db.Pokemons.Any())
                    {
                        db.Pokemons.AddRange(
                            // Gen 3
                            new DemoApi.Domain.Entities.Pokemon { Name = "Gardevoir (沙奈朵)", Gender = "Female", Level = 50, Type = "Psychic/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Milotic (美纳斯)", Gender = "Female", Level = 55, Type = "Water", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Mawile (大嘴娃)", Gender = "Female", Level = 40, Type = "Steel/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },
                            
                            // Gen 4
                            new DemoApi.Domain.Entities.Pokemon { Name = "Lopunny (长耳兔)", Gender = "Female", Level = 45, Type = "Normal", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Glaceon (冰伊布)", Gender = "Female", Level = 30, Type = "Ice", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Leafeon (叶伊布)", Gender = "Female", Level = 30, Type = "Grass", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Froslass (雪妖女)", Gender = "Female", Level = 47, Type = "Ice/Ghost", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Roserade (罗丝雷朵)", Gender = "Female", Level = 44, Type = "Grass/Poison", IsInTeam = false, GuidId = Guid.NewGuid() },

                            // Gen 5
                            new DemoApi.Domain.Entities.Pokemon { Name = "Gothitelle (哥德小姐)", Gender = "Female", Level = 42, Type = "Psychic", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Lilligant (裙儿小姐)", Gender = "Female", Level = 35, Type = "Grass", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Snivy (藤藤蛇)", Gender = "Female", Level = 5, Type = "Grass", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Leavanny (保姆虫)", Gender = "Female", Level = 38, Type = "Bug/Grass", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Meloetta (美洛耶塔)", Gender = "Unknown", Level = 100, Type = "Normal/Psychic", IsInTeam = false, GuidId = Guid.NewGuid() },

                            // Gen 6
                            new DemoApi.Domain.Entities.Pokemon { Name = "Braixen (长尾火狐)", Gender = "Female", Level = 25, Type = "Fire", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Delphox (妖火红狐)", Gender = "Female", Level = 60, Type = "Fire/Psychic", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Sylveon (仙子伊布)", Gender = "Female", Level = 35, Type = "Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Florges (花洁夫人)", Gender = "Female", Level = 50, Type = "Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Diancie (蒂安希)", Gender = "Unknown", Level = 90, Type = "Rock/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },

                            // Gen 7
                            new DemoApi.Domain.Entities.Pokemon { Name = "Primarina (西狮海壬)", Gender = "Female", Level = 52, Type = "Water/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Salazzle (焰后蜥)", Gender = "Female", Level = 40, Type = "Poison/Fire", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Tsareena (甜冷美后)", Gender = "Female", Level = 48, Type = "Grass", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Pheromosa (费洛美螂)", Gender = "Zero", Level = 70, Type = "Bug/Fighting", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Magearna (玛机雅娜)", Gender = "Unknown", Level = 95, Type = "Steel/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },

                            // Gen 8
                            new DemoApi.Domain.Entities.Pokemon { Name = "Hatterene (布莉姆温)", Gender = "Female", Level = 55, Type = "Psychic/Fairy", IsInTeam = false, GuidId = Guid.NewGuid() },

                            // Eevee Family (Extras)
                            new DemoApi.Domain.Entities.Pokemon { Name = "Vaporeon (水伊布)", Gender = "Female", Level = 30, Type = "Water", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Espeon (太阳伊布)", Gender = "Female", Level = 30, Type = "Psychic", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Umbreon (月亮伊布)", Gender = "Female", Level = 30, Type = "Dark", IsInTeam = false, GuidId = Guid.NewGuid() },
                            new DemoApi.Domain.Entities.Pokemon { Name = "Chikorita (菊草叶)", Gender = "Female", Level = 5, Type = "Grass", IsInTeam = false, GuidId = Guid.NewGuid() }
                        );

                        db.SaveChanges();
                    }
                    #endregion
                }
            });
        }
    }
}