using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Modules.TLEData.Domain.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSatelliteToOmmCsv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TLELine1",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "TLELine2",
                table: "Satellites");

            migrationBuilder.RenameColumn(
                name: "Epoch",
                table: "Satellites",
                newName: "EPOCH");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Satellites",
                newName: "OBJECT_NAME");

            migrationBuilder.RenameColumn(
                name: "NoradId",
                table: "Satellites",
                newName: "NORAD_CAT_ID");

            migrationBuilder.AddColumn<double>(
                name: "ARG_OF_PERICENTER",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BSTAR",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CLASSIFICATION_TYPE",
                table: "Satellites",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ECCENTRICITY",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ELEMENT_SET_NO",
                table: "Satellites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EPHEMERIS_TYPE",
                table: "Satellites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "INCLINATION",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MEAN_ANOMALY",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MEAN_MOTION",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MEAN_MOTION_DDOT",
                table: "Satellites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MEAN_MOTION_DOT",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OBJECT_ID",
                table: "Satellites",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RA_OF_ASC_NODE",
                table: "Satellites",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "REV_AT_EPOCH",
                table: "Satellites",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARG_OF_PERICENTER",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "BSTAR",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "CLASSIFICATION_TYPE",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "ECCENTRICITY",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "ELEMENT_SET_NO",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "EPHEMERIS_TYPE",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "INCLINATION",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "MEAN_ANOMALY",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "MEAN_MOTION",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "MEAN_MOTION_DDOT",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "MEAN_MOTION_DOT",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "OBJECT_ID",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "RA_OF_ASC_NODE",
                table: "Satellites");

            migrationBuilder.DropColumn(
                name: "REV_AT_EPOCH",
                table: "Satellites");

            migrationBuilder.RenameColumn(
                name: "EPOCH",
                table: "Satellites",
                newName: "Epoch");

            migrationBuilder.RenameColumn(
                name: "OBJECT_NAME",
                table: "Satellites",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NORAD_CAT_ID",
                table: "Satellites",
                newName: "NoradId");

            migrationBuilder.AddColumn<string>(
                name: "TLELine1",
                table: "Satellites",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TLELine2",
                table: "Satellites",
                type: "character varying(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");
        }
    }
}
