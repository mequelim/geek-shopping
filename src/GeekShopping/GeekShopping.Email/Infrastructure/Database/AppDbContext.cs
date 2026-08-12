using GeekShopping.Email.Infrastructure.Database.Configurations;
using GeekShopping.Email.Infrastructure.Persistence.Configurations;
using GeekShopping.Email.Model;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Infrastructure.Database
{
    /// <summary>
    /// Represents the database context for the application, providing configuration and management of database operations for the application entities.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Represents the database context for the application, providing configuration and management of database operations for the application entities.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet:
        /// <summary>
        /// Gets or sets the collection of <c>EmailContent</c> entities within the database context.
        /// </summary>
        /// <remarks>
        /// This property represents the table of email-related content and logs in the database.
        /// Each record corresponds to an <c>EmailContent</c> entity, which includes email content details, logging information, and the date the email was sent.
        /// </remarks>
        public DbSet<EmailLog> Email { get; set; }

        // Methods:
        /// <summary>
        /// Configures the model for the database context, defining the structure, relationships, and conventions for the application's entity mappings.
        /// </summary>
        /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/> that provides the APIs for configuring the entity framework model. </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EmailLogTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}