using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiChitra.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatNumToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatNumbers",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatNumbers",
                table: "Tickets");
        }
    }
}
