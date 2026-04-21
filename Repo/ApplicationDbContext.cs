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

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Car> Cars { get; set; }        
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<MessageBox> MessageBoxes { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UsersRole> UsersRoles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>().ToTable("Address");
            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<Car>().ToTable("Car");
            modelBuilder.Entity<CarModel>().ToTable("Model");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Contact>().ToTable("Contact");
            modelBuilder.Entity<Favorites>().ToTable("Favorites");
            modelBuilder.Entity<MessageBox>().ToTable("MessageBox");
            modelBuilder.Entity<Provider>().ToTable("Provider");
            modelBuilder.Entity<Sale>().ToTable("Sale");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<UserSettings>().ToTable("UserSetting");
            modelBuilder.Entity<UsersRole>().ToTable("UsersRole");

            modelBuilder.Entity<Car>(entity => {
                entity.Property(e => e.CarPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CarTare).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Sale>(entity => {
                entity.Property(e => e.FinalPrice).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Contact)
                .WithOne(con => con.Client)
                .HasForeignKey<Client>(c => c.ContactId);
        }
    }
}