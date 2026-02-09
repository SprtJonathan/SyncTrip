using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncTrip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTripFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConvoyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RouteProfile = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Convoys_ConvoyId",
                        column: x => x.ConvoyId,
                        principalTable: "Convoys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripWaypoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: false),
                    Longitude = table.Column<double>(type: "double precision", precision: 10, scale: 7, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AddedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripWaypoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripWaypoints_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripWaypoints_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ConvoyId_Status",
                table: "Trips",
                columns: new[] { "ConvoyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TripWaypoints_AddedByUserId",
                table: "TripWaypoints",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripWaypoints_TripId_OrderIndex",
                table: "TripWaypoints",
                columns: new[] { "TripId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripWaypoints");

            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
