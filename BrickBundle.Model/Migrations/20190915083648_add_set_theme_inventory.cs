using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrickBundle.Model.Migrations
{
    public partial class add_set_theme_inventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ParentID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Themes_Themes_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Themes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SetNum = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    ThemeID = table.Column<long>(nullable: false),
                    NumParts = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sets_Themes_ThemeID",
                        column: x => x.ThemeID,
                        principalTable: "Themes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<long>(nullable: false),
                    SetID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Inventories_Sets_SetID",
                        column: x => x.SetID,
                        principalTable: "Sets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryParts",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryID = table.Column<long>(nullable: false),
                    PartID = table.Column<long>(nullable: false),
                    ColorID = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    IsSpare = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryParts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryParts_LegoColors_ColorID",
                        column: x => x.ColorID,
                        principalTable: "LegoColors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryParts_Inventories_InventoryID",
                        column: x => x.InventoryID,
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryParts_Parts_PartID",
                        column: x => x.PartID,
                        principalTable: "Parts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventorySets",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryID = table.Column<long>(nullable: false),
                    SetID = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventorySets_Inventories_InventoryID",
                        column: x => x.InventoryID,
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventorySets_Sets_SetID",
                        column: x => x.SetID,
                        principalTable: "Sets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserParts_Quantity",
                table: "UserParts",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_UserParts_UserID",
                table: "UserParts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_SetID",
                table: "Inventories",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Version_SetID",
                table: "Inventories",
                columns: new[] { "Version", "SetID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryParts_ColorID",
                table: "InventoryParts",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryParts_InventoryID",
                table: "InventoryParts",
                column: "InventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryParts_PartID",
                table: "InventoryParts",
                column: "PartID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryParts_InventoryID_PartID_ColorID_IsSpare",
                table: "InventoryParts",
                columns: new[] { "InventoryID", "PartID", "ColorID", "IsSpare" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventorySets_InventoryID",
                table: "InventorySets",
                column: "InventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySets_SetID",
                table: "InventorySets",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySets_InventoryID_SetID",
                table: "InventorySets",
                columns: new[] { "InventoryID", "SetID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sets_SetNum",
                table: "Sets",
                column: "SetNum",
                unique: true,
                filter: "[SetNum] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_ThemeID",
                table: "Sets",
                column: "ThemeID");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_Name",
                table: "Themes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_ParentID",
                table: "Themes",
                column: "ParentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryParts");

            migrationBuilder.DropTable(
                name: "InventorySets");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropIndex(
                name: "IX_UserParts_Quantity",
                table: "UserParts");

            migrationBuilder.DropIndex(
                name: "IX_UserParts_UserID",
                table: "UserParts");
        }
    }
}
