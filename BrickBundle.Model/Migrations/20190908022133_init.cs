using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BrickBundle.Model.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LegoColors",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsTransparent = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RGB = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegoColors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    ResetPasswordAttempts = table.Column<int>(nullable: false),
                    ResetPasswordCodesSent = table.Column<int>(nullable: false),
                    ResetPasswordCode = table.Column<string>(nullable: true),
                    ResetPasswordCodeCreated = table.Column<DateTime>(nullable: true),
                    IsEmailVerified = table.Column<bool>(nullable: false),
                    EmailVerificationCode = table.Column<string>(nullable: true),
                    EmailVerificationCodeCreated = table.Column<DateTime>(nullable: true),
                    Password = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Parts_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserParts",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<long>(nullable: false),
                    PartID = table.Column<long>(nullable: false),
                    ColorID = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserParts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserParts_LegoColors_ColorID",
                        column: x => x.ColorID,
                        principalTable: "LegoColors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserParts_Parts_PartID",
                        column: x => x.PartID,
                        principalTable: "Parts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserParts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LegoColors_Name",
                table: "LegoColors",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_CategoryID",
                table: "Parts",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Code",
                table: "Parts",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Name",
                table: "Parts",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserParts_ColorID",
                table: "UserParts",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_UserParts_PartID",
                table: "UserParts",
                column: "PartID");

            migrationBuilder.CreateIndex(
                name: "IX_UserParts_UserID_PartID_ColorID",
                table: "UserParts",
                columns: new[] { "UserID", "PartID", "ColorID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress",
                unique: true,
                filter: "[EmailAddress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserParts");

            migrationBuilder.DropTable(
                name: "LegoColors");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
