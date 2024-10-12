using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class ChangeMeterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_waterMeters_AspNetUsers_UserId",
                table: "waterMeters");

            migrationBuilder.DropIndex(
                name: "IX_waterMeters_UserId",
                table: "waterMeters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "waterMeters");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "waterMeters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "waterMeters");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "waterMeters",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_waterMeters_UserId",
                table: "waterMeters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_waterMeters_AspNetUsers_UserId",
                table: "waterMeters",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
