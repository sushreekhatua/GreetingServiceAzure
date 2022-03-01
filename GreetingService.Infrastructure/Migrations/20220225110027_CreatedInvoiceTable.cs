using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreetingService.Infrastructure.Migrations
{
    public partial class CreatedInvoiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "modified",
                table: "Users",
                newName: "Modified");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Users",
                newName: "Last_name");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Users",
                newName: "First_name");

            migrationBuilder.RenameColumn(
                name: "created",
                table: "Users",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Greetings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    senderEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InvoiceYear = table.Column<int>(type: "int", nullable: false),
                    InvoiceMonth = table.Column<int>(type: "int", nullable: false),
                    CostperGreeting = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Totalcost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_senderEmail",
                        column: x => x.senderEmail,
                        principalTable: "Users",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Greetings_InvoiceId",
                table: "Greetings",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_senderEmail",
                table: "Invoices",
                column: "senderEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Greetings_Invoices_InvoiceId",
                table: "Greetings",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Greetings_Invoices_InvoiceId",
                table: "Greetings");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Greetings_InvoiceId",
                table: "Greetings");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Greetings");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Modified",
                table: "Users",
                newName: "modified");

            migrationBuilder.RenameColumn(
                name: "Last_name",
                table: "Users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "First_name",
                table: "Users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Users",
                newName: "created");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");
        }
    }
}
