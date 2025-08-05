using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Database.Migrations
{
    /// <inheritdoc />
    public partial class addcheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyPayDate",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "Check",
                table: "Payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyPayDay",
                table: "Payments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Check",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MonthlyPayDay",
                table: "Payments");

            migrationBuilder.AddColumn<DateOnly>(
                name: "MonthlyPayDate",
                table: "Payments",
                type: "date",
                nullable: true);
        }
    }
}
