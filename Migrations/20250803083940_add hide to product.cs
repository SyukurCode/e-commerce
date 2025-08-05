using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commers_Adelia.Migrations
{
    /// <inheritdoc />
    public partial class addhidetoproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isHide",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isHide",
                table: "Products");
        }
    }
}
