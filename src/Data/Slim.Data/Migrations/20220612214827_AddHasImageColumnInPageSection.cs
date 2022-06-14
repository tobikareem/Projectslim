using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class AddHasImageColumnInPageSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                schema: "slm",
                table: "PageSection",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))");

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
