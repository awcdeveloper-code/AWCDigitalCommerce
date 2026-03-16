using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWC.DigitalCommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class RenamecolumnTypewithClassinSeatsentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Seats",
                newName: "Class");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Class",
                table: "Seats",
                newName: "Type");
        }
    }
}
