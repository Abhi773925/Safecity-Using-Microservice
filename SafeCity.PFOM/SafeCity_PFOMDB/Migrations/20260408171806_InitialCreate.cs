using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeCity_PFOMDB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patrol",
                columns: table => new
                {
                    PatrolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficerId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrol", x => x.PatrolId);
                });

            migrationBuilder.CreateTable(
                name: "FieldReport",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatrolId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldReport", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_FieldReport_Patrol_PatrolId",
                        column: x => x.PatrolId,
                        principalTable: "Patrol",
                        principalColumn: "PatrolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldReport_PatrolId",
                table: "FieldReport",
                column: "PatrolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldReport");

            migrationBuilder.DropTable(
                name: "Patrol");
        }
    }
}
