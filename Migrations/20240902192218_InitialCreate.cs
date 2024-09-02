using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Make = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    Doors = table.Column<int>(type: "int", nullable: false),
                    TransmissionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerHp = table.Column<int>(type: "int", nullable: false),
                    RangeKm = table.Column<int>(type: "int", nullable: false),
                    AvailabilityStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}
