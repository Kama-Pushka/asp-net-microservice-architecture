using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructured.Migrations
{
    /// <inheritdoc />
    public partial class userInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Posts",
                newName: "UserInfo_UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserInfo_Name",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserInfo_Name",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "UserInfo_UserId",
                table: "Posts",
                newName: "UserId");
        }
    }
}
