using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk_Backend_API.Migrations
{
    public partial class NewStaffAssigned : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_FusionAdmins_AssignedAdminId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Staffs_StaffId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_StaffId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "AssignedAdminId",
                table: "Tickets",
                newName: "StaffAssignedToId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_AssignedAdminId",
                table: "Tickets",
                newName: "IX_Tickets_StaffAssignedToId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Staffs_StaffAssignedToId",
                table: "Tickets",
                column: "StaffAssignedToId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Staffs_AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Staffs_StaffAssignedToId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Staffs");

            migrationBuilder.RenameColumn(
                name: "StaffAssignedToId",
                table: "Tickets",
                newName: "AssignedAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_StaffAssignedToId",
                table: "Tickets",
                newName: "IX_Tickets_AssignedAdminId");

            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StaffId",
                table: "Tickets",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_FusionAdmins_AssignedAdminId",
                table: "Tickets",
                column: "AssignedAdminId",
                principalTable: "FusionAdmins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Staffs_StaffId",
                table: "Tickets",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
