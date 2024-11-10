using BL.ServiceInterface;
using BL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BL.Services
{
    public class CustomServices
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<ILessonService, LessonService>();
            services.AddTransient<IMistakeService, MistakeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IStudyClassService, StudyClassService>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddTransient<IClassroomService, ClassroomService>();
            services.AddTransient<IVersionService, VersionService>();
            services.AddTransient<ISubdivisionService, SubdivisionService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IGeneticAlgorithmService, GeneticAlgorithmService>();
            services.AddTransient<IBuildingService, BuildingService>();
            services.AddTransient<IStudyClassReportingService, StudyClassReportingService>();
            services.AddTransient<IClassroomTypeService, ClassroomTypeService>();
        }
    }
}
