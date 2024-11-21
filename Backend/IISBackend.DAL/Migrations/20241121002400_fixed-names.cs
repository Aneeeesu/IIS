using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixednames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "ReservationRequestEntities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "ReservationRequestEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
