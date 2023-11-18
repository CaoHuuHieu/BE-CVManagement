using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVManagement.Repositories.Migrations
{
    public partial class updatedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CVs",
                newName: "UploadName");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "CVs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "CVs");

            migrationBuilder.RenameColumn(
                name: "UploadName",
                table: "CVs",
                newName: "Name");
        }
    }
}
