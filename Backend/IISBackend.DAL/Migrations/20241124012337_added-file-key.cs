using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedfilekey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "FileEntities",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "FileEntities");
        }
    }
}
