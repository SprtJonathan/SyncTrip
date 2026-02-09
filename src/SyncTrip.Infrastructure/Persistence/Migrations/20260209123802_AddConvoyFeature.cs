using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SyncTrip.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConvoyFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Brands",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                column: "LogoUrl",
                value: "https://example.com/logos/yamaha.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                column: "LogoUrl",
                value: "https://example.com/logos/honda.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3,
                column: "LogoUrl",
                value: "https://example.com/logos/kawasaki.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4,
                column: "LogoUrl",
                value: "https://example.com/logos/suzuki.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/bmw-motorrad.png", "BMW Motorrad" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/ducati.png", "Ducati" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/ktm.png", "KTM" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/harley-davidson.png", "Harley-Davidson" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 9,
                column: "LogoUrl",
                value: "https://example.com/logos/triumph.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10,
                column: "LogoUrl",
                value: "https://example.com/logos/aprilia.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 11,
                column: "LogoUrl",
                value: "https://example.com/logos/renault.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 12,
                column: "LogoUrl",
                value: "https://example.com/logos/peugeot.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 13,
                column: "LogoUrl",
                value: "https://example.com/logos/citroen.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 14,
                column: "LogoUrl",
                value: "https://example.com/logos/ds.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/alpine.png", "Alpine" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/bmw.png", "BMW" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/mercedes.png", "Mercedes-Benz" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/audi.png", "Audi" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/volkswagen.png", "Volkswagen" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/porsche.png", "Porsche" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/ferrari.png", "Ferrari" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/lamborghini.png", "Lamborghini" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/fiat.png", "Fiat" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/alfa-romeo.png", "Alfa Romeo" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/toyota.png", "Toyota" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/nissan.png", "Nissan" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/mazda.png", "Mazda" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/subaru.png", "Subaru" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/mitsubishi.png", "Mitsubishi" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/ford.png", "Ford" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/chevrolet.png", "Chevrolet" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/tesla.png", "Tesla" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/jeep.png", "Jeep" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 34,
                column: "LogoUrl",
                value: "https://example.com/logos/hyundai.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 35,
                column: "LogoUrl",
                value: "https://example.com/logos/kia.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/fiat-professional.png", "Fiat Professional" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/iveco.png", "Iveco" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/man.png", "MAN" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/scania.png", "Scania" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://example.com/logos/volvo-trucks.png", "Volvo Trucks" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvoyMembers");

            migrationBuilder.DropTable(
                name: "Convoys");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Brands",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/yamaha-motor.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/honda.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/kawasaki.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/suzuki.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/ducati.com", "Ducati" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/bmw-motorrad.com", "BMW Motorrad" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/harley-davidson.com", "Harley-Davidson" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/ktm.com", "KTM" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 9,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/triumph.co.uk");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/aprilia.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 11,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/renault.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 12,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/peugeot.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 13,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/citroen.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 14,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/dsautomobiles.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/volkswagen.com", "Volkswagen" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/mercedes-benz.com", "Mercedes-Benz" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/audi.com", "Audi" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/bmw.com", "BMW" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/porsche.com", "Porsche" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/opel.com", "Opel" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/toyota.com", "Toyota" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/nissan.com", "Nissan" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/mazda.com", "Mazda" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/mitsubishicars.com", "Mitsubishi" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/subaru.com", "Subaru" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/lexus.com", "Lexus" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/ford.com", "Ford" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/chevrolet.com", "Chevrolet" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/tesla.com", "Tesla" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/jeep.com", "Jeep" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/fiat.com", "Fiat" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/alfaromeo.com", "Alfa Romeo" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/lancia.com", "Lancia" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 34,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/hyundai.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 35,
                column: "LogoUrl",
                value: "https://logo.clearbit.com/kia.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/landrover.com", "Land Rover" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/mini.com", "Mini" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/iveco.com", "Iveco" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/man.eu", "Man" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "LogoUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/dacia.com", "Dacia" });
        }
    }
}
