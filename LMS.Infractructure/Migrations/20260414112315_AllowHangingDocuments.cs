using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowHangingDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Document_ExactlyOneOwner",
                table: "Documents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Document_AtMostOneOwner",
                table: "Documents",
                sql: "(\r\n    CASE WHEN CourseId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN ModuleId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN ActivityId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN SubmissionId IS NOT NULL THEN 1 ELSE 0 END\r\n) <= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Document_AtMostOneOwner",
                table: "Documents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Document_ExactlyOneOwner",
                table: "Documents",
                sql: "(\r\n    CASE WHEN CourseId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN ModuleId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN ActivityId IS NOT NULL THEN 1 ELSE 0 END +\r\n    CASE WHEN SubmissionId IS NOT NULL THEN 1 ELSE 0 END\r\n) = 1");
        }
    }
}
