using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterventionService.Migrations
{
    /// <inheritdoc />
    public partial class CreateInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReclamationId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    TechnicianName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InterventionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsUnderWarranty = table.Column<bool>(type: "bit", nullable: false),
                    PartsCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    LaborCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstimatedHours = table.Column<int>(type: "int", nullable: true),
                    ArticleReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_InterventionDate",
                table: "Interventions",
                column: "InterventionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_ReclamationId",
                table: "Interventions",
                column: "ReclamationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_Status",
                table: "Interventions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_TechnicianId",
                table: "Interventions",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_IsActive",
                table: "Technicians",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_UserId",
                table: "Technicians",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "Technicians");
        }
    }
}
