using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CryptoPlayground.Data.Migrations
{
    public partial class TeamLetterTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Letters");

            migrationBuilder.DropColumn(
                name: "UnlockedOn",
                table: "Letters");

            migrationBuilder.CreateTable(
                name: "TeamLetters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LetterId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    UnlockedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamLetters_Letters_LetterId",
                        column: x => x.LetterId,
                        principalTable: "Letters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamLetters_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamLetters_LetterId",
                table: "TeamLetters",
                column: "LetterId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamLetters_TeamId",
                table: "TeamLetters",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamLetters");

            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "Letters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Letters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnlockedOn",
                table: "Letters",
                nullable: true);
        }
    }
}
