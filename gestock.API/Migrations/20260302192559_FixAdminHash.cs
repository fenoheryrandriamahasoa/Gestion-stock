using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestock.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$v8pwCwQ4kmg2Ro7vl/w7se2vT/RJDp/cyj0fwBhH1kuojm6ZkYWtS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$KIXbCPm5dRmVJfGhT0wbUOPZwJGMqaFNQ5JhXKYvGxLIhzWqQZGy");
        }
    }
}
