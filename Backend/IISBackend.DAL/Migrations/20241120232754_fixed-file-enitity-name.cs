using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixedfileenitityname : Migration
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

            migrationBuilder.DropForeignKey(
                name: "FK_ImageEntities_AspNetUsers_OwnerId",
                table: "ImageEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageEntities",
                table: "ImageEntities");

            migrationBuilder.RenameTable(
                name: "ImageEntities",
                newName: "FileEntities");

            migrationBuilder.RenameIndex(
                name: "IX_ImageEntities_Url",
                table: "FileEntities",
                newName: "IX_FileEntities_Url");

            migrationBuilder.RenameIndex(
                name: "IX_ImageEntities_OwnerId",
                table: "FileEntities",
                newName: "IX_FileEntities_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileEntities",
                table: "FileEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalEntities_FileEntities_ImageId",
                table: "AnimalEntities",
                column: "ImageId",
                principalTable: "FileEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FileEntities_ImageId",
                table: "AspNetUsers",
                column: "ImageId",
                principalTable: "FileEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileEntities_AspNetUsers_OwnerId",
                table: "FileEntities",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalEntities_FileEntities_ImageId",
                table: "AnimalEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FileEntities_ImageId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_FileEntities_AspNetUsers_OwnerId",
                table: "FileEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileEntities",
                table: "FileEntities");

            migrationBuilder.RenameTable(
                name: "FileEntities",
                newName: "ImageEntities");

            migrationBuilder.RenameIndex(
                name: "IX_FileEntities_Url",
                table: "ImageEntities",
                newName: "IX_ImageEntities_Url");

            migrationBuilder.RenameIndex(
                name: "IX_FileEntities_OwnerId",
                table: "ImageEntities",
                newName: "IX_ImageEntities_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageEntities",
                table: "ImageEntities",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ImageEntities_AspNetUsers_OwnerId",
                table: "ImageEntities",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
