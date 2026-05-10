using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeCity_EDRADB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispatch",
                columns: table => new
                {
                    DispatchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentID = table.Column<int>(type: "int", nullable: false),
                    DispatcherID = table.Column<int>(type: "int", nullable: false),
                    ResourceID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Date = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispatch", x => x.DispatchID);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    ResourceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Availability = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "varchar(max)", nullable: false),
                    UnitName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.ResourceID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispatch");

            migrationBuilder.DropTable(
                name: "Resource");
        }
    }
}
