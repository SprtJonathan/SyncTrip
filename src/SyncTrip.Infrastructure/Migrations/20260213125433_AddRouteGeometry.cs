using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RouteDistanceMeters",
                table: "Trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RouteDurationSeconds",
                table: "Trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RouteGeometry",
                table: "Trips",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteDistanceMeters",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RouteDurationSeconds",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RouteGeometry",
                table: "Trips");
        }
    }
}
