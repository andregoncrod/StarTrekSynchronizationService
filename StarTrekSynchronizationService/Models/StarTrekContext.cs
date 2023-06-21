using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekSynchronizationService.Models
{
    public class StarTrekContext : DbContext
    {
        public StarTrekContext(DbContextOptions<StarTrekContext> dbContextOptions): base(dbContextOptions)
        {
        }

        public DbSet<Spacecraft> Spacecrafts { get; set; }
    }
}
