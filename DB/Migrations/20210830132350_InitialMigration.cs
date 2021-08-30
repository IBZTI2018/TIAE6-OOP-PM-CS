using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DB.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "municipalities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipalities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Rule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rule", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "taxDeclarationAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxDeclarationAttributes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "streets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    postalCode = table.Column<int>(type: "int", nullable: false),
                    municipalityId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streets", x => x.id);
                    table.ForeignKey(
                        name: "FK_streets_municipalities_municipalityId",
                        column: x => x.municipalityId,
                        principalTable: "municipalities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    streetId = table.Column<int>(type: "int", nullable: false),
                    streetNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.id);
                    table.ForeignKey(
                        name: "FK_persons_streets_streetId",
                        column: x => x.streetId,
                        principalTable: "streets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "taxDeclarations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personId = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    isApproved = table.Column<bool>(type: "bit", nullable: false),
                    isSent = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxDeclarations", x => x.id);
                    table.ForeignKey(
                        name: "FK_taxDeclarations_persons_personId",
                        column: x => x.personId,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "taxDeclarationEntries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    taxDeclarationId = table.Column<int>(type: "int", nullable: false),
                    taxDeclarationAttributeId = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createdByRuleId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxDeclarationEntries", x => x.id);
                    table.ForeignKey(
                        name: "FK_taxDeclarationEntries_Rule_createdByRuleId",
                        column: x => x.createdByRuleId,
                        principalTable: "Rule",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_taxDeclarationEntries_taxDeclarationAttributes_taxDeclarationAttributeId",
                        column: x => x.taxDeclarationAttributeId,
                        principalTable: "taxDeclarationAttributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_taxDeclarationEntries_taxDeclarations_taxDeclarationId",
                        column: x => x.taxDeclarationId,
                        principalTable: "taxDeclarations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "municipalities",
                columns: new[] { "id", "createdAt", "modifiedAt", "name" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sitten" });

            migrationBuilder.InsertData(
                table: "streets",
                columns: new[] { "id", "createdAt", "modifiedAt", "municipalityId", "name", "postalCode" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Bahnhofstrasse", 1950 });

            migrationBuilder.InsertData(
                table: "persons",
                columns: new[] { "id", "createdAt", "firstName", "lastName", "modifiedAt", "streetId", "streetNumber" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "André", "Glatzl", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "1A" });

            migrationBuilder.InsertData(
                table: "persons",
                columns: new[] { "id", "createdAt", "firstName", "lastName", "modifiedAt", "streetId", "streetNumber" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sven", "Gehring", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "1B" });

            migrationBuilder.CreateIndex(
                name: "IX_persons_streetId",
                table: "persons",
                column: "streetId");

            migrationBuilder.CreateIndex(
                name: "IX_streets_municipalityId",
                table: "streets",
                column: "municipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_taxDeclarationEntries_createdByRuleId",
                table: "taxDeclarationEntries",
                column: "createdByRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_taxDeclarationEntries_taxDeclarationAttributeId",
                table: "taxDeclarationEntries",
                column: "taxDeclarationAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_taxDeclarationEntries_taxDeclarationId",
                table: "taxDeclarationEntries",
                column: "taxDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_taxDeclarations_personId",
                table: "taxDeclarations",
                column: "personId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taxDeclarationEntries");

            migrationBuilder.DropTable(
                name: "Rule");

            migrationBuilder.DropTable(
                name: "taxDeclarationAttributes");

            migrationBuilder.DropTable(
                name: "taxDeclarations");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "streets");

            migrationBuilder.DropTable(
                name: "municipalities");
        }
    }
}
