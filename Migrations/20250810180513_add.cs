using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commers_Adelia.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SellerPaymentMethods_UserId",
                table: "SellerPaymentMethods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerDeliveryOptions_userId",
                table: "SellerDeliveryOptions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCodes_UserId",
                table: "QrCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_onlineTransferNotes_UserId",
                table: "onlineTransferNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CodNotes_UserId",
                table: "CodNotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CodNotes_AspNetUsers_UserId",
                table: "CodNotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_onlineTransferNotes_AspNetUsers_UserId",
                table: "onlineTransferNotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QrCodes_AspNetUsers_UserId",
                table: "QrCodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerDeliveryOptions_AspNetUsers_userId",
                table: "SellerDeliveryOptions",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerPaymentMethods_AspNetUsers_UserId",
                table: "SellerPaymentMethods",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_CodNotes_AspNetUsers_UserId",
                table: "CodNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_onlineTransferNotes_AspNetUsers_UserId",
                table: "onlineTransferNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_QrCodes_AspNetUsers_UserId",
                table: "QrCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerDeliveryOptions_AspNetUsers_userId",
                table: "SellerDeliveryOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerPaymentMethods_AspNetUsers_UserId",
                table: "SellerPaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_SellerPaymentMethods_UserId",
                table: "SellerPaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_SellerDeliveryOptions_userId",
                table: "SellerDeliveryOptions");

            migrationBuilder.DropIndex(
                name: "IX_QrCodes_UserId",
                table: "QrCodes");

            migrationBuilder.DropIndex(
                name: "IX_onlineTransferNotes_UserId",
                table: "onlineTransferNotes");

            migrationBuilder.DropIndex(
                name: "IX_CodNotes_UserId",
                table: "CodNotes");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId",
                table: "Chats");
        }
    }
}
