using DatabaseTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Tester.Models;

namespace Tester.Data
{
    public class TesterContext : DbContext
    {
        public TesterContext(DbContextOptions<TesterContext> options) :
            base(options){ }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();

            user.Property(e => e.UserId)
                .ValueGeneratedOnAdd();

            user.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            user.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()")
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

            var address = modelBuilder.Entity<Address>();

            address.Property(w => w.AddressId)
                .ValueGeneratedOnAdd();
        }
    }
}
