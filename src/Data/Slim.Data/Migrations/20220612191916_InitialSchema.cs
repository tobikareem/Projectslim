using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Slim.Data.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "slm");

            migrationBuilder.CreateTable(
                name: "PageImage",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageImageName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    ActualImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageImage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazorPage",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    URL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorPage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceAction",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceAction = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageSection",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazorPageId = table.Column<int>(type: "int", nullable: false),
                    PageSectionName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSection_PageId",
                        column: x => x.RazorPageId,
                        principalSchema: "slm",
                        principalTable: "RazorPage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RazorPageResourceActionMap",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazorPageId = table.Column<int>(type: "int", nullable: false),
                    ResourceActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazorPageResourceActionMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RazorPageResourceActionMap_PageId",
                        column: x => x.RazorPageId,
                        principalSchema: "slm",
                        principalTable: "RazorPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RazorPageResourceActionMap_ResourceActionId",
                        column: x => x.ResourceActionId,
                        principalSchema: "slm",
                        principalTable: "ResourceAction",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSectionImage",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazorPageId = table.Column<int>(type: "int", nullable: false),
                    RazorPageSectionId = table.Column<int>(type: "int", nullable: false),
                    PageImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionImage_PageId",
                        column: x => x.RazorPageId,
                        principalSchema: "slm",
                        principalTable: "RazorPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageSectionImage_PageImageId",
                        column: x => x.PageImageId,
                        principalSchema: "slm",
                        principalTable: "PageImage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageSectionImage_SectionId",
                        column: x => x.RazorPageSectionId,
                        principalSchema: "slm",
                        principalTable: "PageSection",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSectionResource",
                schema: "slm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazorPageId = table.Column<int>(type: "int", nullable: false),
                    RazorPageSectionId = table.Column<int>(type: "int", nullable: false),
                    ResourceActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionResource_PageId",
                        column: x => x.RazorPageId,
                        principalSchema: "slm",
                        principalTable: "RazorPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageSectionResource_ResourceActionId",
                        column: x => x.ResourceActionId,
                        principalSchema: "slm",
                        principalTable: "ResourceAction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageSectionResource_SectionId",
                        column: x => x.RazorPageSectionId,
                        principalSchema: "slm",
                        principalTable: "PageSection",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageSection_RazorPageId",
                schema: "slm",
                table: "PageSection",
                column: "RazorPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionImage_PageImageId",
                schema: "slm",
                table: "PageSectionImage",
                column: "PageImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionImage_RazorPageId",
                schema: "slm",
                table: "PageSectionImage",
                column: "RazorPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionImage_RazorPageSectionId",
                schema: "slm",
                table: "PageSectionImage",
                column: "RazorPageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionResource_RazorPageId",
                schema: "slm",
                table: "PageSectionResource",
                column: "RazorPageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionResource_RazorPageSectionId",
                schema: "slm",
                table: "PageSectionResource",
                column: "RazorPageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionResource_ResourceActionId",
                schema: "slm",
                table: "PageSectionResource",
                column: "ResourceActionId");

            migrationBuilder.CreateIndex(
                name: "NC_slim_PageResourceActionMap_PageID",
                schema: "slm",
                table: "RazorPageResourceActionMap",
                column: "RazorPageId");

            migrationBuilder.CreateIndex(
                name: "NC_slim_PageResourceActionMap_ResourceActionID",
                schema: "slm",
                table: "RazorPageResourceActionMap",
                column: "ResourceActionId");

            migrationBuilder.CreateIndex(
                name: "NC_slim_ResourceAction_Enabled",
                schema: "slm",
                table: "ResourceAction",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "NC_slim_ResourceAction_ResourceAction_Enabled",
                schema: "slm",
                table: "ResourceAction",
                columns: new[] { "ResourceAction", "Enabled" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageSectionImage",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "PageSectionResource",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "RazorPageResourceActionMap",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "PageImage",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "PageSection",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "ResourceAction",
                schema: "slm");

            migrationBuilder.DropTable(
                name: "RazorPage",
                schema: "slm");
        }
    }
}
