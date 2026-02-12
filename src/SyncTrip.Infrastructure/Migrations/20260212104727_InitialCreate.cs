using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SyncTrip.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Convoys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    LeaderUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convoys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagicLinkTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicLinkTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DeactivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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
                name: "UserLicenses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LicenseType = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLicenses", x => new { x.UserId, x.LicenseType });
                    table.ForeignKey(
                        name: "FK_UserLicenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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

            migrationBuilder.CreateTable(
                name: "ConvoyMembers",
                columns: table => new
                {
                    ConvoyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvoyMembers", x => new { x.ConvoyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ConvoyMembers_Convoys_ConvoyId",
                        column: x => x.ConvoyId,
                        principalTable: "Convoys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConvoyMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvoyMembers_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://example.com/logos/yamaha.png", "Yamaha" },
                    { 2, "https://example.com/logos/honda.png", "Honda" },
                    { 3, "https://example.com/logos/kawasaki.png", "Kawasaki" },
                    { 4, "https://example.com/logos/suzuki.png", "Suzuki" },
                    { 5, "https://example.com/logos/bmw-motorrad.png", "BMW Motorrad" },
                    { 6, "https://example.com/logos/ducati.png", "Ducati" },
                    { 7, "https://example.com/logos/ktm.png", "KTM" },
                    { 8, "https://example.com/logos/harley-davidson.png", "Harley-Davidson" },
                    { 9, "https://example.com/logos/triumph.png", "Triumph" },
                    { 10, "https://example.com/logos/aprilia.png", "Aprilia" },
                    { 11, "https://example.com/logos/renault.png", "Renault" },
                    { 12, "https://example.com/logos/peugeot.png", "Peugeot" },
                    { 13, "https://example.com/logos/citroen.png", "Citroën" },
                    { 14, "https://example.com/logos/ds.png", "DS Automobiles" },
                    { 15, "https://example.com/logos/alpine.png", "Alpine" },
                    { 16, "https://example.com/logos/bmw.png", "BMW" },
                    { 17, "https://example.com/logos/mercedes.png", "Mercedes-Benz" },
                    { 18, "https://example.com/logos/audi.png", "Audi" },
                    { 19, "https://example.com/logos/volkswagen.png", "Volkswagen" },
                    { 20, "https://example.com/logos/porsche.png", "Porsche" },
                    { 21, "https://example.com/logos/ferrari.png", "Ferrari" },
                    { 22, "https://example.com/logos/lamborghini.png", "Lamborghini" },
                    { 23, "https://example.com/logos/fiat.png", "Fiat" },
                    { 24, "https://example.com/logos/alfa-romeo.png", "Alfa Romeo" },
                    { 25, "https://example.com/logos/toyota.png", "Toyota" },
                    { 26, "https://example.com/logos/nissan.png", "Nissan" },
                    { 27, "https://example.com/logos/mazda.png", "Mazda" },
                    { 28, "https://example.com/logos/subaru.png", "Subaru" },
                    { 29, "https://example.com/logos/mitsubishi.png", "Mitsubishi" },
                    { 30, "https://example.com/logos/ford.png", "Ford" },
                    { 31, "https://example.com/logos/chevrolet.png", "Chevrolet" },
                    { 32, "https://example.com/logos/tesla.png", "Tesla" },
                    { 33, "https://example.com/logos/jeep.png", "Jeep" },
                    { 34, "https://example.com/logos/hyundai.png", "Hyundai" },
                    { 35, "https://example.com/logos/kia.png", "Kia" },
                    { 36, "https://example.com/logos/fiat-professional.png", "Fiat Professional" },
                    { 37, "https://example.com/logos/iveco.png", "Iveco" },
                    { 38, "https://example.com/logos/man.png", "MAN" },
                    { 39, "https://example.com/logos/scania.png", "Scania" },
                    { 40, "https://example.com/logos/volvo-trucks.png", "Volvo Trucks" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvoyMembers_UserId",
                table: "ConvoyMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvoyMembers_VehicleId",
                table: "ConvoyMembers",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Convoys_JoinCode",
                table: "Convoys",
                column: "JoinCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Convoys_LeaderUserId",
                table: "Convoys",
                column: "LeaderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicLinkTokens_Email",
                table: "MagicLinkTokens",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_MagicLinkTokens_Token",
                table: "MagicLinkTokens",
                column: "Token");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserLicenses_UserId",
                table: "UserLicenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BrandId",
                table: "Vehicles",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvoyMembers");

            migrationBuilder.DropTable(
                name: "MagicLinkTokens");

            migrationBuilder.DropTable(
                name: "TripWaypoints");

            migrationBuilder.DropTable(
                name: "UserLicenses");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Convoys");
        }
    }
}
