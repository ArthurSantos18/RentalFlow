using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted_IsActive",
                table: "Users",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsDeleted_IsActive",
                table: "Teams",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_IsDeleted_IsActive",
                table: "RentalApplications",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_IsDeleted_IsActive",
                table: "Properties",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Operators_IsDeleted_IsActive",
                table: "Operators",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_IsDeleted_IsActive",
                table: "Applicants",
                columns: new[] { "IsDeleted", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted_IsActive",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teams_IsDeleted_IsActive",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_RentalApplications_IsDeleted_IsActive",
                table: "RentalApplications");

            migrationBuilder.DropIndex(
                name: "IX_Properties_IsDeleted_IsActive",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Operators_IsDeleted_IsActive",
                table: "Operators");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_IsDeleted_IsActive",
                table: "Applicants");
        }
    }
}
