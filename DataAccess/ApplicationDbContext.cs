global using Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    /// <summary>
    /// В задании просят настроить связь многие-ко-многим, с помощью EFCore.
    /// Насколько я понимаю, не подразумевается использование дополнительной сущности,
    /// но, по моему опыту, лучше её всё-таки использовать: это значительно упрощает работу и
    /// настройка с помощью EFCore тут всё-таки проводится
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Subscription>().
            HasKey(e => new { e.FromId, e.ToId });
        modelBuilder.Entity<Subscription>()
            .HasOne(x => x.SubscribedTo)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.ToId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Subscription>()
            .HasOne(x => x.SubscribedFrom)
            .WithMany(x => x.Following)
            .HasForeignKey(x => x.FromId);
    }
}