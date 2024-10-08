using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvicartWebApp.Migrations
{
    /// <inheritdoc />
    public partial class url : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imgUrl",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgUrl",
                table: "Stores");
        }
    }
}
