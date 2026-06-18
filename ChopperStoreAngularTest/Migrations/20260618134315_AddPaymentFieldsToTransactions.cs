using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChopperStoreAngularTest.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPaymentId",
                table: "transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPreferenceId",
                table: "transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MercadoPagoPaymentId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "MercadoPagoPreferenceId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "transactions");
        }
    }
}
