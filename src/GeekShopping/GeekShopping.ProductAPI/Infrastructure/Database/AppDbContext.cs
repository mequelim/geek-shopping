using GeekShopping.ProductAPI.Infrastructure.Database.Configurations;
using GeekShopping.ProductAPI.Infrastructure.Persistence.Configurations;
using GeekShopping.ProductAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Infrastructure.Database
{
    /// <summary>
    /// Represents the Entity Framework database context for the application.
    /// </summary>
    /// <remarks>
    /// This class is responsible for managing the database connection, entity configurations, and interactions with the database for the application.
    /// It extends the <see cref="DbContext"/> class, providing an abstraction layer for querying and persisting objects.
    /// </remarks>
    public class AppDbContext : DbContext
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
        /// <summary>
        /// Represents the collection of products stored in the database.
        /// This property is used to perform CRUD operations on the <see cref="Product"/> entity within the application's data context.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        // Methods:
        /// <summary>
        /// Configures the entity mappings, relationships, and conventions for the application database.
        /// </summary>
        /// <param name="modelBuilder">
        /// An instance of <see cref="ModelBuilder"/> used to configure the database schema and entity relationships.
        /// </param>
        /// <remarks>
        /// This method is responsible for applying specific configurations to the database model, including:
        /// - Mapping entity configurations registered using <see cref="IEntityTypeConfiguration{TEntity}"/>.
        /// - Applying global conventions defined in the application.
        /// - Configuring additional properties and behaviors for specific entities, such as concurrency tokens and column mapping.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProductsTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    
            modelBuilder.Entity<Product>()
                .Property((product) => product.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}