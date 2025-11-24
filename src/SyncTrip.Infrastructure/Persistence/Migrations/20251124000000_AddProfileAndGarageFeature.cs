using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SyncTrip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAndGarageFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Créer la table Brands
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

            // Créer la table Vehicles
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

            // Créer la table UserLicenses
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

            // Index sur Brands.Name (unique)
            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            // Index sur Vehicles
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BrandId",
                table: "Vehicles",
                column: "BrandId");

            // Index sur UserLicenses
            migrationBuilder.CreateIndex(
                name: "IX_UserLicenses_UserId",
                table: "UserLicenses",
                column: "UserId");

            // Insérer les données seed pour les marques
            InsertBrandsSeed(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserLicenses");
            migrationBuilder.DropTable(name: "Vehicles");
            migrationBuilder.DropTable(name: "Brands");
        }

        private void InsertBrandsSeed(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name", "LogoUrl" },
                values: new object[,]
                {
                    // Marques de motos
                    { 1, "Yamaha", "https://example.com/logos/yamaha.png" },
                    { 2, "Honda", "https://example.com/logos/honda.png" },
                    { 3, "Kawasaki", "https://example.com/logos/kawasaki.png" },
                    { 4, "Suzuki", "https://example.com/logos/suzuki.png" },
                    { 5, "BMW Motorrad", "https://example.com/logos/bmw-motorrad.png" },
                    { 6, "Ducati", "https://example.com/logos/ducati.png" },
                    { 7, "KTM", "https://example.com/logos/ktm.png" },
                    { 8, "Harley-Davidson", "https://example.com/logos/harley-davidson.png" },
                    { 9, "Triumph", "https://example.com/logos/triumph.png" },
                    { 10, "Aprilia", "https://example.com/logos/aprilia.png" },
                    // Marques de voitures françaises
                    { 11, "Renault", "https://example.com/logos/renault.png" },
                    { 12, "Peugeot", "https://example.com/logos/peugeot.png" },
                    { 13, "Citroën", "https://example.com/logos/citroen.png" },
                    { 14, "DS Automobiles", "https://example.com/logos/ds.png" },
                    { 15, "Alpine", "https://example.com/logos/alpine.png" },
                    // Marques de voitures allemandes
                    { 16, "BMW", "https://example.com/logos/bmw.png" },
                    { 17, "Mercedes-Benz", "https://example.com/logos/mercedes.png" },
                    { 18, "Audi", "https://example.com/logos/audi.png" },
                    { 19, "Volkswagen", "https://example.com/logos/volkswagen.png" },
                    { 20, "Porsche", "https://example.com/logos/porsche.png" },
                    // Marques de voitures italiennes
                    { 21, "Ferrari", "https://example.com/logos/ferrari.png" },
                    { 22, "Lamborghini", "https://example.com/logos/lamborghini.png" },
                    { 23, "Fiat", "https://example.com/logos/fiat.png" },
                    { 24, "Alfa Romeo", "https://example.com/logos/alfa-romeo.png" },
                    // Marques de voitures japonaises
                    { 25, "Toyota", "https://example.com/logos/toyota.png" },
                    { 26, "Nissan", "https://example.com/logos/nissan.png" },
                    { 27, "Mazda", "https://example.com/logos/mazda.png" },
                    { 28, "Subaru", "https://example.com/logos/subaru.png" },
                    { 29, "Mitsubishi", "https://example.com/logos/mitsubishi.png" },
                    // Marques de voitures américaines
                    { 30, "Ford", "https://example.com/logos/ford.png" },
                    { 31, "Chevrolet", "https://example.com/logos/chevrolet.png" },
                    { 32, "Tesla", "https://example.com/logos/tesla.png" },
                    { 33, "Jeep", "https://example.com/logos/jeep.png" },
                    // Marques de voitures coréennes
                    { 34, "Hyundai", "https://example.com/logos/hyundai.png" },
                    { 35, "Kia", "https://example.com/logos/kia.png" },
                    // Marques de véhicules utilitaires / camping-cars
                    { 36, "Fiat Professional", "https://example.com/logos/fiat-professional.png" },
                    { 37, "Iveco", "https://example.com/logos/iveco.png" },
                    { 38, "MAN", "https://example.com/logos/man.png" },
                    { 39, "Scania", "https://example.com/logos/scania.png" },
                    { 40, "Volvo Trucks", "https://example.com/logos/volvo-trucks.png" }
                });
        }
    }
}
