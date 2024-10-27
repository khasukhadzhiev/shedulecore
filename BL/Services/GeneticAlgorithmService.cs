using DAL.Entities.Schedule;
using DTL.Dto;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using Version = DAL.Entities.Version;

namespace BL.Services
{
    public class GeneticAlgorithmService : IGeneticAlgorithmService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly ILogger<AccountService> _logger;

        private readonly ScheduleGeneratedProgress _generateScheduleProgress;

        private List<StudyClass> StudyClassList;

        public GeneticAlgorithmService(ScheduleHighSchoolDb context, IHttpContextAccessor httpContextAccessor, ILogger<AccountService> logger, ScheduleGeneratedProgress generateScheduleProgress)
        {
            _context = context;
            _logger = logger;
            _generateScheduleProgress = generateScheduleProgress;
        }

        ///<inheritdoc/>
        public async Task<ScheduleGeneratedProgress> GetScheduleGenerateProgressAsync()
        {
            return await Task.Run(() =>
            {
                return _generateScheduleProgress;
            });
        }

        ///<inheritdoc/>
        public async Task StopScheduleGenerateAsync()
        {
            await Task.Run(() =>
            {
                _generateScheduleProgress.End = true;
            });
        }

        ///<inheritdoc/>
        public async Task SaveScheduleWithMistakesAsync()
        {
            await Task.Run(() =>
            {
                _generateScheduleProgress.SaveWithMistakes = true;
            });
        }

        ///<inheritdoc/>
        public async Task GenerateScheduleAsync(GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            if (_generateScheduleProgress.Start)
            {
                throw new Exception("Генерация уже запущена!");
            }

            DateTime startTime = DateTime.Now;

            bool generateFinish = false;

            _generateScheduleProgress.Start = true;
            _generateScheduleProgress.End = false;
            _generateScheduleProgress.SaveWithMistakes = false;
            _generateScheduleProgress.Generation = 0;
            _generateScheduleProgress.Message = "";
            _generateScheduleProgress.SpentTime = "";
            _generateScheduleProgress.MistakeType = new List<string>();

            try
            {
                int versionId = geneticAlgorithmDataDto.VersionId;
                int classShiftId = geneticAlgorithmDataDto.ClassShiftId;
                var version = _context.Versions.FirstOrDefault(v => v.Id == versionId);

                List<Lesson> lessons = new List<Lesson>();

                string selectLessonTypeQuery = "(IsSubClassLesson == false && IsSubWeekLesson == false)";

                selectLessonTypeQuery += version.UseSubClass ? " || (IsSubClassLesson == true)" : "";

                selectLessonTypeQuery += version.UseSubWeek ? " || (IsSubWeekLesson == true)" : "";

                lessons = await _context.Lessons.AsQueryable().AsTracking()
                                    .Include(l => l.Teacher)
                                    .Include(l => l.StudyClass)
                                        .ThenInclude(s => s.ClassShift)
                                    .Include(l => l.StudyClass)
                                        .ThenInclude(s => s.EducationForm)
                                    .Include(v => v.Version)
                                    .Where(selectLessonTypeQuery)
                                    .Where(l => l.VersionId == versionId)
                                    .Where(l => l.StudyClass.ClassShiftId == classShiftId)
                                    .OrderByDescending(l => l.StudyClass.EducationFormId)
                                        .ThenBy(l => l.FlowId)
                                    .ToListAsync();

                if (lessons.Count == 0)
                {
                    _generateScheduleProgress.Start = false;
                    _generateScheduleProgress.SaveWithMistakes = false;
                    _generateScheduleProgress.End = true;
                    throw new Exception("Занятий не найдено!");
                }



                List<Lesson>[] initialPopulationArr = new List<Lesson>[]
                {
                 lessons.Select(l => (Lesson)l.Clone()).ToList(),
                 lessons.Select(l => (Lesson)l.Clone()).ToList(),
                 lessons.Select(l => (Lesson)l.Clone()).ToList(),
                 lessons.Select(l => (Lesson)l.Clone()).ToList(),
                 lessons.Select(l => (Lesson)l.Clone()).ToList(),
                };

                RandomSetLessonListPosition(initialPopulationArr[0], version, geneticAlgorithmDataDto);
                RandomSetLessonListPosition(initialPopulationArr[1], version, geneticAlgorithmDataDto);
                RandomSetLessonListPosition(initialPopulationArr[2], version, geneticAlgorithmDataDto);
                RandomSetLessonListPosition(initialPopulationArr[3], version, geneticAlgorithmDataDto);
                RandomSetLessonListPosition(initialPopulationArr[4], version, geneticAlgorithmDataDto);

                StudyClassList = initialPopulationArr[0].Select(l => l.StudyClass).Distinct().ToList();

                SetLessonWeight(initialPopulationArr, version, geneticAlgorithmDataDto);

                List<Lesson> bestGenom = initialPopulationArr.OrderBy(p => p.Sum(l => l.Weight)).First().Select(l => (Lesson)l.Clone()).ToList();

                double bestGenomMistakeCount = bestGenom.Sum(l => l.Weight);

                _generateScheduleProgress.End = Fitness(initialPopulationArr, lessons, bestGenom);

                List<Lesson>[] population = initialPopulationArr;

                while (!_generateScheduleProgress.End)
                {
                    _generateScheduleProgress.MistakeType.RemoveRange(0, _generateScheduleProgress.MistakeType.Count);

                    var nextPopulation = Crossover(population, version, geneticAlgorithmDataDto);
                    var selectedPopulation = Selection(nextPopulation);

                    if (_generateScheduleProgress.Generation % 200 == 0)
                    {
                        Mutation(selectedPopulation, version, geneticAlgorithmDataDto);
                    }

                    SetLessonWeight(selectedPopulation, version, geneticAlgorithmDataDto);

                    var mistakeSumArr = new double[]{
                        selectedPopulation[0].Sum(l => l.Weight),
                        selectedPopulation[1].Sum(l => l.Weight),
                        selectedPopulation[2].Sum(l => l.Weight),
                        selectedPopulation[3].Sum(l => l.Weight),
                        selectedPopulation[4].Sum(l => l.Weight),
                    };

                    if (bestGenomMistakeCount > mistakeSumArr.Min())
                    {
                        bestGenom = selectedPopulation.OrderBy(p => p.Sum(l => l.Weight)).First().Select(l => (Lesson)l.Clone()).ToList();
                        bestGenomMistakeCount = bestGenom.Sum(l => l.Weight);
                    }

                    if (selectedPopulation.Any(sp => sp.Sum(l => l.Weight) == 0) || _generateScheduleProgress.SaveWithMistakes)
                    {
                        generateFinish = Fitness(selectedPopulation, lessons, bestGenom);
                    }

                    population = selectedPopulation;

                    _generateScheduleProgress.Generation++;

                    string mistakeSumString = $"Поколение {_generateScheduleProgress.Generation}  --  " + String.Join(",", mistakeSumArr);

                    mistakeSumString += $" " +
                        $"| Лучшее поколение: {bestGenomMistakeCount} " +
                        $"| Текущее поколение:  {mistakeSumArr.Min()} " +
                        $"| Виды ошибок: {string.Join(", ", _generateScheduleProgress.MistakeType.OrderBy(m => m))}.";

                    _generateScheduleProgress.Message = mistakeSumString;

                    TimeSpan time = DateTime.Now - startTime;

                    _generateScheduleProgress.SpentTime = $"Время выполнения: {time.Hours} ч. : {time.Minutes} м. : {time.Seconds} с.";

                    if (generateFinish)
                    {
                        _generateScheduleProgress.SpentTime = $"Расписание сгенерировано!!! Время выполнения: {time.Hours} ч. : {time.Minutes} м. : {time.Seconds} с.";
                        Debug.WriteLine(_generateScheduleProgress.SpentTime);
                        break;
                    }

                    Debug.WriteLine(_generateScheduleProgress.Message);
                }
            }
            finally
            {
                _generateScheduleProgress.Start = false;
                _generateScheduleProgress.SaveWithMistakes = false;
                _generateScheduleProgress.End = true;
            }
        }

        /// <summary>
        /// Фитнес функция для проверки пригодности решения
        /// </summary>
        /// <param name="lessonsArr">Популяция</param>
        /// <returns></returns>
        private bool Fitness(List<Lesson>[] lessonsArr, List<Lesson> lessons, List<Lesson> bestPopulation)
        {
            bool fitnessResult = false;

            foreach (var fitnessLessons in lessonsArr)
            {
                if (fitnessLessons.Sum(l => l.Weight) == 0)
                {
                    foreach (var lesson in lessons)
                    {
                        lesson.RowIndex = fitnessLessons.FirstOrDefault(l => l.Id == lesson.Id).RowIndex;
                        lesson.ColIndex = fitnessLessons.FirstOrDefault(l => l.Id == lesson.Id).ColIndex;
                    }

                    _context.SaveChanges();

                    fitnessResult = true;
                    return fitnessResult;
                }
            }


            if (_generateScheduleProgress.SaveWithMistakes)
            {
                foreach (var lesson in lessons)
                {
                    lesson.RowIndex = bestPopulation.FirstOrDefault(l => l.Id == lesson.Id).RowIndex;
                    lesson.ColIndex = bestPopulation.FirstOrDefault(l => l.Id == lesson.Id).ColIndex;
                }

                fitnessResult = true;

                //bool hasChanges = _context.ChangeTracker.HasChanges(); // should be true
                //int updates = _context.SaveChanges();                  // should be > 0

                _context.SaveChanges();
            }

            return fitnessResult;
        }

        /// <summary>
        /// Функция отбора
        /// </summary>
        /// <param name="lessonsArr">Популяция</param>
        /// <returns></returns>
        private List<Lesson>[] Selection(List<Lesson>[] lessonsArr)
        {
            if (lessonsArr.Length < 5)
            {
                return lessonsArr;
            }

            var newLessonArr = lessonsArr.OrderBy(i => i.Sum(l => l.Weight)).Take(5).ToArray();

            return newLessonArr;
        }

        /// <summary>
        /// Скрещивание
        /// </summary>
        /// <param name="lessonsArr">Популяция</param>
        private List<Lesson>[] Crossover(List<Lesson>[] lessonsArr, Version version, GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            List<List<Lesson>> newLessonPopulation = new List<List<Lesson>>();

            Parallel.For(0, lessonsArr.Length, (i) =>
            {
                Parallel.For(i + 1, lessonsArr.Length, (j) =>
                    {
                        ConcurrentBag<Lesson> gen = new ConcurrentBag<Lesson>();

                        //Parallel.For(0, lessonsArr[0].Count, k =>
                        //{
                        //    var lesson_i = lessonsArr[i][k];
                        //    var lesson_j = lessonsArr[j][k];

                        //    if (lesson_i.Weight > lesson_j.Weight)
                        //    {
                        //        if (lesson_j.Weight > 0)
                        //        {
                        //            RandomSetLessonPosition(lesson_j, version, lessonsArr[j].OrderBy(l => l.StudyClassId).ThenBy(l => l.TeacherId).ToList(), geneticAlgorithmDataDto);
                        //        }
                        //        gen.Add((Lesson)lesson_j.Clone());
                        //    }
                        //    else
                        //    {
                        //        if (lesson_i.Weight > 0)
                        //        {
                        //            RandomSetLessonPosition(lesson_i, version, lessonsArr[i].OrderBy(l => l.StudyClassId).ThenBy(l => l.TeacherId).ToList(), geneticAlgorithmDataDto);
                        //        }
                        //        gen.Add((Lesson)lesson_i.Clone());
                        //    }
                        //});

                        Task.Run(() =>
                        {
                            for (int k = 0; k < lessonsArr[0].Count; k++)
                            {
                                var lesson_i = lessonsArr[i][k];
                                var lesson_j = lessonsArr[j][k];

                                if (lesson_i.Weight > lesson_j.Weight)
                                {
                                    if (lesson_j.Weight > 0)
                                    {
                                        RandomSetLessonPosition(lesson_j, version, lessonsArr[j].OrderBy(l => l.StudyClassId).ThenBy(l => l.TeacherId).ToList(), geneticAlgorithmDataDto);
                                    }

                                    gen.Add((Lesson)lesson_j.Clone());
                                }
                                else
                                {
                                    if (lesson_i.Weight > 0)
                                    {
                                        RandomSetLessonPosition(lesson_i, version, lessonsArr[i].OrderBy(l => l.StudyClassId).ThenBy(l => l.TeacherId).ToList(), geneticAlgorithmDataDto);
                                    }

                                    gen.Add((Lesson)lesson_i.Clone());
                                }
                            }
                        }).Wait();

                        var genFlows = gen.Where(g => g.FlowId != null).Distinct();

                        foreach (var genFlow in genFlows)
                        {
                            var flows = gen.Where(g => g.FlowId == genFlow.FlowId);

                            var flowRowIndex = flows.Where(g => g.Weight == flows.Min(m => m.Weight)).FirstOrDefault().RowIndex;

                            foreach (var flow in flows)
                            {
                                flow.RowIndex = flowRowIndex;
                            }
                        }

                        newLessonPopulation.Add(gen.ToList());
                    });
            });

            return newLessonPopulation.ToArray();
        }

        /// <summary>
        /// Мутация популяции
        /// </summary>
        /// <param name="lessonsArr">Массив списков занятий</param>
        /// <param name="version">Версия расписания</param>
        /// <param name="geneticAlgorithmDataDto">Данные выполнения</param>
        private void Mutation(List<Lesson>[] lessonsArr, Version version, GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            Parallel.ForEach(lessonsArr, lessons =>
            {
                var rnd = RandomNumberGenerator.GetInt32(0, 3);

                if (rnd == 0)
                {
                    Parallel.ForEach(lessons, lesson =>
                    {
                        var rnd2 = RandomNumberGenerator.GetInt32(0, 3);

                        if (rnd2 == 0)
                        {
                            RandomSetLessonPosition(lesson, version, lessons.OrderBy(l => l.StudyClassId).ThenBy(l => l.TeacherId).ToList(), geneticAlgorithmDataDto);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Случайная инициализация популяции
        /// </summary>
        /// <param name="lessons">Список занятий</param>
        /// <param name="version">Версия расписания</param>
        private void RandomSetLessonListPosition(List<Lesson> lessons, Version version, GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            List<int?> setedFlow = new List<int?>(lessons.Where(l => l.FlowId != null).Count());

            Parallel.ForEach(lessons, (lesson) =>
            {
                if (setedFlow.Contains(lesson.FlowId))
                {
                    return;
                }

                var educationFormDaysTeacher = geneticAlgorithmDataDto.EducationFormData
                    .Where(e => e.Id == lesson.StudyClass.EducationFormId)
                    .FirstOrDefault().WeekDays
                    .Intersect(lesson.Teacher.WeekDays)
                    .ToList();

                if (educationFormDaysTeacher.Count == 0 || TeacherOverBusy(lessons, lesson, educationFormDaysTeacher))
                {
                    _generateScheduleProgress.Start = false;
                    _generateScheduleProgress.SaveWithMistakes = false;
                    _generateScheduleProgress.End = true;

                    throw new Exception($"Невозможно создать расписание для преподавателя: {lesson.Teacher.FirstName} {lesson.Teacher.Name}.");
                }

                var educationFormDaysStudyClass = geneticAlgorithmDataDto.EducationFormData
                    .Where(e => e.Id == lesson.StudyClass.EducationFormId)
                    .FirstOrDefault().WeekDays
                    .ToList();

                if (StudyClassOverBusy(lessons, lesson, educationFormDaysStudyClass, version))
                {
                    _generateScheduleProgress.Start = false;
                    _generateScheduleProgress.SaveWithMistakes = false;
                    _generateScheduleProgress.End = true;

                    throw new Exception($"Невозможно создать расписание для группы: {lesson.StudyClass.Name}.");
                }


                bool rowIndexIsFind = false;

                int rowIndex = 0;

                var subClassLessonsBySubject = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.IsSubClassLesson && l.SubjectId == lesson.SubjectId).ToList();

                var teacherBusyLessons = lessons.Where(l => l.TeacherId == lesson.TeacherId && l.RowIndex != null).Select(l => l.RowIndex.Value).Distinct().ToList();

                bool colIndexIsUnit = subClassLessonsBySubject.Where(l => l.ColIndex == 0).Count() > subClassLessonsBySubject.Where(l => l.ColIndex == 1).Count();

                for (int d = 0; d < educationFormDaysTeacher.Count && !rowIndexIsFind; d++)
                {
                    for (int l = 0; l < lesson.Teacher.LessonNumbers.Length && !rowIndexIsFind; l++)
                    {
                        int lessonNumber = SetLessonNumber(version, lesson, l);

                        int dayNumber = version.UseSubWeek ? educationFormDaysTeacher[d] * 2 * version.MaxLesson : educationFormDaysTeacher[d] * version.MaxLesson;

                        rowIndex = dayNumber + lessonNumber;

                        if (lesson.RowIndex == rowIndex || teacherBusyLessons.Contains(rowIndex))
                        {
                            continue;
                        }

                        bool isAlreadyAdded = true;

                        if (lesson.IsSubClassLesson)
                        {
                            lesson.ColIndex = colIndexIsUnit ? 1 : 0;

                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId
                            && ((l.RowIndex == rowIndex && !l.IsSubClassLesson) || (l.RowIndex == rowIndex && l.ColIndex == lesson.ColIndex))).Any();
                        }
                        else
                        {
                            lesson.ColIndex = 0;
                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.RowIndex == rowIndex).Any();
                        }

                        if (lesson.IsSubWeekLesson && isAlreadyAdded)
                        {
                            var rowIndexSecond = rowIndex >= 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.IsSubWeekLesson && l.RowIndex == rowIndexSecond).Any();

                            rowIndex = !isAlreadyAdded ? rowIndexSecond : rowIndex;
                        }

                        if (!isAlreadyAdded)
                        {
                            rowIndexIsFind = true;
                        }
                    }
                }

                if (!rowIndexIsFind)
                {
                    int randLessonIndex = RandomNumberGenerator.GetInt32(0, lesson.Teacher.LessonNumbers.Length);
                    int lessonNumber = SetLessonNumber(version, lesson, randLessonIndex);

                    int randomDay = educationFormDaysTeacher[RandomNumberGenerator.GetInt32(0, educationFormDaysTeacher.Count)];
                    int dayNumber = version.UseSubWeek ? randomDay * 2 * version.MaxLesson : randomDay * version.MaxLesson;

                    rowIndex = dayNumber + lessonNumber;

                    if (lesson.IsSubClassLesson)
                    {
                        lesson.ColIndex = colIndexIsUnit ? 1 : 0;
                    }
                    else
                    {
                        lesson.ColIndex = 0;
                    }

                    if (lesson.IsSubWeekLesson)
                    {
                        var rowIndexSecond = rowIndex >= 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

                        var isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.IsSubWeekLesson && l.RowIndex == rowIndexSecond).Any();

                        rowIndex = !isAlreadyAdded ? rowIndexSecond : rowIndex;
                    }
                }

                lesson.RowIndex = rowIndex;

                if (lesson.FlowId != null && !setedFlow.Contains(lesson.FlowId))
                {
                    SetFlowRowIndex(lessons, lesson);

                    setedFlow.Add(lesson.FlowId);
                }
            });
        }

        /// <summary>
        /// Случайная инициализация популяции
        /// </summary>
        /// <param name="lessons">Список занятий</param>
        /// <param name="version">Версия расписания</param>
        private void RandomSetLessonPosition(Lesson lesson, Version version, List<Lesson> lessons, GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            var teacherBusyLessons = lessons.Where(l => l.TeacherId == lesson.TeacherId && l.RowIndex != null).Select(l => l.RowIndex.Value).Distinct().ToList();

            var subClassLessonsBySubject = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.IsSubClassLesson && l.SubjectId == lesson.SubjectId).ToList();

            bool colIndexIsUnit = subClassLessonsBySubject.Where(l => l.ColIndex == 0).Count() > subClassLessonsBySubject.Where(l => l.ColIndex == 1).Count();

            Task.Run(() =>
            {
                List<Lesson> rowIndexCheckLessonList = new List<Lesson>();

                var educationFormDays = geneticAlgorithmDataDto.EducationFormData
                    .Where(e => e.Id == lesson.StudyClass.EducationFormId)
                    .FirstOrDefault().WeekDays
                    .Intersect(lesson.Teacher.WeekDays)
                    .ToList();

                if (educationFormDays.Count == 0)
                {
                    _generateScheduleProgress.Start = false;
                    _generateScheduleProgress.SaveWithMistakes = false;
                    _generateScheduleProgress.End = true;

                    throw new Exception($"Невозможно создать расписание для {lesson.Teacher.FirstName} {lesson.Teacher.Name}. Группа {lesson.StudyClass.Name}");
                }

                bool rowIndexIsFind = false;

                int rowIndex = 0;

                for (int d = 0; d < educationFormDays.Count && !rowIndexIsFind; d++)
                {
                    int dayNumber = version.UseSubWeek ? educationFormDays[d] * 2 * version.MaxLesson : educationFormDays[d] * version.MaxLesson;

                    for (int l = 0; l < lesson.Teacher.LessonNumbers.Length && !rowIndexIsFind; l++)
                    {
                        int lessonNumber = SetLessonNumber(version, lesson, l);

                        rowIndex = dayNumber + lessonNumber;

                        if (lesson.RowIndex == rowIndex || teacherBusyLessons.Contains(rowIndex))
                        {
                            continue;
                        }

                        bool isAlreadyAdded = true;

                        if (lesson.IsSubClassLesson)
                        {
                            lesson.ColIndex = colIndexIsUnit ? 1 : 0;

                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0
                            && ((l.RowIndex == rowIndex && !l.IsSubClassLesson) || (l.RowIndex == rowIndex && l.ColIndex == lesson.ColIndex))).Any();
                        }
                        else
                        {
                            lesson.ColIndex = 0;
                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0 && l.RowIndex == rowIndex).Any();
                        }

                        if (lesson.IsSubWeekLesson && isAlreadyAdded)
                        {
                            var rowIndexSecond = rowIndex >= 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

                            isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0 && l.IsSubWeekLesson && l.RowIndex == rowIndexSecond).Any();

                            rowIndex = !isAlreadyAdded ? rowIndexSecond : rowIndex;
                        }

                        if (!isAlreadyAdded)
                        {
                            rowIndexIsFind = true;
                        }
                    }
                }

                if (!rowIndexIsFind)
                {
                    int randLessonIndex = RandomNumberGenerator.GetInt32(0, lesson.Teacher.LessonNumbers.Length);
                    int lessonNumber = SetLessonNumber(version, lesson, randLessonIndex);

                    int randomDay = educationFormDays[RandomNumberGenerator.GetInt32(0, educationFormDays.Count)];
                    int dayNumber = version.UseSubWeek ? randomDay * 2 * version.MaxLesson : randomDay * version.MaxLesson;

                    rowIndex = dayNumber + lessonNumber;

                    bool isAlreadyAdded = true;

                    if (lesson.IsSubClassLesson)
                    {
                        lesson.ColIndex = colIndexIsUnit ? 1 : 0;

                        isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0
                        && ((l.RowIndex == rowIndex && !l.IsSubClassLesson) || (l.RowIndex == rowIndex && l.ColIndex == lesson.ColIndex))).Any();
                    }
                    else
                    {
                        lesson.ColIndex = 0;
                        isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0 && l.RowIndex == rowIndex).Any();
                    }

                    if (lesson.IsSubWeekLesson && isAlreadyAdded)
                    {
                        var rowIndexSecond = rowIndex >= 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

                        isAlreadyAdded = lessons.Where(l => l.StudyClassId == lesson.StudyClassId && l.Weight == 0 && l.IsSubWeekLesson && l.RowIndex == rowIndexSecond).Any();

                        rowIndex = !isAlreadyAdded ? rowIndexSecond : rowIndex;
                    }
                }

                lesson.RowIndex = rowIndex;

                if (lesson.FlowId != null)
                {
                    SetFlowRowIndex(lessons, lesson);
                }

            }).Wait();
        }

        /// <summary>
        /// Расчет весов занятий
        /// </summary>
        /// <param name="lessonsArr">Массив списков занятий</param>
        /// <param name="version">Версия расписания</param>
        /// <param name="geneticAlgorithmDataDto">Данные выполнения</param>
        private void SetLessonWeight(List<Lesson>[] lessonsArr, Version version, GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            Parallel.ForEach(lessonsArr, (lessons) =>
            {
                Parallel.ForEach(lessons, (lesson) =>
                {
                    var rowIndexSecond = lesson.RowIndex >= 0 && lesson.RowIndex % 2 != 0 ? lesson.RowIndex - 1 : lesson.RowIndex + 1;

                    var conditionTeacherString = new StringBuilder();

                    if (!lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)//Целая группа
                    {
                        if (version.UseSubWeek)
                        {
                            conditionTeacherString.Append($"(RowIndex == {lesson.RowIndex} || RowIndex == {rowIndexSecond})");
                        }
                        else
                        {
                            conditionTeacherString.Append($"(RowIndex == {lesson.RowIndex})");
                        }

                        conditionTeacherString.Append($" AND (((FlowId != null || {lesson.FlowId != null}) AND FlowId != {lesson.FlowId ?? -1}) || (FlowId == null AND {lesson.FlowId == null}))");
                        conditionTeacherString.Append($" AND (TeacherId == {lesson.TeacherId} || StudyClassId == {lesson.StudyClassId})");
                    }
                    else if (!lesson.IsSubClassLesson && lesson.IsSubWeekLesson)//Целая группа по одной неделе
                    {
                        conditionTeacherString.Append($"(RowIndex == {lesson.RowIndex} || (IsSubWeekLesson == {false} && RowIndex == {rowIndexSecond}))");
                        conditionTeacherString.Append($" AND (((FlowId != null || {lesson.FlowId != null}) AND FlowId != {lesson.FlowId ?? -1}) || (FlowId == null AND {lesson.FlowId == null}))");
                        conditionTeacherString.Append($" AND (TeacherId == {lesson.TeacherId} || StudyClassId == {lesson.StudyClassId})");
                    }
                    else if (lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)//Подгруппа в каждую неделю
                    {
                        if (version.UseSubWeek)
                        {
                            conditionTeacherString.Append($"((RowIndex == {lesson.RowIndex} || RowIndex == {rowIndexSecond}) AND (ColIndex == {lesson.ColIndex}) AND (StudyClassId == {lesson.StudyClassId}))");
                            conditionTeacherString.Append($" || ((RowIndex == {lesson.RowIndex} || RowIndex == {rowIndexSecond}) AND (TeacherId == {lesson.TeacherId}))");
                        }
                        else
                        {
                            conditionTeacherString.Append($"((RowIndex == {lesson.RowIndex}) AND (ColIndex == {lesson.ColIndex}) AND (StudyClassId == {lesson.StudyClassId}))");
                            conditionTeacherString.Append($" || ((RowIndex == {lesson.RowIndex}) AND (TeacherId == {lesson.TeacherId}))");
                        }
                    }
                    else//Подгруппа по одной неделе
                    {
                        conditionTeacherString.Append($"((RowIndex == {lesson.RowIndex} || (IsSubWeekLesson == {false} && RowIndex == {rowIndexSecond})) AND (TeacherId == {lesson.TeacherId}))");
                        conditionTeacherString.Append($" || (RowIndex == {lesson.RowIndex} AND ColIndex == {lesson.ColIndex} AND (StudyClassId == {lesson.StudyClassId} || TeacherId == {lesson.TeacherId}))");
                    }

                    var mistakeLessonList = lessons.AsQueryable()
                        .Where(conditionTeacherString.ToString())
                        .Where(l => l.Id != lesson.Id
                            && l.StudyClass.ClassShiftId == lesson.StudyClass.ClassShiftId
                            && l.VersionId == version.Id
                        ).ToList();

                    foreach (var item in mistakeLessonList)
                    {
                        item.Weight += 1;
                    }

                    lesson.Weight = mistakeLessonList.Count;

                    if (lesson.Weight > 0)
                    {
                        if (!_generateScheduleProgress.MistakeType.Contains("Накладки"))
                        {
                            _generateScheduleProgress.MistakeType.Add("Накладки");
                        }
                    }

                    if (!geneticAlgorithmDataDto.AllowEmptyLesson)
                    {
                        //Проверка на одиночные пары в день
                        var singleLessons = lessons
                            .Where(l => l.StudyClassId == lesson.StudyClassId)
                            .GroupBy(l => l.LessonDay)
                            .Where(g => g.Count() < 2)
                            .SelectMany(g => g)
                            .ToList();

                        foreach (var item in singleLessons)
                        {
                            if (item.FlowId != null)
                            {
                                continue;
                            }

                            item.Weight += 1;

                            if (!_generateScheduleProgress.MistakeType.Contains("Одиночные пары в день"))
                            {
                                _generateScheduleProgress.MistakeType.Add("Одиночные пары в день");
                            }
                        }
                    }

                });

                //Проверка на одиночные пары подгрупп
                if (version.UseSubClass && lessons.Any(l => l.IsSubClassLesson) && !geneticAlgorithmDataDto.AllowEmptyLesson)
                {
                    Parallel.ForEach(StudyClassList, studyClass =>
                    {
                        var subClassLessonCount = lessons
                            .Where(l => l.IsSubClassLesson && l.StudyClassId == studyClass.Id).Count();

                        if (subClassLessonCount % 2 == 0)
                        {
                            var subClassLessons = lessons
                                    .Where(l => l.IsSubClassLesson && l.StudyClassId == studyClass.Id)
                                    .GroupBy(l => l.RowIndex)
                                    .Where(g => g.Count() < 2)
                                    .SelectMany(g => g)
                                    .ToList();

                            foreach (var item in subClassLessons)
                            {
                                item.Weight += 1;

                                if (!_generateScheduleProgress.MistakeType.Contains("Одиночные пары подгрупп"))
                                {
                                    _generateScheduleProgress.MistakeType.Add("Одиночные пары подгрупп");
                                }
                            }
                        }
                        else
                        {
                            _generateScheduleProgress.Start = false;
                            _generateScheduleProgress.SaveWithMistakes = false;
                            _generateScheduleProgress.End = true;

                            throw new Exception($"Невозможно создать расписание для {studyClass.Name}. Нечетное количество занятий по подгруппам.");
                        }
                    });
                }

                //Условие допуска окон в расписании
                if (!geneticAlgorithmDataDto.AllowEmptyLesson)
                {
                    foreach (var studyClass in StudyClassList)
                    {
                        var studyClassLessons = lessons
                            .Where(l => l.StudyClassId == studyClass.Id);

                        int lessonDaysCount = version.UseSubWeek ? 7 : 6;

                        for (int i = 0; i < lessonDaysCount; i++)
                        {
                            var lessonListByDay = studyClassLessons.Where(l => l.LessonDay == i).OrderBy(l => l.RowIndex).ToArray();

                            for (int j = lessonListByDay.Length - 1; j >= 1 && lessonListByDay.Length < 4; j--)
                            {
                                var indexDifference = lessonListByDay[j].LessonNumber - lessonListByDay[j - 1].LessonNumber;

                                if (indexDifference.Value > 0)
                                {
                                    if (lessonListByDay[j].FlowId == null)
                                    {
                                        //if (_generateScheduleProgress.MistakeType.Contains("Накладки"))
                                        //{
                                        lessonListByDay[j].RowIndex = version.UseSubWeek ? lessonListByDay[j - 1].RowIndex + 2 : lessonListByDay[j - 1].RowIndex + 1;
                                        //}
                                        //else
                                        //{
                                        //    lessonListByDay[j].Weight = 1;
                                        //}

                                        if (!_generateScheduleProgress.MistakeType.Contains("Окна"))
                                        {
                                            _generateScheduleProgress.MistakeType.Add("Окна");
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Назначить позицию потока
        /// </summary>
        /// <param name="lessons">Список зянятий</param>
        /// <param name="lesson">Занятия</param>
        private void SetFlowRowIndex(List<Lesson> lessons, Lesson lesson)
        {
            var flows = lessons.Where(l => l.FlowId == lesson.FlowId);

            foreach (var flowLesson in flows)
            {
                flowLesson.RowIndex = lesson.RowIndex;
            }
        }

        /// <summary>
        /// Проверка переполненности преподавателя
        /// </summary>
        /// <param name="lessons"></param>
        /// <param name="lesson"></param>
        /// <param name="educationFormDays"></param>
        /// <returns></returns>
        private bool TeacherOverBusy(List<Lesson> lessons, Lesson lesson, List<int> educationFormDays)
        {
            var teacherLessons = lessons.Where(l => l.TeacherId == lesson.TeacherId && l.StudyClass.EducationFormId == lesson.StudyClass.EducationFormId);

            var teacherFlowLessonsCount = teacherLessons.Where(l => l.FlowId == null).Select(l => l.FlowId).Distinct().Count();//количество потоков

            var fullLessonCount = teacherLessons.Where(l => !l.IsSubWeekLesson && l.StudyClass.EducationFormId == lesson.StudyClass.EducationFormId  && l.FlowId == null).Count(); // количество полных занятий без учета потоков

            var subWeekLessonCount = teacherLessons.Where(l => l.FlowId == null).Count() - fullLessonCount;

            var availablePositionCount = lesson.Teacher.LessonNumbers.Length * educationFormDays.Count();

            var result = availablePositionCount - fullLessonCount - teacherFlowLessonsCount - (subWeekLessonCount / 2) < 0;

            return result;
        }


        /// <summary>
        /// Проверка переполненности группы
        /// </summary>
        /// <param name="lessons"></param>
        /// <param name="lesson"></param>
        /// <param name="educationFormDays"></param>
        /// <returns></returns>
        private bool StudyClassOverBusy(List<Lesson> lessons, Lesson lesson, List<int> educationFormDays, Version version)
        {
            var studyClassLessons = lessons.Where(l => l.StudyClassId == lesson.StudyClassId);

            var teacherFlowLessonsCount = studyClassLessons.Where(l => l.FlowId == null).Select(l => l.FlowId).Distinct().Count();//количество потоков

            var fullLessonCount = studyClassLessons.Where(l => !l.IsSubWeekLesson && l.FlowId == null).Count();

            var subWeekLessonCount = studyClassLessons.Where(l => l.FlowId == null).Count() - fullLessonCount;

            var availablePositionCount = version.UseSubWeek ? educationFormDays.Count() * version.MaxLesson * 2 : educationFormDays.Count() * version.MaxLesson;

            var result = availablePositionCount - fullLessonCount - teacherFlowLessonsCount - (subWeekLessonCount / 2) < 0;

            return result;
        }

        /// <summary>
        /// Высислить номер индекс номера занятия по номеру занятия
        /// </summary>
        /// <param name="version">Версия расписания</param>
        /// <param name="lesson">Занятие</param>
        /// <param name="numberIndex">Номер занятия</param>
        /// <returns></returns>
        private int SetLessonNumber(Version version, Lesson lesson, int numberIndex)
        {
            int result;

            if (version.UseSubWeek)
            {
                int lessonNumber = lesson.Teacher.LessonNumbers[numberIndex] * 2;

                result = lessonNumber - 2;
            }
            else
            {
                result = lesson.Teacher.LessonNumbers[numberIndex] - 1;
            }

            return result >= 0 ? result : 0;
        }
    }
}
