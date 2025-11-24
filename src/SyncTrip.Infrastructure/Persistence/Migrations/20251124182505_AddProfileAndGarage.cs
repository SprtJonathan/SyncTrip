using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SyncTrip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAndGarage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
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

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://logo.clearbit.com/yamaha-motor.com", "Yamaha" },
                    { 2, "https://logo.clearbit.com/honda.com", "Honda" },
                    { 3, "https://logo.clearbit.com/kawasaki.com", "Kawasaki" },
                    { 4, "https://logo.clearbit.com/suzuki.com", "Suzuki" },
                    { 5, "https://logo.clearbit.com/ducati.com", "Ducati" },
                    { 6, "https://logo.clearbit.com/bmw-motorrad.com", "BMW Motorrad" },
                    { 7, "https://logo.clearbit.com/harley-davidson.com", "Harley-Davidson" },
                    { 8, "https://logo.clearbit.com/ktm.com", "KTM" },
                    { 9, "https://logo.clearbit.com/triumph.co.uk", "Triumph" },
                    { 10, "https://logo.clearbit.com/aprilia.com", "Aprilia" },
                    { 11, "https://logo.clearbit.com/renault.com", "Renault" },
                    { 12, "https://logo.clearbit.com/peugeot.com", "Peugeot" },
                    { 13, "https://logo.clearbit.com/citroen.com", "Citroën" },
                    { 14, "https://logo.clearbit.com/dsautomobiles.com", "DS Automobiles" },
                    { 15, "https://logo.clearbit.com/volkswagen.com", "Volkswagen" },
                    { 16, "https://logo.clearbit.com/mercedes-benz.com", "Mercedes-Benz" },
                    { 17, "https://logo.clearbit.com/audi.com", "Audi" },
                    { 18, "https://logo.clearbit.com/bmw.com", "BMW" },
                    { 19, "https://logo.clearbit.com/porsche.com", "Porsche" },
                    { 20, "https://logo.clearbit.com/opel.com", "Opel" },
                    { 21, "https://logo.clearbit.com/toyota.com", "Toyota" },
                    { 22, "https://logo.clearbit.com/nissan.com", "Nissan" },
                    { 23, "https://logo.clearbit.com/mazda.com", "Mazda" },
                    { 24, "https://logo.clearbit.com/mitsubishicars.com", "Mitsubishi" },
                    { 25, "https://logo.clearbit.com/subaru.com", "Subaru" },
                    { 26, "https://logo.clearbit.com/lexus.com", "Lexus" },
                    { 27, "https://logo.clearbit.com/ford.com", "Ford" },
                    { 28, "https://logo.clearbit.com/chevrolet.com", "Chevrolet" },
                    { 29, "https://logo.clearbit.com/tesla.com", "Tesla" },
                    { 30, "https://logo.clearbit.com/jeep.com", "Jeep" },
                    { 31, "https://logo.clearbit.com/fiat.com", "Fiat" },
                    { 32, "https://logo.clearbit.com/alfaromeo.com", "Alfa Romeo" },
                    { 33, "https://logo.clearbit.com/lancia.com", "Lancia" },
                    { 34, "https://logo.clearbit.com/hyundai.com", "Hyundai" },
                    { 35, "https://logo.clearbit.com/kia.com", "Kia" },
                    { 36, "https://logo.clearbit.com/landrover.com", "Land Rover" },
                    { 37, "https://logo.clearbit.com/mini.com", "Mini" },
                    { 38, "https://logo.clearbit.com/iveco.com", "Iveco" },
                    { 39, "https://logo.clearbit.com/man.eu", "Man" },
                    { 40, "https://logo.clearbit.com/dacia.com", "Dacia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLicenses_UserId",
                table: "UserLicenses",
                column: "UserId");

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
                name: "UserLicenses");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
