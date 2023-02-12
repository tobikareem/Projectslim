using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class AddProductSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductDetail",
                schema: "slm",
                table: "ShoppingCart",
                newName: "JewelrySize");

            migrationBuilder.AddColumn<string>(
                name: "BagSize",
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
                defaultValue: new Guid("eeaedaed-8dcf-4fe3-b566-f252016ad001"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("d00c8683-2189-4865-8793-e9314d8bbd4e"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BagSize",
                schema: "slm",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "ShoeSize",
                schema: "slm",
                table: "ShoppingCart");

            migrationBuilder.RenameColumn(
                name: "JewelrySize",
                schema: "slm",
                table: "ShoppingCart",
                newName: "ProductDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("d00c8683-2189-4865-8793-e9314d8bbd4e"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("eeaedaed-8dcf-4fe3-b566-f252016ad001"));
        }
    }
}
