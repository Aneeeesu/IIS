using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedimages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "AnimalEntities",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ImageEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Url = table.Column<string>(type: "varchar(255)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FileType = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageEntities_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PendingFileUploadEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Url = table.Column<string>(type: "longtext", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UploaderId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingFileUploadEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingFileUploadEntities_AspNetUsers_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ImageId",
                table: "AspNetUsers",
                column: "ImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalEntities_ImageId",
                table: "AnimalEntities",
                column: "ImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageEntities_OwnerId",
                table: "ImageEntities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageEntities_Url",
                table: "ImageEntities",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingFileUploadEntities_UploaderId",
                table: "PendingFileUploadEntities",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalEntities_ImageEntities_ImageId",
                table: "AnimalEntities",
                column: "ImageId",
                principalTable: "ImageEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ImageEntities_ImageId",
                table: "AspNetUsers",
                column: "ImageId",
                principalTable: "ImageEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalEntities_ImageEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ImageEntities_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ImageEntities");

            migrationBuilder.DropTable(
                name: "PendingFileUploadEntities");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AnimalEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "AnimalEntities");
        }
    }
}
