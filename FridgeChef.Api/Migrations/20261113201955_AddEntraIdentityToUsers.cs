using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEntraIdentityToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntraObjectId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntraTenantId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EntraTenantId_EntraObjectId",
                table: "Users",
                columns: new[] { "EntraTenantId", "EntraObjectId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_EntraTenantId_EntraObjectId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EntraObjectId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EntraTenantId",
                table: "Users");
        }
    }
}
