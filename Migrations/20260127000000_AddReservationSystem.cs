using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiChitra.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpiry",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE Tickets SET Status = 'Reserved' WHERE Status = 'Booked'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationExpiry",
                table: "Tickets");

            migrationBuilder.Sql("UPDATE Tickets SET Status = 'Booked' WHERE Status = 'Reserved'");
        }
    }
}