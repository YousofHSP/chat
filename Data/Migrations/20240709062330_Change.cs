using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_OwnerUserId",
                table: "ChatRooms",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerUserId",
                table: "ChatRooms",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_AspNetUsers_OwnerUserId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_OwnerUserId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChatRooms");
        }
    }
}
