using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChopperStoreAngularTest.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTransactionsAndCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_shoppingcarts_ShoppingCartId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_items_skins_skinId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_shoppingcarts_users_userId",
                table: "shoppingcarts");

            migrationBuilder.DropForeignKey(
                name: "FK_skins_categories_categoryId",
                table: "skins");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_items_itemsId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_userId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_itemsId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_shoppingcarts_userId",
                table: "shoppingcarts");

            migrationBuilder.DropColumn(
                name: "itemsId",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "transactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_userId",
                table: "transactions",
                newName: "IX_transactions_UserId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "shoppingcarts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "skinId",
                table: "items",
                newName: "SkinId");

            migrationBuilder.RenameIndex(
                name: "IX_items_skinId",
                table: "items",
                newName: "IX_items_SkinId");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "lastname",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GoogleId",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "transactionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    SkinId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPriceAtPurchase = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transactionItems_skins_SkinId",
                        column: x => x.SkinId,
                        principalTable: "skins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactionItems_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shoppingcarts_UserId",
                table: "shoppingcarts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactionItems_SkinId",
                table: "transactionItems",
                column: "SkinId");

            migrationBuilder.CreateIndex(
                name: "IX_transactionItems_TransactionId",
                table: "transactionItems",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_shoppingcarts_ShoppingCartId",
                table: "items",
                column: "ShoppingCartId",
                principalTable: "shoppingcarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_skins_SkinId",
                table: "items",
                column: "SkinId",
                principalTable: "skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingcarts_users_UserId",
                table: "shoppingcarts",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skins_categories_categoryId",
                table: "skins",
                column: "categoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_shoppingcarts_ShoppingCartId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_items_skins_SkinId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_shoppingcarts_users_UserId",
                table: "shoppingcarts");

            migrationBuilder.DropForeignKey(
                name: "FK_skins_categories_categoryId",
                table: "skins");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "transactionItems");

            migrationBuilder.DropIndex(
                name: "IX_shoppingcarts_UserId",
                table: "shoppingcarts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "transactions",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_UserId",
                table: "transactions",
                newName: "IX_transactions_userId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "shoppingcarts",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "SkinId",
                table: "items",
                newName: "skinId");

            migrationBuilder.RenameIndex(
                name: "IX_items_SkinId",
                table: "items",
                newName: "IX_items_skinId");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lastname",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GoogleId",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "itemsId",
                table: "transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ShoppingCartId",
                table: "items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_itemsId",
                table: "transactions",
                column: "itemsId");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingcarts_userId",
                table: "shoppingcarts",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_shoppingcarts_ShoppingCartId",
                table: "items",
                column: "ShoppingCartId",
                principalTable: "shoppingcarts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_items_skins_skinId",
                table: "items",
                column: "skinId",
                principalTable: "skins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingcarts_users_userId",
                table: "shoppingcarts",
                column: "userId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skins_categories_categoryId",
                table: "skins",
                column: "categoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_items_itemsId",
                table: "transactions",
                column: "itemsId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_userId",
                table: "transactions",
                column: "userId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
