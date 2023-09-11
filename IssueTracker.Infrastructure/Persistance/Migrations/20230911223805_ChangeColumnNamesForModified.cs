using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Infrastructure.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class ChangeColumnNamesForModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Projects",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Issues",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "AuditEvent",
                newName: "ModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Projects",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Issues",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "ModifiedById",
                table: "AuditEvent",
                newName: "ModifiedBy");
        }
    }
}
