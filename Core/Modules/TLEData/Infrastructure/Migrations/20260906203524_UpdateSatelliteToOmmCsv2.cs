using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Modules.TLEData.Domain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSatelliteToOmmCsv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MEAN_MOTION_DDOT",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MEAN_MOTION_DDOT",
                table: "Satellites",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
