namespace DemoApi.Domain.Models
{
    // 纯粹的数据载体，给 Repository 用的“规格说明书”
    public class PokemonSearchFilter
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Specie { get; set; }
        public string? Gender { get; set; }
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
