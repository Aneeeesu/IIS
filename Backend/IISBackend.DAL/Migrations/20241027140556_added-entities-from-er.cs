using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IISBackend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addedentitiesfromer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthRecordsEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRecordsEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRecordsEntities_AnimalEntities_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "AnimalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthRecordsEntities_AspNetUsers_VetId",
                        column: x => x.VetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationRequestEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationRequestEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationRequestEntities_AnimalEntities_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "AnimalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationRequestEntities_AspNetUsers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolunteerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleEntities_AnimalEntities_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "AnimalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEntities_AspNetUsers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesteeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationRequests_AspNetUsers_RequesteeID",
                        column: x => x.RequesteeID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecordsEntities_AnimalId",
                table: "HealthRecordsEntities",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecordsEntities_VetId",
                table: "HealthRecordsEntities",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRequestEntities_AnimalId",
                table: "ReservationRequestEntities",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRequestEntities_VolunteerId",
                table: "ReservationRequestEntities",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntities_AnimalId",
                table: "ScheduleEntities",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntities_VolunteerId",
                table: "ScheduleEntities",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_RequesteeID",
                table: "VerificationRequests",
                column: "RequesteeID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthRecordsEntities");

            migrationBuilder.DropTable(
                name: "ReservationRequestEntities");

            migrationBuilder.DropTable(
                name: "ScheduleEntities");

            migrationBuilder.DropTable(
                name: "VerificationRequests");
        }
    }
}
