using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSiloAppFSD.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddcontactForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "WishlistId",
                table: "WishlistItems",
                newName: "WishlistID");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "WishlistItems",
                newName: "ProductID");

            migrationBuilder.RenameColumn(
                name: "WishlistItemId",
                table: "WishlistItems",
                newName: "WishlistItemID");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistID");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "WishlistItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductID",
                table: "WishlistItems",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Products_ProductID",
                table: "WishlistItems",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistID",
                table: "WishlistItems",
                column: "WishlistID",
                principalTable: "Wishlists",
                principalColumn: "WishListID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Products_ProductID",
                table: "WishlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistID",
                table: "WishlistItems");

            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_ProductID",
                table: "WishlistItems");

            migrationBuilder.RenameColumn(
                name: "WishlistID",
                table: "WishlistItems",
                newName: "WishlistId");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "WishlistItems",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "WishlistItemID",
                table: "WishlistItems",
                newName: "WishlistItemId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistID",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "WishlistItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "WishListID");
        }
    }
}
