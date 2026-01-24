using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiChitra.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerSeatToMovieShow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerSeat",
                table: "MovieShows",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerSeat",
                table: "MovieShows");
        }
    }
}
