using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoomNBookingIdInChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeLogs_Users_UserId",
                table: "ChangeLogs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChangeLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeLogs_Users_UserId",
                table: "ChangeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeLogs_Users_UserId",
                table: "ChangeLogs");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ChangeLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeLogs_Users_UserId",
                table: "ChangeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
