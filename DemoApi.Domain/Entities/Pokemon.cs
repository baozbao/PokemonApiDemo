using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.Domain.Entities
{
    public class Pokemon
    {
        public int Id { get; set; }

        public Guid GuidId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = "Unknown";

        // true = 在身上, false = 在电脑里
        public bool IsInTeam { get; set; }

        public int Level { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
