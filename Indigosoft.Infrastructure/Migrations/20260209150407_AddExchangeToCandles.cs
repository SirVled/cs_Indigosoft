using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Indigosoft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeToCandles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Exchange",
                table: "Candles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "Candles");
        }
    }
}
