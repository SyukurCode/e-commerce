using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commers_Adelia.Migrations
{
    /// <inheritdoc />
    public partial class changeisDisabletoisEnable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDisable",
                table: "SellerPaymentMethods",
                newName: "isEnable");

            migrationBuilder.RenameColumn(
                name: "isDisable",
                table: "SellerDeliveryOptions",
                newName: "isEnable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isEnable",
                table: "SellerPaymentMethods",
                newName: "isDisable");

            migrationBuilder.RenameColumn(
                name: "isEnable",
                table: "SellerDeliveryOptions",
                newName: "isDisable");
        }
    }
}
