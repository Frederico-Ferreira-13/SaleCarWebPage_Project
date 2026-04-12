using Microsoft.EntityFrameworkCore;
using Core.Model;
using Core.Common;

namespace SaleCarWebPage_Project.Repo
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define aqui os DataSets correspondentes às tuas tabelas
        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CarModel> Models { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<MessageBox> MessageBoxes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aqui podes configurar chaves compostas ou regras específicas se necessário
        }
    }
}