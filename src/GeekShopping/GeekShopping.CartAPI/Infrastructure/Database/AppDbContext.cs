using GeekShopping.CartAPI.Infrastructure.Database.Configurations;
using GeekShopping.CartAPI.Infrastructure.Persistence.Configurations;
using GeekShopping.CartAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Infrastructure.Database
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
        /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="AppDbContext"/>.</param>
        /// <remarks>
        /// This constructor is used to configure the database context with specific settings, such as the connection string, database provider, and other options required for the Entity Framework Core
        /// context.
        /// </remarks>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets:
        /// <summary>
        /// Gets or sets the database set for the <see cref="Product"/> entity.
        /// </summary>
        /// <remarks>
        /// This property allows querying and manipulating product-related data within the database.
        /// It represents the collection of all <see cref="Product"/> entities in the context and maps to the corresponding table in the database.
        /// </remarks>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the database set for the <see cref="CartHeader"/> entity.
        /// </summary>
        /// <remarks>
        /// This property enables querying and working with shopping cart header data within the database.
        /// It represents the collection of all <see cref="CartHeader"/> entities in the context and maps to the corresponding table in the database.
        /// </remarks>
        public DbSet<CartHeader> CartHeaders { get; set; }

        /// <summary>
        /// Gets or sets the database set for the <see cref="CartDetails"/> entity.
        /// </summary>
        /// <remarks>
        /// This property provides access to the collection of all <see cref="CartDetails"/> entities in the database context.
        /// It allows querying and managing the details of shopping cart items, such as product associations and quantities.
        /// </remarks>
        public DbSet<CartDetail> CartDetails { get; set; }

        // Methods:
        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> used to define the shape of entities, relationships, and database mappings.</param>
        /// <remarks>
        /// This method applies entity configurations, global conventions, and additional customizations to the model.
        /// It ensures proper mapping of entities such as <see cref="Product"/>, <see cref="CartHeader"/>, and <see cref="CartDetails"/>.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProductsTable());
            modelBuilder.ApplyConfiguration(new CartHeaderTable());
            modelBuilder.ApplyConfiguration(new CartDetailsTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<CartHeader>()
                .Property((cart) => cart.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            modelBuilder.Entity<CartDetail>()
                .Property((cart) => cart.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}