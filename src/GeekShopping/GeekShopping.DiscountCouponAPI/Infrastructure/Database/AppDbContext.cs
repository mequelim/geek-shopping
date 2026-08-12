using GeekShopping.DiscountCouponAPI.Infrastructure.Database.Configurations;
using GeekShopping.DiscountCouponAPI.Infrastructure.Persistence.Configurations;
using GeekShopping.DiscountCouponAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.DiscountCouponAPI.Infrastructure.Database
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
        /// Gets or sets the collection of products in the database context.
        /// </summary>
        /// <remarks>
        /// This property represents the database set for the <see cref="Coupon"/> entity in the application's database context.
        /// It facilitates querying, inserting, updating, and deleting records corresponding to products in the underlying database.
        /// </remarks>
        public DbSet<Coupon> Coupons { get; set; }

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

            modelBuilder.ApplyConfiguration(new CouponsTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    
            modelBuilder.Entity<Coupon>()
                .Property((product) => product.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}