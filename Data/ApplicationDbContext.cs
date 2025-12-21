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

        // Configure UserProfile entity
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("userprofile");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.UserImage).HasColumnName("user_image");
        });

        // Robustly ignore ClientOptions from external packages (like Supabase)
        // We iterate assemblies individually to handle TypeLoadExceptions safely
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var types = assembly.GetTypes().Where(t => t.Name == "ClientOptions");
                foreach (var type in types)
                {
                    modelBuilder.Ignore(type);
                }
            }
            catch
            {
                // Ignore assembly load errors
            }
        }
    }
}
