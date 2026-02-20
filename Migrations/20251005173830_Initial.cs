using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSpot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    admin_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    admin_image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_admin", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_bestblog",
                columns: table => new
                {
                    blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blog_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    blog_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    blog_image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bestblog", x => x.blog_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_customer",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customer_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customer_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customer_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_adress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_customer", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_feedback",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_feedback", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    product_discount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cat_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_product", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_tbl_product_tbl_category_cat_id",
                        column: x => x.cat_id,
                        principalTable: "tbl_category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_cart",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    prod_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    product_quantity = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_cart", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_tbl_cart_tbl_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "tbl_customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_cart_tbl_product_prod_id",
                        column: x => x.prod_id,
                        principalTable: "tbl_product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_baynow",
                columns: table => new
                {
                    order_i = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carts_id = table.Column<int>(type: "int", nullable: false),
                    Bay_status_id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    cart_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_baynow", x => x.order_i);
                    table.ForeignKey(
                        name: "FK_tbl_baynow_tbl_cart_cart_id",
                        column: x => x.cart_id,
                        principalTable: "tbl_cart",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_baynow_cart_id",
                table: "tbl_baynow",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_customer_id",
                table: "tbl_cart",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_cart_prod_id",
                table: "tbl_cart",
                column: "prod_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_product_cat_id",
                table: "tbl_product",
                column: "cat_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_admin");

            migrationBuilder.DropTable(
                name: "tbl_baynow");

            migrationBuilder.DropTable(
                name: "tbl_bestblog");

            migrationBuilder.DropTable(
                name: "tbl_feedback");

            migrationBuilder.DropTable(
                name: "tbl_cart");

            migrationBuilder.DropTable(
                name: "tbl_customer");

            migrationBuilder.DropTable(
                name: "tbl_product");

            migrationBuilder.DropTable(
                name: "tbl_category");
        }
    }
}
