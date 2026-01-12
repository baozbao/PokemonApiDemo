namespace DemoApi.Service.Requests
{
    public class SearchPokemonRequest
    {
        // 搜索条件
        public string? Name { get; set; }

        public string? Type { get; set; }
        
        public string? Specie { get; set; }

        public string? Gender { get; set; }

        // 范围筛选 (比如按等级)
        public int? MinLevel { get; set; }

        public int? MaxLevel { get; set; }

        // 分页 (默认第1页)
        public int Page { get; set; } = 1;

        // 页大小 (默认10)
        public int PageSize { get; set; } = 10;
        
        // 排序字段 (预留)
        // public string? SortBy { get; set; }
    }
}
