using Microsoft.EntityFrameworkCore;
using PersonManagerApi.Models;

namespace PersonManagerApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
    }
}
