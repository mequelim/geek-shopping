using GeekShopping.OrderAPI.Infrastructure.Database.Configurations;
using GeekShopping.OrderAPI.Infrastructure.Persistence.Configurations;
using GeekShopping.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Infrastructure.Database
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
        /// Gets or sets the collection of OrderDetail entities in the database.
        /// </summary>
        /// <remarks>
        /// This property maps to a table in the database containing details for individual order items.
        /// It is represented as a <c>DbSet</c> within the <c>AppDbContext</c>, enabling LINQ queries and CRUD operations on OrderDetail entities.
        /// Each entry in this collection corresponds to an instance of the <c>OrderDetail</c> model.
        /// </remarks>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// Gets or sets the collection of OrderHeader entities in the database.
        /// </summary>
        /// <remarks>
        /// This property maps to a table in the database containing records for order headers, which store overarching details about customer orders such as user information,
        /// payment details, and order totals.
        /// It is represented as a <c>DbSet</c> within the <c>AppDbContext</c>, allowing LINQ queries and CRUD operations on OrderHeader entities.
        /// Each entry in this collection corresponds to an instance of the <c>OrderHeader</c> model.
        /// </remarks>
        public DbSet<OrderHeader> OrderHeaders { get; set; }

        // Methods:
        /// <summary>
        /// Configures the entity mappings, relationships, and conventions for the application database.
        /// </summary>
        /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> used to configure the database schema and entity relationships.</param>
        /// <remarks>
        /// This method is responsible for applying specific configurations to the database model, including:
        /// - Mapping entity configurations registered using <see cref="IEntityTypeConfiguration{TEntity}"/>.
        /// - Applying global conventions defined in the application.
        /// - Configuring additional properties and behaviors for specific entities, such as concurrency tokens and column mapping.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderDetailsTable());
            modelBuilder.ApplyConfiguration(new OrderHeadersTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    
            modelBuilder.Entity<OrderDetail>()
                .Property((orderDetail) => orderDetail.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();

            modelBuilder.Entity<OrderHeader>()
                .Property((orderHeader) => orderHeader.Version)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}