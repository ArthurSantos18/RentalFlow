using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserEntityId",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserEntityId",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "UserTokens");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted_CreatedAt",
                table: "Users",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsDeleted_CreatedAt",
                table: "Teams",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_IsDeleted_CreatedAt",
                table: "RentalApplications",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_Status_IsDeleted",
                table: "RentalApplications",
                columns: new[] { "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_IsDeleted_CreatedAt",
                table: "Properties",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Operators_IsDeleted_CreatedAt",
                table: "Operators",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Operators_Role_IsActive_IsDeleted",
                table: "Operators",
                columns: new[] { "Role", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_IsDeleted_CreatedAt",
                table: "Applicants",
                columns: new[] { "IsDeleted", "CreatedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsDeleted_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teams_IsDeleted_CreatedAt",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_Name",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_RentalApplications_IsDeleted_CreatedAt",
                table: "RentalApplications");

            migrationBuilder.DropIndex(
                name: "IX_RentalApplications_Status_IsDeleted",
                table: "RentalApplications");

            migrationBuilder.DropIndex(
                name: "IX_Properties_IsDeleted_CreatedAt",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Operators_IsDeleted_CreatedAt",
                table: "Operators");

            migrationBuilder.DropIndex(
                name: "IX_Operators_Role_IsActive_IsDeleted",
                table: "Operators");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_IsDeleted_CreatedAt",
                table: "Applicants");

            migrationBuilder.AddColumn<Guid>(
                name: "UserEntityId",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserEntityId",
                table: "UserTokens",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserEntityId",
                table: "UserTokens",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
