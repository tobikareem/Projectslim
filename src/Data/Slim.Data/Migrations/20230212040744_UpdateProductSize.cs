using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class UpdateProductSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("002a88c4-2a4c-44df-b5b8-22ed8dca6161"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("eeaedaed-8dcf-4fe3-b566-f252016ad001"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("eeaedaed-8dcf-4fe3-b566-f252016ad001"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("002a88c4-2a4c-44df-b5b8-22ed8dca6161"));
        }
    }
}
