using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace KlimaKontrol
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(): base("DefaultConnection")
        {

        }
        public DbSet<City> Cities { get; set; }

    }
}
