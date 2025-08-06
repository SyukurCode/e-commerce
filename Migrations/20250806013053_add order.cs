using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace E_Commers_Adelia.Migrations
{
    /// <inheritdoc />
    public partial class addorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Orders",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                   OrderNo = table.Column<string>(nullable: false),
                   CustomerId = table.Column<string>(nullable: false),
                   ProductId = table.Column<int>(nullable: false),
                   SelectedOption = table.Column<string>(nullable: false),
                   Quantity = table.Column<int>(nullable: false),
                   UnitPrice = table.Column<decimal>(nullable: false),
                   SubTotalPrice = table.Column<decimal>(nullable: false),
                   PlaceDateTime = table.Column<DateTime>(nullable: false),
                   StatusId = table.Column<int>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Orders", x => x.Id);
               });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "Orders");
        }
    }
}
