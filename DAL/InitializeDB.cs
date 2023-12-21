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
              new ClassroomType { Id = 1, Name = "ЛЕКЦИОННАЯ" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 2, Name = "ЛАБОРАТОРИЯ" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 3, Name = "ПРАКТИЧЕСКАЯ" });

            builder.Entity<ClassroomType>().HasData(
              new ClassroomType { Id = 4, Name = "АМФИТЕАТР" });

            //------------------------------------------------

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 1, Name = "ЛЕКЦИЯ" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 2, Name = "ЛАБОРАТОРНАЯ" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 3, Name = "ПРАКТИЧЕСКОЕ" });

            builder.Entity<LessonType>().HasData(
              new LessonType { Id = 4, Name = "СЕМИНАРСКОЕ" });


            //-------------------------------------------------
            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 1, Name = "ОЧНАЯ" });

            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 2, Name = "ЗАОЧНАЯ" });

            builder.Entity<EducationForm>().HasData(
                new EducationForm { Id = 3, Name = "ОЧНО-ЗАОЧНАЯ" });


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
                new ClassShift { Id = 1, Name = "ПЕРВАЯ" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 2, Name = "ВТОРАЯ" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 3, Name = "ТРЕТЬЯ" });
            builder.Entity<ClassShift>().HasData(
                new ClassShift { Id = 4, Name = "ЧЕТВЕРТАЯ" });

            //-------------------------------------------------
            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 1, Name = "ЭКЗАМЕН" });

            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 2, Name = "ЗАЧЕТ" });

            builder.Entity<ReportingType>().HasData(
                new ReportingType { Id = 3, Name = "ЗЕАЧТ С ОЦЕНКОЙ" });

        }
    }
}

