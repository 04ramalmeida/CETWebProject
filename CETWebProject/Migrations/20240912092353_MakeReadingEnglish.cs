using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class MakeReadingEnglish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "dataDeLeitura",
                table: "monthlyReadings",
                newName: "ReadingTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReadingTime",
                table: "monthlyReadings",
                newName: "dataDeLeitura");
        }
    }
}
