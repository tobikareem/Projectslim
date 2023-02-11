using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class AddGenderAndSizeForProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "slm",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HasMaxi",
                schema: "slm",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMidi",
                schema: "slm",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMini",
                schema: "slm",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("289c1a99-5709-4ab2-96f5-ba2937cd51f8"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("482faa8c-78fb-4ff8-9b66-5a974b483238"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "slm",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "HasMaxi",
                schema: "slm",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "HasMidi",
                schema: "slm",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "HasMini",
                schema: "slm",
                table: "Product");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("482faa8c-78fb-4ff8-9b66-5a974b483238"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("289c1a99-5709-4ab2-96f5-ba2937cd51f8"));
        }
    }
}
