using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentSpot.Migrations
{
    /// <inheritdoc />
    public partial class updatedproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(

                
                name: "product_discount",
                table: "tbl_product");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "product_discount",
                table: "tbl_product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
