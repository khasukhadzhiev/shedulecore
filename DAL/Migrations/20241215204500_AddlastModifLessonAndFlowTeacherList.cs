using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddlastModifLessonAndFlowTeacherList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Lessons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StudyClassList",
                table: "Flows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherList",
                table: "Flows",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Versions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Расписание на 2024 год.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "StudyClassList",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "TeacherList",
                table: "Flows");

            migrationBuilder.UpdateData(
                table: "Versions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Расписание на 2023 год.");
        }
    }
}
