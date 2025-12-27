using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.Domain.Entities
{
    public class Team
    {
        public int TeamSize { get; set; }

        public List<Pokemon>? Pokemons { get; set; }
    }
}
