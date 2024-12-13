using System;
using DAL.Entities;
using DAL.Entities.Schedule;
using DTL.Dto;
using DTL.Dto.ScheduleDto;
using System.Data;
using System.Reflection;
using System.Security.Principal;

namespace DTL.Mapping
{
	public static class MapperDto
	{
        public static EmployeeDto ToEmployeeDto(this Employee employee)
        {
            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                Name = employee.Name,
                MiddleName = employee.MiddleName,
                IsValid = employee.Account.IsValid,
                Account = employee.Account.ToAccountDto(),
            };

            return employeeDto;
        }

        public static RoleDto ToRoleDto(this Role role)
        {
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName
            };

            return roleDto;
        }

        public static AccountDto ToAccountDto(this Account account)
        {
            var accountDto = new AccountDto
            {
                Id = account.Id,
                Login = account.Login,
                Password = account.Password,
                IsValid = account.IsValid
            };

            return accountDto;
        }

        public static EducationFormDto ToEducationFormDto(this EducationForm educationForm)
        {
            var educationFormDto = new EducationFormDto
            {
                Id = educationForm.Id,
                Name = educationForm.Name.Trim().ToUpper()
            };

            return educationFormDto;
        }

        public static ClassShiftDto ToClassShiftDto(this ClassShift classShift)
        {
            var classShiftDto = new ClassShiftDto
            {
                Id = classShift.Id,
                Name = classShift.Name.Trim().ToUpper()
            };

            return classShiftDto;
        }

        public static SubdivisionDto ToSubdivisionDto(this Subdivision subdivision)
        {
            var subdivisionDto = new SubdivisionDto
            {
                Id = subdivision.Id,
                Name = subdivision.Name.Trim().ToUpper()
            };

            return subdivisionDto;
        }

        public static SubjectDto ToSubjectDto(this Subject subject)
        {
            var subjectDto = new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name.Trim().ToUpper()
            };

            return subjectDto;
        }

        public static T ToDto<T>(this BaseEntity entity)
        {
            try
            {
                Type entityType = entity.GetType();

                PropertyInfo idProperty = entityType.GetProperty("Id");

                PropertyInfo nameProperty = entityType.GetProperty("Name");

                var dto = (T)Activator.CreateInstance(typeof(T));

                dto.GetType().GetProperty("Id").SetValue(dto, entity.Id);

                dto.GetType().GetProperty("Name").SetValue(dto, (string)nameProperty.GetValue(entity));

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static StudyClassDto ToStudyClassDto(this StudyClass studyClass)
        {
            var studyClassDto = new StudyClassDto
            {
                Id = studyClass.Id,
                Name = studyClass.Name,
                StudentsCount = studyClass.StudentsCount,
                EducationFormId = studyClass.EducationFormId,
                ClassShiftId = studyClass.ClassShiftId,
                SubdivisionId = studyClass.SubdivisionId,
                Subdivision = studyClass.Subdivision?.ToDto<SubdivisionDto>(),
                ClassShift = studyClass.ClassShift?.ToDto<ClassShiftDto>(),
                EducationForm = studyClass.EducationForm?.ToDto<EducationFormDto>(),
            };

            return studyClassDto;
        }

        public static ClassroomDto ToClassroomDto(this Classroom classroom)
        {
            var classroomDto = new ClassroomDto
            {
                Id = classroom.Id,
                Name = classroom.Name,
                ClassroomTypeId = classroom.ClassroomTypeId,
                SeatsCount = classroom.SeatsCount,
                BuildingId = classroom.BuildingId,
                BuildingDto = classroom?.Building?.ToDto<BuildingDto>()
            };

            return classroomDto;
        }

        public static ClassroomTypeDto ToClassroomTypeDto(this ClassroomType classroomType)
        {
            var classroomTypeDto = new ClassroomTypeDto
            {
                Id = classroomType.Id,
                Name = classroomType.Name
            };

            return classroomTypeDto;
        }

        public static TeacherDto ToTeacherDto(this Teacher teacher)
        {
            var teacherDto = new TeacherDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                Name = teacher.Name,
                MiddleName = teacher.MiddleName,
                LessonNumbers = teacher.LessonNumbers,
                WeekDays = teacher.WeekDays,
                FullName = $"{teacher.FirstName?.ToUpper()} {teacher.Name?.ToUpper()} {teacher.MiddleName?.ToUpper()}"
            };

            return teacherDto;
        }

        public static FlowDto ToFlowDto(this Flow flow)
        {
            var flowDto = new FlowDto
            {
                Id = flow.Id,
                Name = flow?.Name,
                TeacherList = flow?.TeacherList?.Select(t => t.ToTeacherDto()).ToList(),
            };

            return flowDto;
        }

        public static LessonDto ToLessonDto(this Lesson lesson)
        {
            var lessonDto = new LessonDto
            {
                Id = lesson.Id,
                ClassroomId = lesson.ClassroomId,
                FlowId = lesson.FlowId,
                FlowStudyClassNames = lesson?.Flow?.Name,
                LessonTypeId = lesson.LessonTypeId,
                StudyClassId = lesson.StudyClassId,
                SubjectId = lesson.SubjectId,
                TeacherId = lesson.TeacherId,
                RowIndex = lesson.RowIndex,
                ColIndex = lesson.ColIndex,
                IsParallel = lesson.IsParallel,
                IsSubClassLesson = lesson.IsSubClassLesson,
                IsSubWeekLesson = lesson.IsSubWeekLesson,
                VersionId = lesson.VersionId,
                LessonDay = lesson.LessonDay,
                LessonNumber = lesson.LessonNumber,
                WeekNumber = lesson.WeekNumber,

                Version = lesson?.Version?.ToVersionDto(),
                Classroom = lesson?.Classroom?.ToClassroomDto(),
                Flow = lesson?.Flow?.ToFlowDto(),
                LessonType = lesson?.LessonType?.ToDto<LessonTypeDto>(),
                StudyClass = lesson?.StudyClass?.ToDto<StudyClassDto>(),
                Subject = lesson?.Subject?.ToDto<SubjectDto>(),
                Teacher = lesson?.Teacher?.ToTeacherDto(),
                Name = null,
            };

            if(lessonDto.FlowId != null)
            {
                lessonDto.Teacher.FullName = string.Join(",", lessonDto.Flow.TeacherList.Select(t => t.FullName));
            }

            return lessonDto;
        }

        public static VersionDto ToVersionDto(this global::DAL.Entities.Version version)
        {
            var versionDto = new VersionDto
            {
                Id = version.Id,
                MaxLesson = version.MaxLesson,
                UseSunday = version.UseSunday,
                UseSubWeek = version.UseSubWeek,
                UseSubClass = version.UseSubClass,
                ShowClassShift = version.ShowClassShift,
                ShowEducationForm = version.ShowEducationForm,
                Name = version.Name,
                IsActive = version.IsActive,
                ShowReportingIds = version.ShowReportingIds
            };

            return versionDto;
        }

        public static StudyClassReportingDto ToStudyClassReportingDto(this StudyClassReporting studyClassReporting)
        {
            var StudyClassReportingDto = new StudyClassReportingDto
            {
                Id = studyClassReporting.Id,
                SubjectId = studyClassReporting.SubjectId,
                TeacherId = studyClassReporting.TeacherId,
                ReportingTypeId = studyClassReporting.ReportingTypeId,
                StudyClassId = studyClassReporting.StudyClassId,
                ClassroomId = studyClassReporting?.ClassroomId,
                VersionId = studyClassReporting.VersionId,
                Date = studyClassReporting?.Date?.ToString("o"),
                Subject = studyClassReporting?.Subject?.ToSubjectDto(),
                Teacher = studyClassReporting?.Teacher?.ToTeacherDto(),
                ReportingType = studyClassReporting?.ReportingType?.ToReportingTypeDto(),
                StudyClass = studyClassReporting?.StudyClass?.ToStudyClassDto(),
                Classroom = studyClassReporting?.Classroom?.ToClassroomDto(),
                Version = studyClassReporting?.Version?.ToVersionDto(),
            };

            return StudyClassReportingDto;
        }

        public static ReportingTypeDto ToReportingTypeDto(this ReportingType reportingType)
        {
            var reportingTypeDto = new ReportingTypeDto
            {
                Id = reportingType.Id,
                Name = reportingType.Name
            };

            return reportingTypeDto;
        }
    }
}

