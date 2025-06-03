using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProofedAndPolished.Migrations
{
    /// <inheritdoc />
    public partial class Testing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Date", "PostedToFacebook", "PostedToWebsite" },
                values: new object[] { new DateTime(2025, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Date", "PostedToFacebook", "PostedToWebsite" },
                values: new object[] { new DateTime(2025, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
