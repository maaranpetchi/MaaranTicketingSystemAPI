using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaranTicketingSystemAPI.Migrations
{
    public partial class SecondCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "key",
                table: "users",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "key");
        }
    }
}
