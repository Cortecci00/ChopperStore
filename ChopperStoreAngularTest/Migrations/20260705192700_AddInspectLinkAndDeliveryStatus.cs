using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChopperStoreAngularTest.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectLinkAndDeliveryStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatus",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InspectLink",
                table: "skins",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "InspectLink",
                table: "skins");
        }
    }
}
