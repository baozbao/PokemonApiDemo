namespace DemoApi.Domain.Entities;

// 对应数据库表：PokemonPC
public class PokemonPC
{
    public int Id { get; set; }
    public Guid GuidId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = "Unknown";

    // 虽然表分开了，但你为了练习保留了这个字段 (默认为 false)
    public bool IsInTeam { get; set; } = false;

    public int Level { get; set; }
    public string Type { get; set; } = string.Empty;
}