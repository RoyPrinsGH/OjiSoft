using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OjiSoftPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergeUserAndProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OjiProfiles");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NicknameColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileMainColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileSecondaryColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NicknameColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileMainColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileSecondaryColor",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "OjiProfiles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OjiProfiles", x => new { x.ID, x.UserID });
                    table.ForeignKey(
                        name: "FK_OjiProfiles_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OjiProfiles_UserID",
                table: "OjiProfiles",
                column: "UserID",
                unique: true);
        }
    }
}
