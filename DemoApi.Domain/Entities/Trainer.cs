using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.Domain.Entities
{
    public class Trainer
    {
        public int Id { get; set; }

        public Guid GuidId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = "Unknown";
    }
}
