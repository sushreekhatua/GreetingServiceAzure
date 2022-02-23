using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreetingService.Infrastructure.Migrations
{
    public partial class AddedUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.email);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
