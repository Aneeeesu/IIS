using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class animalbirtday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age",
                table: "AnimalEntities");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AnimalEntities",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AnimalEntities");

            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "AnimalEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
