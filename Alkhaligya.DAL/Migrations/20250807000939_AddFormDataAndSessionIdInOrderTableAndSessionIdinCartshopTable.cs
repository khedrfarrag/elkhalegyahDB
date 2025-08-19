using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alkhaligya.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFormDataAndSessionIdInOrderTableAndSessionIdinCartshopTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ApplicationUser_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CartShops_UserId",
                table: "CartShops");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orders",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsGuestOrder",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CartShops",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "CartShops",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartShops_UserId",
                table: "CartShops",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ApplicationUser_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ApplicationUser_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CartShops_UserId",
                table: "CartShops");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsGuestOrder",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "CartShops");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CartShops",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartShops_UserId",
                table: "CartShops",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ApplicationUser_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
