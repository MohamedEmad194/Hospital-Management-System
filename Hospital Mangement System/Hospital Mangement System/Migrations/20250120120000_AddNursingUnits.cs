using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Mangement_System.Migrations
{
    /// <inheritdoc />
    public partial class AddNursingUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NursingUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Wing = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Lead = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nurses = table.Column<int>(type: "int", nullable: false),
                    Coverage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ratio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Focus = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NursingUnits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NursingUnits_UnitId",
                table: "NursingUnits",
                column: "UnitId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NursingUnits");
        }
    }
}

