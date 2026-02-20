using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSpot.Migrations
{
    /// <inheritdoc />
    public partial class updatedbayNow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_baynow_tbl_cart_cart_id",
                table: "tbl_baynow");

            migrationBuilder.DropIndex(
                name: "IX_tbl_baynow_cart_id",
                table: "tbl_baynow");

            migrationBuilder.DropColumn(
                name: "cart_id",
                table: "tbl_baynow");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_baynow_carts_id",
                table: "tbl_baynow",
                column: "carts_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_baynow_tbl_cart_carts_id",
                table: "tbl_baynow",
                column: "carts_id",
                principalTable: "tbl_cart",
                principalColumn: "cart_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_baynow_tbl_cart_carts_id",
                table: "tbl_baynow");

            migrationBuilder.DropIndex(
                name: "IX_tbl_baynow_carts_id",
                table: "tbl_baynow");

            migrationBuilder.AddColumn<int>(
                name: "cart_id",
                table: "tbl_baynow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_baynow_cart_id",
                table: "tbl_baynow",
                column: "cart_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_baynow_tbl_cart_cart_id",
                table: "tbl_baynow",
                column: "cart_id",
                principalTable: "tbl_cart",
                principalColumn: "cart_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
