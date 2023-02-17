using BL.ServiceInterface;
using DTL.Dto;
using DTL.Dto.ScheduleDto;
using ExcelDataReader;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL;

namespace BL.Services
{
    public class ImportService : IImportService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly ISubdivisionService _subdivisionService;

        private readonly ILessonService _lessonService;

        private readonly ITeacherService _teacherService;

        private readonly ISubjectService _subjectService;

        private readonly IStudyClassService _studyClassService;

        private readonly IStudyClassReportingService _studyClassReportingService;

        private readonly ImportProgress _importProgres;

        public ImportService(ScheduleHighSchoolDb context,
            ISubdivisionService subdivisionService,
            ILessonService lessonService,
            ITeacherService teacherService,
            ISubjectService subjectService,
            IStudyClassService studyClassService,
            IStudyClassReportingService studyClassReportingService,
            ImportProgress importProgres
            )
        {
            _context = context;
            _subdivisionService = subdivisionService;
            _lessonService = lessonService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _studyClassService = studyClassService;
            _importProgres = importProgres;
            _studyClassReportingService = studyClassReportingService;
        }

        public async Task<ImportProgress> GetImportProgressAsync()
        {
            var progress = new ImportProgress();

            Task<ImportProgress> getImportProgressTask = new Task<ImportProgress>(() =>
            {
                progress.TotalLessonCount = _importProgres.TotalLessonCount;
                progress.CheckedLessonCount = _importProgres.CheckedLessonCount;
                progress.ImportError = _importProgres.ImportError;
                progress.ErrorMessage = _importProgres.ErrorMessage;
                progress.ImportFinished = _importProgres.ImportFinished;
                progress.InProcess = _importProgres.InProcess;

                return progress;
            });

            getImportProgressTask.Start();

            return await getImportProgressTask;

        }

        public async Task RemoveImportProgressAsync()
        {
            Task removeImportTask = new Task(() =>
            {
                _importProgres.TotalLessonCount = 0;
                _importProgres.CheckedLessonCount = 0;
                _importProgres.ImportError = false;
                _importProgres.ErrorMessage = null;
                _importProgres.ImportFinished = true;
                _importProgres.InProcess = false;
            });

            removeImportTask.Start();

            await removeImportTask;
        }

        ///<inheritdoc/>
        public async Task ImportScheduleDataAsync(IFormFile file, int versionId)
        {
            if (_importProgres.InProcess)
            {
                throw new Exception("Предыдущий импорт еще не закончен!");
            }

            _importProgres.TotalLessonCount = 0;
            _importProgres.CheckedLessonCount = 0;
            _importProgres.ImportError = false;
            _importProgres.ErrorMessage = null;
            _importProgres.ImportFinished = false;
            _importProgres.InProcess = true;

            //Первая строка файла импорта не заносится. Первой строкой должны быть заголовки столбцов.

            List<ImportDataModel> dataList = new List<ImportDataModel>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var reader = ExcelReaderFactory.CreateReader(stream);

                _importProgres.TotalLessonCount = reader.RowCount;

                int rowIndex = 0;
                while (reader.Read())
                {
                    try
                    {
                        if (rowIndex == 0)
                        {
                            rowIndex++;
                            continue;
                        }

                        ImportDataModel importDataModel = new ImportDataModel();

                        importDataModel.Subdivision = reader.GetValue(0).ToString().Trim().ToUpper();
                        importDataModel.StudyClass = reader.GetValue(1).ToString().Trim().ToUpper();
                        importDataModel.StudentsCount = Convert.ToInt32(reader.GetValue(2));
                        importDataModel.Teacher = Regex.Replace(reader.GetValue(3).ToString().Trim().ToUpper(), "[ ]+", " ");
                        importDataModel.Subject = reader.GetValue(4).ToString().Trim().ToUpper();
                        importDataModel.LessonType = reader.GetValue(5).ToString().Trim().ToUpper();
                        importDataModel.IsSubclassLesson = Convert.ToBoolean(reader.GetValue(6));
                        importDataModel.IsSubweekLesson = Convert.ToBoolean(reader.GetValue(7));
                        importDataModel.IsParallel = Convert.ToBoolean(reader.GetValue(8));
                        importDataModel.IsFlow = Convert.ToBoolean(reader.GetValue(9));
                        importDataModel.FlowStudyClassNames = reader?.GetValue(10)?.ToString()?.Trim()?.ToUpper();
                        importDataModel.ClassShift = reader.GetValue(11).ToString().Trim().ToUpper();
                        importDataModel.EducationForm = reader.GetValue(12).ToString().Trim().ToUpper();

                        try
                        {
                            importDataModel.StudyClassReportingName = reader.GetValue(13)?.ToString()?.Trim()?.ToUpper();
                        }
                        catch
                        {
                            importDataModel.StudyClassReportingName = null;
                            //reader.GetValue(13) столбца нет
                        }

                        dataList.Add(importDataModel);

                        rowIndex++;
                    }
                    catch (Exception ex)
                    {
                        _importProgres.ImportError = true;
                        _importProgres.ImportFinished = true;
                        _importProgres.ErrorMessage = $"Ошибка при проверке данных. Строка: {rowIndex + 1}";
                        throw new Exception(ex.Message);
                    }
                }

                reader.Dispose();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Добавляем подразделения
                    var subdivisionList = dataList
                        .Select(s => s.Subdivision)
                        .Distinct();

                    foreach (var subdivision in subdivisionList)
                    {
                        await _subdivisionService.AddSubdivisionAsync(new SubdivisionDto
                        {
                            Name = subdivision
                        });
                    }


                    //Добавляем преподавателей
                    var teacherList = dataList
                        .Select(s => s.Teacher)
                        .Distinct();

                    foreach (var teacher in teacherList)
                    {
                        await _teacherService.AddTeacherAsync(new TeacherDto
                        {
                            FirstName = teacher?.Split(" ")?.FirstOrDefault(),
                            Name = teacher?.Split(" ")?.Skip(1)?.FirstOrDefault(),
                            MiddleName = teacher?.Split(" ")?.Skip(2)?.FirstOrDefault(),
                        }, versionId);
                    }


                    //Добавляем дисциплины
                    var subjectList = dataList
                        .Select(s => s.Subject)
                        .Distinct();

                    foreach (var subject in subjectList)
                    {
                        await _subjectService.AddSubjectAsync(new SubjectDto
                        {
                            Name = subject
                        });
                    }


                    //Добавляем учебные группы
                    var studyClassGroup = dataList.GroupBy(m => m.StudyClass);

                    foreach (var studyClass in studyClassGroup)
                    {
                        var educationForm = await _context.EducationForms.FirstOrDefaultAsync(e => e.Name == studyClass.FirstOrDefault().EducationForm);

                        var classShift = await _context.ClassShifts.FirstOrDefaultAsync(e => e.Name == studyClass.FirstOrDefault().ClassShift);

                        var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(e => e.Name == studyClass.FirstOrDefault().Subdivision);

                        await _studyClassService.AddStudyClassAsync(new StudyClassDto
                        {
                            Name = studyClass.Key,
                            StudentsCount = studyClass.FirstOrDefault().StudentsCount,
                            EducationFormId = educationForm.Id,
                            ClassShiftId = classShift.Id,
                            SubdivisionId = subdivision.Id
                        });
                    }



                    foreach (var row in dataList)
                    {
                        var subdivision = await _context.Subdivisions.FirstOrDefaultAsync(e => e.Name == row.Subdivision);

                        var srudyClass = await _context.StudyClasses.FirstOrDefaultAsync(s => s.Name == row.StudyClass && s.SubdivisionId == subdivision.Id);

                        var teacher = await _context.Teachers.FirstOrDefaultAsync(s => s.FirstName + " " + s.Name + " " + s.MiddleName == row.Teacher);

                        var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Name == row.Subject);

                        var lessonType = await _context.LessonTypes.FirstOrDefaultAsync(s => s.Name.ToUpper().Substring(0, 3) == row.LessonType.Substring(0, 3));


                        if (!string.IsNullOrEmpty(row.StudyClassReportingName))
                        {
                            var reportingType = await _context.ReportingType
                                .FirstOrDefaultAsync(s => s.Name.ToUpper().Replace(" ", "") == row.StudyClassReportingName.ToUpper().Replace(" ", ""));

                            await _studyClassReportingService.AddStudyClassReportingAsync(new StudyClassReportingDto
                            {
                                StudyClassId = srudyClass.Id,
                                SubjectId = subject.Id,
                                TeacherId = teacher.Id,
                                ReportingTypeId = reportingType.Id,
                                VersionId = versionId,
                            });
                        }

                        if (row.IsFlow)
                        {

                            var flowStudyClassNames = row.FlowStudyClassNames.Split(",");

                            var flowStudyClassIds = await _context.StudyClasses.Where(s => flowStudyClassNames.Contains(s.Name)).Select(s => s.Id).ToListAsync();

                            await _lessonService.AddFlowLessonAsync(new LessonDto
                            {
                                StudyClassId = srudyClass.Id,
                                TeacherId = teacher.Id,
                                SubjectId = subject.Id,
                                LessonTypeId = lessonType.Id,
                                IsParallel = row.IsParallel,
                                IsSubClassLesson = row.IsSubclassLesson,
                                IsSubWeekLesson = row.IsSubweekLesson,
                                FlowStudyClassIds = flowStudyClassIds.ToArray(),
                            }, versionId);
                        }
                        else if (row.IsParallel)
                        {
                            await _lessonService.AddParallelLessonAsync(new LessonDto
                            {
                                StudyClassId = srudyClass.Id,
                                TeacherId = teacher.Id,
                                SubjectId = subject.Id,
                                LessonTypeId = lessonType.Id,
                                IsParallel = row.IsParallel,
                                IsSubClassLesson = row.IsSubclassLesson,
                                IsSubWeekLesson = row.IsSubweekLesson,
                            }, versionId);
                        }
                        else
                        {
                            await _lessonService.AddMainLessonAsync(new LessonDto
                            {
                                StudyClassId = srudyClass.Id,
                                TeacherId = teacher.Id,
                                SubjectId = subject.Id,
                                LessonTypeId = lessonType.Id,
                                IsParallel = row.IsParallel,
                                IsSubClassLesson = row.IsSubclassLesson,
                                IsSubWeekLesson = row.IsSubweekLesson,
                            }, versionId);
                        }

                        _importProgres.CheckedLessonCount++;
                    }

                    transaction.Commit();
                    _importProgres.ImportFinished = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _importProgres.ImportError = true;
                    _importProgres.ImportFinished = true;
                    _importProgres.ErrorMessage = $"Ошибка при импорте данных. Строка: {_importProgres.CheckedLessonCount + 1}";

                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
