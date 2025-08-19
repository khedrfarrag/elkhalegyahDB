using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alkhaligya.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminAndIsConfirmAdminAttrbuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "ApplicationUser");
        }
    }
}
