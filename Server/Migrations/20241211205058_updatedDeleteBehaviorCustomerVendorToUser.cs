using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class updatedDeleteBehaviorCustomerVendorToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_AspNetUsers_Id",
                table: "Vendors");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_AspNetUsers_Id",
                table: "Vendors",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_AspNetUsers_Id",
                table: "Vendors");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_AspNetUsers_Id",
                table: "Vendors",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
