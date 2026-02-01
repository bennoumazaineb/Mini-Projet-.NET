using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microservice2_Reclamations.Migrations
{
    /// <inheritdoc />
    public partial class createInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reclamations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientNom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticleReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAchat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SousGarantie = table.Column<bool>(type: "bit", nullable: false),
                    MontantFacture = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ResponsableSAVId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsableSAVNom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCloture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotesInterne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Solution = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_ClientId",
                table: "Reclamations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_DateCreation",
                table: "Reclamations",
                column: "DateCreation");

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_Statut",
                table: "Reclamations",
                column: "Statut");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reclamations");
        }
    }
}
