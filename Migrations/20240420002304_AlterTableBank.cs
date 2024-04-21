using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_banks_user_id",
                table: "banks");

            migrationBuilder.AlterColumn<int>(
                name: "balance",
                table: "banks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.CreateIndex(
                name: "IX_banks_user_id",
                table: "banks",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_banks_user_id",
                table: "banks");

            migrationBuilder.AlterColumn<decimal>(
                name: "balance",
                table: "banks",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_banks_user_id",
                table: "banks",
                column: "user_id");
        }
    }
}
