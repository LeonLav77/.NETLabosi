using Microsoft.EntityFrameworkCore;
using Vjezba.Model;

namespace Vjezba.DAL
{
    public class ClientManagerDbContext : DbContext
    {
        protected ClientManagerDbContext() { }
        
        public ClientManagerDbContext(DbContextOptions<ClientManagerDbContext> options) : base(options)
        { }
        
        public DbSet<Client> Clients { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<City> Cities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Add seed data if needed
            modelBuilder.Entity<City>().HasData(
                new City { ID = 1, Name = "Zagreb" },
                new City { ID = 2, Name = "Pula" },
                new City { ID = 3, Name = "Rijeka" }
            );

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    ID = 1,
                    FirstName = "Leon",
                    LastName = "Kardas",
                    Email = "lkardas@example.com",
                    Gender = 'M',
                    Address = "Ilica 1",
                    PhoneNumber = "0912345678",
                    CityID = 2,
                    WorkingExperience = 5
                }
            );
        }
    }
}