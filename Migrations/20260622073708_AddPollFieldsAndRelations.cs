using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMD_Course_Work.Migrations
{
    /// <inheritdoc />
    public partial class AddPollFieldsAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VoterToken",
                table: "Votes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Polls",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Options",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PollId_VoterToken",
                table: "Votes",
                columns: new[] { "PollId", "VoterToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_Code",
                table: "Polls",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Options_PollId",
                table: "Options",
                column: "PollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Options_Polls_PollId",
                table: "Options",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Options_Polls_PollId",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PollId_VoterToken",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Polls_Code",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Options_PollId",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Options");

            migrationBuilder.AlterColumn<string>(
                name: "VoterToken",
                table: "Votes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
