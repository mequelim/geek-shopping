using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeekShopping.OrderAPI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabaseAndTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_header",
                columns: table => new
                {
                    order_header_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    coupon_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    purchase_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    first_name = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: false),
                    last_name = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: false),
                    purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    order_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: false),
                    card_number = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    expired_month_year = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    cvv = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_header", x => x.order_header_id);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_header_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    order_value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.order_id);
                    table.CheckConstraint("CK_cart_details_count_positive", "count >= 1");
                    table.ForeignKey(
                        name: "f_k_order_details_order_header_order_header_id",
                        column: x => x.order_header_id,
                        principalTable: "order_header",
                        principalColumn: "order_header_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_order_details_order_header_id",
                table: "order_details",
                column: "order_header_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "order_header");
        }
    }
}
