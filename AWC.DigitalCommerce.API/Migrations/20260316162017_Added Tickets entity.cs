using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWC.DigitalCommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedTicketsentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Taxes = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ServiceFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Card = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Transfer = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Voucher = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SeatAKA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
