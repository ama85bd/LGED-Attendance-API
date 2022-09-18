using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LGED.Model.Migrations
{
    public partial class InitImageDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "AspNetUsers");
        }
    }
}
