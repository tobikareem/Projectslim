using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class AddOnlyBagSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JewelrySize",
                schema: "slm",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "ShoeSize",
                schema: "slm",
                table: "ShoppingCart");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("6a88b505-f7ee-4cb3-9116-76387c3abfd7"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("002a88c4-2a4c-44df-b5b8-22ed8dca6161"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JewelrySize",
                schema: "slm",
                table: "ShoppingCart",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShoeSize",
                schema: "slm",
                table: "ShoppingCart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("002a88c4-2a4c-44df-b5b8-22ed8dca6161"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("6a88b505-f7ee-4cb3-9116-76387c3abfd7"));
        }
    }
}
