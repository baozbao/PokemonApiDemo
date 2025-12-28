namespace DemoApi.Service.Requests
{
    public class CreatePokemonToTeamRequest
    {
        // 可选：如果有 Guid，就用它查是否已存在
        public Guid? GuidId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = "Unknown";
        public int Level { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
