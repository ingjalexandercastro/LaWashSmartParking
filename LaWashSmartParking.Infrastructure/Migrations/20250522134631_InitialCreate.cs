using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaWashSmartParking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ioTDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ioTDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSpots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedDeviceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSpots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParkingUsageLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParkingSpotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IoTDeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OccupiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FreedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingUsageLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_AssignedDeviceId",
                table: "ParkingSpots",
                column: "AssignedDeviceId",
                unique: true,
                filter: "[AssignedDeviceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpots_Location",
                table: "ParkingSpots",
                column: "Location",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ioTDevices");

            migrationBuilder.DropTable(
                name: "ParkingSpots");

            migrationBuilder.DropTable(
                name: "ParkingUsageLogs");
        }
    }
}
