using api.Infrastructure.Database.Entities;
using api.Infrastructure.Database.Extensions;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure;

public class PostgressDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var builder = new ConfigurationBuilder()
           .AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        var configuration = builder.Build();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.UserDestination)
            .WithMany(b => b.TransactionsDestination)
            .HasForeignKey(t => t.UserDestinationId);





        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Converte o nome da entidade para snake_case


            entityType.SetTableName(entityType.GetTableName()?.ToSnakeCase());

            foreach (var property in entityType.GetProperties())
            {
                // Converte o nome da propriedade para snake_case
                property.SetColumnName(property.GetColumnName()?.ToSnakeCase());
            }
        }
    }


    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;
            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = now;
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }

            if (entity.State == EntityState.Modified)
            {
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }


}


