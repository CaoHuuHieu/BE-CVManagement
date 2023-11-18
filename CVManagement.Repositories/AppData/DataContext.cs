using CVManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<CV>? CVs { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<UserCV>? UserCVs {  get; set; }
        public DbSet<Notification>? Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCV>()
                .HasKey(uv => new { uv.CustomerId, uv.CVId });
            modelBuilder.Entity<User>()
             .HasMany(u => u.Customers)
             .WithMany(u => u.HumanResources)
             .UsingEntity<Dictionary<string, object>>(
                 "UserManager",
                 j => j
                     .HasOne<User>()
                     .WithMany()
                     .HasForeignKey("CustomerId"),
                 j => j
                     .HasOne<User>()
                     .WithMany()
                     .HasForeignKey("HrId"),
                 j =>
                 {
                     j.HasKey("CustomerId", "HrId");
                 });

        }
    }
}
