using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedUsernameInLogsNBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_UserId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "ChangedBy",
                table: "ChangeLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookedBy",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccessedBy",
                table: "AccessLogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "ChangeLogs");

            migrationBuilder.DropColumn(
                name: "BookedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AccessedBy",
                table: "AccessLogs");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_UserId",
                table: "Rooms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_UserId",
                table: "Rooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
