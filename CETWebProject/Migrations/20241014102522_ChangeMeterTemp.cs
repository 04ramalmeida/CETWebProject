using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class ChangeMeterTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_metersTemp_AspNetUsers_UserId",
                table: "metersTemp");

            migrationBuilder.DropIndex(
                name: "IX_metersTemp_UserId",
                table: "metersTemp");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "metersTemp");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "metersTemp",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "metersTemp");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "metersTemp",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_metersTemp_UserId",
                table: "metersTemp",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_metersTemp_AspNetUsers_UserId",
                table: "metersTemp",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
