using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commers_Adelia.Migrations
{
    /// <inheritdoc />
    public partial class fixquantityspeel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quntity",
                table: "Orders",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Orders",
                newName: "Quntity");
        }
    }
}
