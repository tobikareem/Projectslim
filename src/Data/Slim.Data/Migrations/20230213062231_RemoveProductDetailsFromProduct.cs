using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class RemoveProductDetailsFromProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                defaultValue: new Guid("8ba5af9a-dfec-4c76-8526-35910d4b7120"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("47da4518-e911-48e7-aa92-deba6a0c29d0"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                defaultValue: new Guid("47da4518-e911-48e7-aa92-deba6a0c29d0"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("8ba5af9a-dfec-4c76-8526-35910d4b7120"));
        }
    }
}
