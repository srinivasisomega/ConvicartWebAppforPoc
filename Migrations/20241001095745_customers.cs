using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvicartWebApp.Migrations
{
    /// <inheritdoc />
    public partial class customers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietId",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DietId",
                table: "Customers",
                type: "int",
                nullable: true);
        }
    }
}
