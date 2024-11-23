using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedanimalstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "VerificationRequestEntities");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AnimalEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnimalStatusEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    AnimalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AssociatedUserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalStatusEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalStatusEntity_AnimalEntities_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "AnimalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalStatusEntity_AspNetUsers_AssociatedUserId",
                        column: x => x.AssociatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalStatusEntity_AnimalId",
                table: "AnimalStatusEntity",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalStatusEntity_AssociatedUserId",
                table: "AnimalStatusEntity",
                column: "AssociatedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalStatusEntity");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AnimalEntities");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "VerificationRequestEntities",
                type: "longtext",
                nullable: false);
        }
    }
}
