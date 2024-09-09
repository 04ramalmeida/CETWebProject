using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class addmeters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyReadings",
                table: "MonthlyReadings");

            migrationBuilder.RenameTable(
                name: "MonthlyReadings",
                newName: "monthlyReadings");

            migrationBuilder.AddColumn<int>(
                name: "WaterMeterId",
                table: "monthlyReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignUpDateTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_monthlyReadings",
                table: "monthlyReadings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "waterMeters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waterMeters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_waterMeters_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_monthlyReadings_WaterMeterId",
                table: "monthlyReadings",
                column: "WaterMeterId");

            migrationBuilder.CreateIndex(
                name: "IX_waterMeters_UserId",
                table: "waterMeters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_monthlyReadings_waterMeters_WaterMeterId",
                table: "monthlyReadings",
                column: "WaterMeterId",
                principalTable: "waterMeters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_monthlyReadings_waterMeters_WaterMeterId",
                table: "monthlyReadings");

            migrationBuilder.DropTable(
                name: "waterMeters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_monthlyReadings",
                table: "monthlyReadings");

            migrationBuilder.DropIndex(
                name: "IX_monthlyReadings_WaterMeterId",
                table: "monthlyReadings");

            migrationBuilder.DropColumn(
                name: "WaterMeterId",
                table: "monthlyReadings");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SignUpDateTime",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "monthlyReadings",
                newName: "MonthlyReadings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyReadings",
                table: "MonthlyReadings",
                column: "Id");
        }
    }
}
