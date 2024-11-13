using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class AddReadingToInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "readingId",
                table: "invoices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoices_readingId",
                table: "invoices",
                column: "readingId");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_monthlyReadings_readingId",
                table: "invoices",
                column: "readingId",
                principalTable: "monthlyReadings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_monthlyReadings_readingId",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_readingId",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "readingId",
                table: "invoices");
        }
    }
}
