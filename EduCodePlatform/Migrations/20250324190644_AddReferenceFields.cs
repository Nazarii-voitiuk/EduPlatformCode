using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCodePlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_AppUser_CreatedBy",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_CreatedBy",
                table: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Task",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceCss",
                table: "Task",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceHtml",
                table: "Task",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceJs",
                table: "Task",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceCss",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ReferenceHtml",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ReferenceJs",
                table: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Task",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreatedBy",
                table: "Task",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_AppUser_CreatedBy",
                table: "Task",
                column: "CreatedBy",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
