using Random_User_Generator.Models;
using Microsoft.EntityFrameworkCore;

namespace Random_User_Generator.Data
{
    public class RandonUserGeneratorDataContext : DbContext
    {
        public DbSet<User> Users{ get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=random_user_generator;Username=postgres;Password=1234");
            
        }
    }
}
    