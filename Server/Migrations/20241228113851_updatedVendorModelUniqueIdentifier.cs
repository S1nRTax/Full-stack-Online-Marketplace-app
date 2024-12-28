using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class updatedVendorModelUniqueIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Vendors_VendorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "VendorId",
                table: "Posts",
                newName: "ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_VendorId",
                table: "Posts",
                newName: "IX_Posts_ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Vendors_ShopId",
                table: "Posts",
                column: "ShopId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Vendors_ShopId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                table: "Posts",
                newName: "VendorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ShopId",
                table: "Posts",
                newName: "IX_Posts_VendorId");

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Vendors_VendorId",
                table: "Posts",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
