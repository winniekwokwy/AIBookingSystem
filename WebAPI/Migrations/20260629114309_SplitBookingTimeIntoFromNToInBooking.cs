using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SplitBookingTimeIntoFromNToInBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingTime",
                table: "Bookings",
                newName: "BookingTo");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookingFrom",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingFrom",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "BookingTo",
                table: "Bookings",
                newName: "BookingTime");
        }
    }
}
