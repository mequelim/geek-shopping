using GeekShopping.Email.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.Email.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides configuration for the database table associated with the <see cref="EmailLog"/> entity.
    /// </summary>
    /// <remarks>
    /// This class defines the mappings for the <see cref="EmailLog"/> entity in the database, including the table name, primary key, column names, default values, and
    /// relational mappings.
    /// </remarks>
    public class EmailLogTable : IEntityTypeConfiguration<Model.EmailLog>
    {
        /// <summary>
        /// Configures the database table and mappings for the <see cref="EmailLog"/> entity.
        /// </summary>
        /// <param name="builder">
        /// An <see cref="EntityTypeBuilder{TEntity}"/> instance used for configuring the properties, relationships, and mapping of the <see cref="EmailLog"/> entity to
        /// the database.
        /// </param>
        public void Configure(EntityTypeBuilder<Model.EmailLog> builder)
        {
            builder.ToTable("emails");

            //* Id (GUID):
            builder.HasKey((email) => email.Id);  // Primary Key (PK).

            builder
                .Property((email) => email.Id)
                .HasColumnName("email_id")
                .IsRequired();

            //* Other fields:
            builder
                .Property((email) => email.Email)
                .HasColumnName(("email_address"))
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property((email) => email.Log)
                .HasColumnName(("log"))
                .HasMaxLength(500)
                .IsRequired();

            //* Dates:
            builder
                .Property((email) => email.SentDate)
                .HasColumnName("send_date")
                .HasDefaultValueSql("NOW()");
        }
    }
}