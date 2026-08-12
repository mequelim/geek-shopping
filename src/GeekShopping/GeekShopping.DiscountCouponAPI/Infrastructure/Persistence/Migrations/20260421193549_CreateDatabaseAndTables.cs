using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.DiscountCouponAPI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabaseAndTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    coupon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coupon_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    coupon_discount_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.coupon_id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_coupons_coupon_code",
                table: "coupons",
                column: "coupon_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coupons");
        }
    }
}
