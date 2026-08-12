using GeekShopping.DuendeIdentityServer.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.DuendeIdentityServer.Infrastructure.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor:
        /// <summary>
        /// Represents the Entity Framework database context for the application.
        /// </summary>
        /// <remarks>
        /// This context provides access to the database and manages entity configurations.
        /// It facilitates querying and persisting objects to a relational database by configuring entity mappings, relationships, and conventions.
        /// </remarks>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet:

        // Methods:
        /// <summary>
        /// Configures the entity framework model for the context by applying global conventions and entity configurations.
        /// </summary>
        /// <remarks>
        /// This method is called by Entity Framework when the model for the context is being created.
        /// It applies global conventions and scans the assembly for entity type configurations to include in the model.
        /// Override this method to customize model configuration for the context.
        /// </remarks>
        /// <param name="modelBuilder">
        /// The builder used to construct the model for the context.
        /// Provides configuration for entity types, relationships, and conventions.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}