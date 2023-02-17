using System;
using DAL.Entities;
using DAL.Entities.Schedule;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;

namespace DAL
{
	public class InitializeDB
	{
        public static void Initialize(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(
              new Role { Id = 1, Name = "admin", DisplayName = "Администратор" });

            builder.Entity<Role>().HasData(
              new Role { Id = 2, Name = "employee", DisplayName = "Сотрудник" });

            //----------------------

            builder.Entity<Employee>().HasData(
              new Employee { Id = 1, FirstName = "Администратор", Name = "Администратор", MiddleName = "Администратор" });
            //----------------------

            builder.Entity<Account>().HasData(
              new Account { Id = 1, Login = "mmadminchesu", EmployeeId = 1, Password = HashHelper.HashPassword("x9aVWNJf32"), IsValid = true });
            //----------------------

            builder.Entity<Subdivision>().HasData(
              new Subdivision { Id = 1, Name = "ОБЩЕЕ ПОДРАЗДЕЛЕНИЕ" });

            builder.Entity<EmployeeSubdivision>().HasData(
              new EmployeeSubdivision { Id = 1, EmployeeId = 1, SubdivisionId = 1 });
            //----------------------

            builder.Entity<EmployeeRole>().HasData(
              new EmployeeRole { Id = 1, EmployeeId = 1, RoleId = 1 });

            builder.Entity<EmployeeRole>().HasData(
              new EmployeeRole { Id = 2, EmployeeId = 1, RoleId = 2 });

            //------------------------------------------------

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 1, Name = "Лекционная" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 2, Name = "Лаборатория" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 3, Name = "Практическая" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 4, Name = "Амфитеатр" });

            //------------------------------------------------

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 1, Name = "Лекция" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 2, Name = "Лабораторная" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 3, Name = "Практическое" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 4, Name = "Семинарское" });


            //-------------------------------------------------
            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 1, Name = "Очная" });

            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 2, Name = "Заочная" });

            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 3, Name = "Очно-заочная" });


            //-------------------------------------------------
            builder.Entity<Entities.Version>().HasData(
                new Entities.Version
                {
                    Id = 1,
                    Name = $"Расписание на {DateTime.Now.Date.Year} год.",
                    IsActive = true,
                    MaxLesson = 6,
                    UseSunday = true,
                    UseSubWeek = false,
                    UseSubClass = false,
                    ShowClassShift = false,
                    ShowEducationForm = false
                });

            //-------------------------------------------------
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 1, Name = "Первая" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 2, Name = "Вторая" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 3, Name = "Третья" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 4, Name = "Четвертая" });

            //-------------------------------------------------
            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 1, Name = "Экзамен" });

            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 2, Name = "Зачет" });

            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 3, Name = "Зачет с оценкой" });

        }
    }
}

