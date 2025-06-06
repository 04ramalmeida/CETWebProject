﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CETWebProject.Migrations
{
    public partial class AddMeterReading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usageAmount",
                table: "monthlyReadings",
                newName: "UsageAmount");

            migrationBuilder.CreateTable(
                name: "AddReadingViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsageAmount = table.Column<float>(type: "real", nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WaterMeterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddReadingViewModel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddReadingViewModel");

            migrationBuilder.RenameColumn(
                name: "UsageAmount",
                table: "monthlyReadings",
                newName: "usageAmount");
        }
    }
}
