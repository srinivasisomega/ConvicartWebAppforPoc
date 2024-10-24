using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConvicartWebApp.Migrations
{
    /// <inheritdoc />
    public partial class frredi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "imgUrl",
                table: "OrderItems");

            

            

            migrationBuilder.AddColumn<byte[]>(
                name: "ProductImage",
                table: "OrderItems",
                type: "varbinary(max)",
                nullable: true);

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeSteps_Stores_ProductId",
                table: "RecipeSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeSteps",
                table: "RecipeSteps");

            migrationBuilder.DropIndex(
                name: "IX_RecipeSteps_ProductId",
                table: "RecipeSteps");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "StepNumber",
                table: "RecipeSteps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "RecipeSteps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RecipeSteps",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "imgUrl",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeSteps",
                table: "RecipeSteps",
                columns: new[] { "ProductId", "StepNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeSteps_Stores_ProductId",
                table: "RecipeSteps",
                column: "ProductId",
                principalTable: "Stores",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
