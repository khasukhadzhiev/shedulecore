using System;
using System.Reflection.Emit;
using DAL.Configurations;
using DAL.Entities;
using DAL.Entities.Schedule;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
	public class ScheduleHighSchoolDb: DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Building> Building { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<LessonType> LessonTypes { get; set; }
        public DbSet<StudyClass> StudyClasses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<ClassroomType> ClassroomTypes { get; set; }
        public DbSet<Entities.Version> Versions { get; set; }
        public DbSet<EducationForm> EducationForms { get; set; }
        public DbSet<ClassShift> ClassShifts { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<EmployeeSubdivision> EmployeeSubdivisions { get; set; }
        public DbSet<StudyClassReporting> StudyClassReporting { get; set; }
        public DbSet<ReportingType> ReportingType { get; set; }

        public ScheduleHighSchoolDb(DbContextOptions<ScheduleHighSchoolDb> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TeacherConfiguration());

            builder.ApplyConfiguration(new VersionConfiguration());

            builder.ApplyConfiguration(new FlowTeachersIdsConfiguration());

            InitializeDB.Initialize(builder);

            base.OnModelCreating(builder);
        }


        //dotnet ef migrations add InitialCreate -s ../CoreAPI
        //dotnet ef database update -s ../CoreAPI
    }
}

