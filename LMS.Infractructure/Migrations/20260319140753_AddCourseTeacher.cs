using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeacher_AspNetUsers_TeacherId",
                table: "CourseTeacher");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeacher_Courses_CourseId",
                table: "CourseTeacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTeacher",
                table: "CourseTeacher");

            migrationBuilder.RenameTable(
                name: "CourseTeacher",
                newName: "CourseTeachers");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTeacher_TeacherId",
                table: "CourseTeachers",
                newName: "IX_CourseTeachers_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTeachers",
                table: "CourseTeachers",
                columns: new[] { "CourseId", "TeacherId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeachers_AspNetUsers_TeacherId",
                table: "CourseTeachers",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeachers_Courses_CourseId",
                table: "CourseTeachers",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeachers_AspNetUsers_TeacherId",
                table: "CourseTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTeachers_Courses_CourseId",
                table: "CourseTeachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTeachers",
                table: "CourseTeachers");

            migrationBuilder.RenameTable(
                name: "CourseTeachers",
                newName: "CourseTeacher");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTeachers_TeacherId",
                table: "CourseTeacher",
                newName: "IX_CourseTeacher_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTeacher",
                table: "CourseTeacher",
                columns: new[] { "CourseId", "TeacherId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeacher_AspNetUsers_TeacherId",
                table: "CourseTeacher",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTeacher_Courses_CourseId",
                table: "CourseTeacher",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
