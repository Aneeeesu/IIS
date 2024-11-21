using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class madeusedfiletagdynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalEntities_ImageEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ImageEntities_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AnimalEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "ImageEntities");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ImageId",
                table: "AspNetUsers",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalEntities_ImageId",
                table: "AnimalEntities",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalEntities_ImageEntities_ImageId",
                table: "AnimalEntities",
                column: "ImageId",
                principalTable: "ImageEntities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ImageEntities_ImageId",
                table: "AspNetUsers",
                column: "ImageId",
                principalTable: "ImageEntities",
                principalColumn: "Id");
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

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AnimalEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "ImageEntities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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
    }
}
