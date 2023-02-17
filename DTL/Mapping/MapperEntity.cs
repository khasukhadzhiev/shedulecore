using System;
using DTL.Dto;
using DTL.Dto.ScheduleDto;
using DAL.Entities;
using DAL.Entities.Schedule;
using Infrastructure.Helpers;
using System.Reflection;
using System.Security.Principal;

namespace DTL.Mapping
{
	public static class MapperEntity
	{
        public static Employee ToEmployee(this EmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName?.Trim()?.ToUpper(),
                Name = employeeDto.Name?.Trim()?.ToUpper(),
                MiddleName = employeeDto.MiddleName?.Trim()?.ToUpper(),
            };

            return employee;
        }

        public static Account ToAccount(this EmployeeDto employeeDto, Employee employee)
        {
            var account = new Account
            {
                Login = employeeDto.Account.Login,
                Password = HashHelper.HashPassword(employeeDto.Account.Password),
                Employee = employee,
                EmployeeId = employee.Id,
                IsValid = employeeDto.IsValid
            };

            return account;
        }

        public static Subject ToSubject(this SubjectDto subjectDto)
        {
            var subject = new Subject
            {
                Id = subjectDto.Id,
                Name = subjectDto.Name.Trim().ToUpper()
            };

            return subject;
        }

        public static T ToEntity<T>(this BaseDto dto)
        {
            try
            {
                Type dtoType = dto.GetType();

                PropertyInfo idProperty = dtoType.GetProperty("Id");

                PropertyInfo nameProperty = dtoType.GetProperty("Name");

                var entity = (T)Activator.CreateInstance(typeof(T));

                entity.GetType().GetProperty("Id").SetValue(entity, dto.Id);

                entity.GetType().GetProperty("Name").SetValue(entity, dto.Name.Trim().ToUpper());

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static StudyClass ToStudyClass(this StudyClassDto studyClassDto)
        {
            var studyClass = new StudyClass
            {
                Id = studyClassDto.Id,
                Name = studyClassDto.Name.Trim().ToUpper(),
                StudentsCount = studyClassDto.StudentsCount,
                EducationFormId = studyClassDto.EducationFormId,
                SubdivisionId = studyClassDto.SubdivisionId,
                Subdivision = studyClassDto.Subdivision?.ToEntity<Subdivision>(),
                ClassShiftId = studyClassDto.ClassShiftId,
                ClassShift = studyClassDto.ClassShift?.ToEntity<ClassShift>(),
                EducationForm = studyClassDto.EducationForm?.ToEntity<EducationForm>(),
            };

            return studyClass;
        }

        public static Classroom ToClassroom(this ClassroomDto classroomDto)
        {
            var classroom = new Classroom
            {
                Id = classroomDto.Id,
                Name = classroomDto.Name.Trim().ToUpper(),
                ClassroomTypeId = classroomDto.ClassroomTypeId,
                SeatsCount = classroomDto.SeatsCount,
                BuildingId = classroomDto.BuildingId,
                Building = classroomDto?.BuildingDto?.ToEntity<Building>()
            };

            return classroom;
        }

        public static Teacher ToTeacher(this TeacherDto teacherDto)
        {
            var teacher = new Teacher
            {
                Id = teacherDto.Id,
                FirstName = teacherDto.FirstName.Trim().ToUpper(),
                Name = teacherDto.Name.Trim().ToUpper(),
                MiddleName = teacherDto.MiddleName.Trim().ToUpper(),
                WeekDays = teacherDto.WeekDays,
                LessonNumbers = teacherDto.LessonNumbers
            };

            return teacher;
        }

        public static Lesson ToLesson(this LessonDto lessonDto)
        {
            var lesson = new Lesson
            {
                Id = lessonDto.Id,
                ClassroomId = lessonDto.ClassroomId,
                FlowId = lessonDto.FlowId,
                LessonTypeId = lessonDto.LessonTypeId,
                StudyClassId = lessonDto.StudyClassId,
                SubjectId = lessonDto.SubjectId,
                TeacherId = lessonDto.TeacherId,
                RowIndex = lessonDto.RowIndex,
                ColIndex = lessonDto.ColIndex,
                IsParallel = lessonDto.IsParallel,
                IsSubClassLesson = lessonDto.IsSubClassLesson,
                IsSubWeekLesson = lessonDto.IsSubWeekLesson,
                VersionId = lessonDto.VersionId,

                Version = lessonDto?.Version?.ToVersion(),
                Classroom = lessonDto?.Classroom?.ToClassroom(),
                Flow = lessonDto?.Flow?.ToEntity<Flow>(),
                LessonType = lessonDto?.LessonType?.ToEntity<LessonType>(),
                StudyClass = lessonDto?.StudyClass?.ToEntity<StudyClass>(),
                Subject = lessonDto?.Subject?.ToEntity<Subject>(),
                Teacher = lessonDto?.Teacher?.ToTeacher(),
            };

            return lesson;
        }

        public static DAL.Entities.Version ToVersion(this VersionDto versionDto)
        {
            var version = new DAL.Entities.Version
            {
                Id = versionDto.Id,
                MaxLesson = versionDto.MaxLesson,
                UseSunday = versionDto.UseSunday,
                UseSubWeek = versionDto.UseSubWeek,
                UseSubClass = versionDto.UseSubClass,
                ShowClassShift = versionDto.ShowClassShift,
                ShowEducationForm = versionDto.ShowEducationForm,
                Name = versionDto.Name,
                IsActive = versionDto.IsActive,
                ShowReportingIds = versionDto.ShowReportingIds
            };

            return version;
        }

        public static StudyClassReporting ToStudyClassReporting(this StudyClassReportingDto studyClassReportingDto)
        {
            var studyClassReporting = new StudyClassReporting
            {
                Id = studyClassReportingDto.Id,
                SubjectId = studyClassReportingDto.SubjectId,
                TeacherId = studyClassReportingDto.TeacherId,
                ReportingTypeId = studyClassReportingDto.ReportingTypeId,
                StudyClassId = studyClassReportingDto.StudyClassId,
                ClassroomId = studyClassReportingDto?.ClassroomId,
                VersionId = studyClassReportingDto.VersionId,
                Subject = studyClassReportingDto?.Subject?.ToSubject(),
                Teacher = studyClassReportingDto?.Teacher?.ToTeacher(),
                ReportingType = studyClassReportingDto?.ReportingType?.ToReportingType(),
                StudyClass = studyClassReportingDto?.StudyClass?.ToStudyClass(),
                Classroom = studyClassReportingDto?.Classroom?.ToClassroom(),
                Version = studyClassReportingDto?.Version?.ToVersion(),
            };

            if (string.IsNullOrEmpty(studyClassReportingDto?.Date))
            {
                studyClassReporting.Date = null;
            }
            else
            {
                studyClassReporting.Date = DateTime.Parse(studyClassReportingDto?.Date);
            }

            return studyClassReporting;
        }

        public static ReportingType ToReportingType(this ReportingTypeDto reportingTypeDto)
        {
            var reportingType = new ReportingType
            {
                Id = reportingTypeDto.Id,
                Name = reportingTypeDto.Name
            };

            return reportingType;
        }
    }
}

