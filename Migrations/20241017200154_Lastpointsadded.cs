using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvicartWebApp.Migrations
{
    /// <inheritdoc />
    public partial class Lastpointsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPointsAddedDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPointsAddedDate",
                table: "Customers");
        }
    }
}
