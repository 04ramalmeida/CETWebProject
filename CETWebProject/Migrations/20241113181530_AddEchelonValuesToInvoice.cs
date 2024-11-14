using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class AddEchelonValuesToInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "invoices",
                newName: "TotalValue");

            migrationBuilder.AddColumn<decimal>(
                name: "FirstDecimalValue",
                table: "invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SecondDecimalValue",
                table: "invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ThirdDecimalValue",
                table: "invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstDecimalValue",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "SecondDecimalValue",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "ThirdDecimalValue",
                table: "invoices");

            migrationBuilder.RenameColumn(
                name: "TotalValue",
                table: "invoices",
                newName: "Value");
        }
    }
}
