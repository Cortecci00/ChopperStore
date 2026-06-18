using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChopperStoreAngularTest.Migrations
{
    /// <inheritdoc />
    public partial class AddStockSnapshotToTransactionItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactionItems_skins_SkinId",
                table: "transactionItems");

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "transactionItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SkinName",
                table: "transactionItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkinPhotoUrl",
                table: "transactionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ti
                SET ti.SkinName = s.name, ti.SkinPhotoUrl = s.PhotoUrl
                FROM transactionItems ti
                INNER JOIN skins s ON s.Id = ti.SkinId
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_transactionItems_skins_SkinId",
                table: "transactionItems",
                column: "SkinId",
                principalTable: "skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactionItems_skins_SkinId",
                table: "transactionItems");

            migrationBuilder.DropColumn(
                name: "SkinName",
                table: "transactionItems");

            migrationBuilder.DropColumn(
                name: "SkinPhotoUrl",
                table: "transactionItems");

            migrationBuilder.AlterColumn<int>(
                name: "SkinId",
                table: "transactionItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionItems_skins_SkinId",
                table: "transactionItems",
                column: "SkinId",
                principalTable: "skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
