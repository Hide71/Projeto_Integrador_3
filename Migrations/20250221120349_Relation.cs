using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Controle_Pessoal.Migrations
{
    /// <inheritdoc />
    public partial class Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "categoryId",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_categoryId",
                table: "Expenses",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_userId",
                table: "Expenses",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_categoryId",
                table: "Expenses",
                column: "categoryId",
                principalTable: "Categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_userId",
                table: "Expenses",
                column: "userId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_categoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_userId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_categoryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_userId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "categoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");
        }
    }
}
