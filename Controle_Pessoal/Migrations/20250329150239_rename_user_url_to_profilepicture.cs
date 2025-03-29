using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Controle_Pessoal.Migrations
{
    /// <inheritdoc />
    public partial class rename_user_url_to_profilepicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Users",
                newName: "ProfilePicture");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "Users",
                newName: "Url");
        }
    }
}
