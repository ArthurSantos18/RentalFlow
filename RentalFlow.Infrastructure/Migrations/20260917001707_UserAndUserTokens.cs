using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserAndUserTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEntity_Operators_OperatorId",
                table: "UserEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokenEntity_UserEntity_UserId",
                table: "UserTokenEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokenEntity",
                table: "UserTokenEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEntity",
                table: "UserEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserEntity_OperatorId",
                table: "UserEntity");

            migrationBuilder.RenameTable(
                name: "UserTokenEntity",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                name: "UserEntity",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokenEntity_UserId",
                table: "UserTokens",
                newName: "IX_UserTokens_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserEntityId",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_RefreshToken",
                table: "UserTokens",
                column: "RefreshToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserEntityId",
                table: "UserTokens",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OperatorId",
                table: "Users",
                column: "OperatorId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Operators_OperatorId",
                table: "Users",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserEntityId",
                table: "UserTokens",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Operators_OperatorId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserEntityId",
                table: "UserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_RefreshToken",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserEntityId",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OperatorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "UserTokens");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                newName: "UserTokenEntity");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserEntity");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokenEntity",
                newName: "IX_UserTokenEntity_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserTokenEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "UserEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokenEntity",
                table: "UserTokenEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEntity",
                table: "UserEntity",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntity_OperatorId",
                table: "UserEntity",
                column: "OperatorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEntity_Operators_OperatorId",
                table: "UserEntity",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokenEntity_UserEntity_UserId",
                table: "UserTokenEntity",
                column: "UserId",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
