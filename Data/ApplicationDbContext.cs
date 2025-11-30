using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Models;
using System.Linq; 
using System.Reflection; 

namespace MyMvcAuthProject.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore any ClientOptions type that Supabase or other packages might introduce.
        // This prevents EF Core from trying to map external types as entities.
        try
        {
            var clientOptionsType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == "ClientOptions");

            if (clientOptionsType != null)
            {
                var method = typeof(ModelBuilder)
                    .GetMethod("Ignore", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new Type[] { }, null)?
                    .MakeGenericMethod(clientOptionsType);

                method?.Invoke(modelBuilder, null);
            }
        }
        catch
        {
            // Silently ignore if the type cannot be found or ignored
        }
    }
}
