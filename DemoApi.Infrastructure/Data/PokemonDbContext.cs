using DemoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApi.Infrastructure.Data
{
    public class PokemonDbContext : DbContext
    {
        public PokemonDbContext(DbContextOptions<PokemonDbContext> options):base(options) 
        {
        
        }

        public DbSet<PokemonPC> PokemonPC { get; set; }
        public DbSet<PokemonTeam> PokemonTeam { get; set; }

    }
}
