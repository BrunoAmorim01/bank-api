using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_origin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_destination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_origin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_destination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    transaction_type = table.Column<int>(type: "integer", nullable: false),
                    transaction_status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_banks_bank_destination_id",
                        column: x => x.bank_destination_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_banks_bank_origin_id",
                        column: x => x.bank_origin_id,
                        principalTable: "banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_users_user_destination_id",
                        column: x => x.user_destination_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_users_user_origin_id",
                        column: x => x.user_origin_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_bank_destination_id",
                table: "transactions",
                column: "bank_destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_bank_origin_id",
                table: "transactions",
                column: "bank_origin_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_destination_id",
                table: "transactions",
                column: "user_destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_origin_id",
                table: "transactions",
                column: "user_origin_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions");
        }
    }
}
