using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservasXYZ.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRateGuestPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_RoomId_SeasonId",
                table: "Rates");

            migrationBuilder.AddColumn<int>(
                name: "BaseGuests",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraPersonPrice",
                table: "Rates",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Rates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_RoomId_SeasonId_Kind",
                table: "Rates",
                columns: new[] { "RoomId", "SeasonId", "Kind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_RoomId_SeasonId_Kind",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "BaseGuests",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "ExtraPersonPrice",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Rates");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_RoomId_SeasonId",
                table: "Rates",
                columns: new[] { "RoomId", "SeasonId" },
                unique: true);
        }
    }
}
