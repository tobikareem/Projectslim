using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class AddUserPageImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("3c4c924d-73fa-466c-83b5-a039c6154c68"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("2b1a1b4f-2d37-49a7-8fcd-029e016d7189"));

            migrationBuilder.CreateTable(
                name: "UserPageImages",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPageImages", x => x.ImageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPageImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImageId",
                schema: "slm",
                table: "Image",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("2b1a1b4f-2d37-49a7-8fcd-029e016d7189"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("3c4c924d-73fa-466c-83b5-a039c6154c68"));
        }
    }
}
