using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk_Backend_API.Migrations
{
    public partial class NewStaffDtoUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Staffs_AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Staffs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedStaffId",
                table: "Staffs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "Staffs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_AssignedStaffId",
                table: "Staffs",
                column: "AssignedStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Staffs_AssignedStaffId",
                table: "Staffs",
                column: "AssignedStaffId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }
    }
}
