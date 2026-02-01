using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Migrations
{
    /// <inheritdoc />
    public partial class Createinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrixAchat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrixVente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DureeGarantieMois = table.Column<int>(type: "int", nullable: false),
                    DateMiseEnStock = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstDisponible = table.Column<bool>(type: "bit", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Reference",
                table: "Articles",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
