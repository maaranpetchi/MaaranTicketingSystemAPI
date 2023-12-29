using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaranTicketingSystemAPI.Migrations
{
    public partial class UserDepartmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "users",
                newName: "Departmentid");

            migrationBuilder.RenameIndex(
                name: "IX_users_DepartmentId",
                table: "users",
                newName: "IX_users_Departmentid");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "Departmentid",
                table: "users",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_users_Departmentid",
                table: "users",
                newName: "IX_users_DepartmentId");

           
        }
    }
}
