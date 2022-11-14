using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class CategoryPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pros",
                schema: "slm",
                table: "Review",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cons",
                schema: "slm",
                table: "Review",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("9ea14e66-acdd-4bf5-a545-89275e5948d3"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("b73ebc7a-d0ce-4f94-a05d-18f093af419e"));

            migrationBuilder.AddColumn<int>(
                name: "RazorPageId",
                schema: "slm",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Category_RazorPageId",
                schema: "slm",
                table: "Category",
                column: "RazorPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_RazorPage",
                schema: "slm",
                table: "Category",
                column: "RazorPageId",
                principalSchema: "slm",
                principalTable: "RazorPage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_RazorPage",
                schema: "slm",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_RazorPageId",
                schema: "slm",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "RazorPageId",
                schema: "slm",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "Pros",
                schema: "slm",
                table: "Review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cons",
                schema: "slm",
                table: "Review",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("b73ebc7a-d0ce-4f94-a05d-18f093af419e"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("9ea14e66-acdd-4bf5-a545-89275e5948d3"));
        }
    }
}
