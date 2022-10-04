using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LGED.Model.Migrations
{
    public partial class InitAttendaceIamgeRepo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "AttendanceWithImage",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AttendanceWithImage");
        }
    }
}
