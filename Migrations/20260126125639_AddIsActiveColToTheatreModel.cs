using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiChitra.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColToTheatreModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Theatres",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Theatres");
        }
    }
}
