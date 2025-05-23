﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(ScheduleHighSchoolDb))]
    [Migration("20241230105203_InitialDB")]
    partial class InitialDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DAL.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean");

                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EmployeeId = 1,
                            IsValid = true,
                            Login = "mmadminchesu",
                            Password = "jxDxllZXPuVBOuC4PrRKtTXWLQ8="
                        });
                });

            modelBuilder.Entity("DAL.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Администратор",
                            MiddleName = "Администратор",
                            Name = "Администратор"
                        });
                });

            modelBuilder.Entity("DAL.Entities.EmployeeRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("RoleId");

                    b.ToTable("EmployeeRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EmployeeId = 1,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            EmployeeId = 1,
                            RoleId = 2
                        });
                });

            modelBuilder.Entity("DAL.Entities.EmployeeSubdivision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<int>("SubdivisionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("SubdivisionId");

                    b.ToTable("EmployeeSubdivisions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            EmployeeId = 1,
                            SubdivisionId = 1
                        });
                });

            modelBuilder.Entity("DAL.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayName = "Администратор",
                            Name = "admin"
                        },
                        new
                        {
                            Id = 2,
                            DisplayName = "Сотрудник",
                            Name = "employee"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Building", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Building");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.ClassShift", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ClassShifts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ПЕРВАЯ"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ВТОРАЯ"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ТРЕТЬЯ"
                        },
                        new
                        {
                            Id = 4,
                            Name = "ЧЕТВЕРТАЯ"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Classroom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BuildingId")
                        .HasColumnType("integer");

                    b.Property<int>("ClassroomTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("SeatsCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.HasIndex("ClassroomTypeId");

                    b.ToTable("Classrooms");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.ClassroomType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ClassroomTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ЛЕКЦИОННАЯ"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ЛАБОРАТОРИЯ"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ПРАКТИЧЕСКАЯ"
                        },
                        new
                        {
                            Id = 4,
                            Name = "АМФИТЕАТР"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.EducationForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EducationForms");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ОЧНАЯ"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ЗАОЧНАЯ"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ОЧНО-ЗАОЧНАЯ"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Flow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("StudyClassList")
                        .HasColumnType("text");

                    b.Property<string>("TeacherList")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Flows");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Lesson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClassroomId")
                        .HasColumnType("integer");

                    b.Property<int?>("ColIndex")
                        .HasColumnType("integer");

                    b.Property<int?>("FlowId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsParallel")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSubClassLesson")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSubWeekLesson")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LessonTypeId")
                        .HasColumnType("integer");

                    b.Property<int?>("RowIndex")
                        .HasColumnType("integer");

                    b.Property<int>("StudyClassId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int>("TeacherId")
                        .HasColumnType("integer");

                    b.Property<int>("VersionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("FlowId");

                    b.HasIndex("LessonTypeId");

                    b.HasIndex("StudyClassId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TeacherId");

                    b.HasIndex("VersionId");

                    b.ToTable("Lessons");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.LessonType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LessonTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ЛЕКЦИЯ"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ЛАБОРАТОРНАЯ"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ПРАКТИЧЕСКОЕ"
                        },
                        new
                        {
                            Id = 4,
                            Name = "СЕМИНАРСКОЕ"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.ReportingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ReportingType");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ЭКЗАМЕН"
                        },
                        new
                        {
                            Id = 2,
                            Name = "ЗАЧЕТ"
                        },
                        new
                        {
                            Id = 3,
                            Name = "ЗЕАЧТ С ОЦЕНКОЙ"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.StudyClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ClassShiftId")
                        .HasColumnType("integer");

                    b.Property<int>("EducationFormId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("StudentsCount")
                        .HasColumnType("integer");

                    b.Property<int>("SubdivisionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClassShiftId");

                    b.HasIndex("EducationFormId");

                    b.HasIndex("SubdivisionId");

                    b.ToTable("StudyClasses");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.StudyClassReporting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClassroomId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ReportingTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("StudyClassId")
                        .HasColumnType("integer");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int>("TeacherId")
                        .HasColumnType("integer");

                    b.Property<int>("VersionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ClassroomId");

                    b.HasIndex("ReportingTypeId");

                    b.HasIndex("StudyClassId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TeacherId");

                    b.HasIndex("VersionId");

                    b.ToTable("StudyClassReporting");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Subdivision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subdivisions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "ОБЩЕЕ ПОДРАЗДЕЛЕНИЕ"
                        });
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Teacher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LessonNumbers")
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("WeekDays")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("DAL.Entities.Version", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("MaxLesson")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("ShowClassShift")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowEducationForm")
                        .HasColumnType("boolean");

                    b.Property<string>("ShowReportingIds")
                        .HasColumnType("text");

                    b.Property<bool>("UseSubClass")
                        .HasColumnType("boolean");

                    b.Property<bool>("UseSubWeek")
                        .HasColumnType("boolean");

                    b.Property<bool>("UseSunday")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Versions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsActive = true,
                            MaxLesson = 6,
                            Name = "Расписание на 2024 год.",
                            ShowClassShift = false,
                            ShowEducationForm = false,
                            UseSubClass = false,
                            UseSubWeek = false,
                            UseSunday = true
                        });
                });

            modelBuilder.Entity("DAL.Entities.Account", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithOne("Account")
                        .HasForeignKey("DAL.Entities.Account", "EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("DAL.Entities.EmployeeRole", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany("EmployeeRoles")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DAL.Entities.EmployeeSubdivision", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Subdivision", "Subdivision")
                        .WithMany()
                        .HasForeignKey("SubdivisionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Subdivision");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Classroom", b =>
                {
                    b.HasOne("DAL.Entities.Schedule.Building", "Building")
                        .WithMany()
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.ClassroomType", "ClassroomType")
                        .WithMany()
                        .HasForeignKey("ClassroomTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Building");

                    b.Navigation("ClassroomType");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.Lesson", b =>
                {
                    b.HasOne("DAL.Entities.Schedule.Classroom", "Classroom")
                        .WithMany()
                        .HasForeignKey("ClassroomId");

                    b.HasOne("DAL.Entities.Schedule.Flow", "Flow")
                        .WithMany()
                        .HasForeignKey("FlowId");

                    b.HasOne("DAL.Entities.Schedule.LessonType", "LessonType")
                        .WithMany()
                        .HasForeignKey("LessonTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.StudyClass", "StudyClass")
                        .WithMany()
                        .HasForeignKey("StudyClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Teacher", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Version", "Version")
                        .WithMany()
                        .HasForeignKey("VersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Classroom");

                    b.Navigation("Flow");

                    b.Navigation("LessonType");

                    b.Navigation("StudyClass");

                    b.Navigation("Subject");

                    b.Navigation("Teacher");

                    b.Navigation("Version");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.StudyClass", b =>
                {
                    b.HasOne("DAL.Entities.Schedule.ClassShift", "ClassShift")
                        .WithMany()
                        .HasForeignKey("ClassShiftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.EducationForm", "EducationForm")
                        .WithMany()
                        .HasForeignKey("EducationFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Subdivision", "Subdivision")
                        .WithMany()
                        .HasForeignKey("SubdivisionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassShift");

                    b.Navigation("EducationForm");

                    b.Navigation("Subdivision");
                });

            modelBuilder.Entity("DAL.Entities.Schedule.StudyClassReporting", b =>
                {
                    b.HasOne("DAL.Entities.Schedule.Classroom", "Classroom")
                        .WithMany()
                        .HasForeignKey("ClassroomId");

                    b.HasOne("DAL.Entities.Schedule.ReportingType", "ReportingType")
                        .WithMany()
                        .HasForeignKey("ReportingTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.StudyClass", "StudyClass")
                        .WithMany()
                        .HasForeignKey("StudyClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Schedule.Teacher", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Version", "Version")
                        .WithMany()
                        .HasForeignKey("VersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Classroom");

                    b.Navigation("ReportingType");

                    b.Navigation("StudyClass");

                    b.Navigation("Subject");

                    b.Navigation("Teacher");

                    b.Navigation("Version");
                });

            modelBuilder.Entity("DAL.Entities.Employee", b =>
                {
                    b.Navigation("Account");

                    b.Navigation("EmployeeRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
