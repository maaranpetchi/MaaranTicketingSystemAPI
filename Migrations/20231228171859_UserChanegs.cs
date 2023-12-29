using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaranTicketingSystemAPI.Migrations
{
    public partial class UserChanegs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.DropIndex(
                name: "IX_users_Departmentid",
                table: "users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_Departmentid",
                table: "users",
                column: "Departmentid");

          
        }
    }
}
