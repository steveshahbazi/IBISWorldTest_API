using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IBISWorldTest.Migrations
{
    /// <inheritdoc />
    public partial class SeedTermTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Terms",
                columns: new[] { "Id", "Definition", "Name" },
                values: new object[,]
                {
                    { 1, "Practice of writing program", "Coding" },
                    { 2, "A great company to work", "Ibis" },
                    { 3, "Good way of making web apps", "Web API application" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Terms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Terms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Terms",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
