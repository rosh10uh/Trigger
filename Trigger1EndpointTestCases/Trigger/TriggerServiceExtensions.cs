using Microsoft.Extensions.DependencyInjection;
using Trigger.BLL.Assessment;
using Trigger.BLL.AssessmentYear;
using Trigger.BLL.ChangePassword;
using Trigger.BLL.Company;
using Trigger.BLL.ContactUs;
using Trigger.BLL.Country;
using Trigger.BLL.Dashboard;
using Trigger.BLL.Department;
using Trigger.BLL.Employee;
using Trigger.BLL.Ethnicity;
using Trigger.BLL.ExcelUpload;
using Trigger.BLL.IndustryType;
using Trigger.BLL.Login;
using Trigger.BLL.Notification;
using Trigger.BLL.OrganizationType;
using Trigger.BLL.Questionnaries;
using Trigger.BLL.Region;
using Trigger.BLL.Role;
using Trigger.BLL.Shared.Interfaces;
using Trigger.BLL.Widget;
using Trigger.DAL;
using Trigger.DAL.Assessment;
using Trigger.DAL.AuthUserDetail;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.ClaimRepo;
using Trigger.DAL.Company;
using Trigger.DAL.Employee;
using Trigger.DAL.EmpProfileRepo;
using Trigger.DAL.ExcelUpload;
using Trigger.DAL.IndustryType;
using Trigger.DAL.NotificationRepo;
using Trigger.DAL.OrganizationType;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
<<<<<<< HEAD
using Trigger.DAL.TeamConfiguration;
=======
using Trigger.DAL.Shared.Sql;
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
using Trigger.DAL.UserDetail;
using Trigger.DAL.UserLoginRepo;
using Trigger.Middleware;
using Trigger.Utility;

namespace Trigger
{
    /// <summary>
    /// Contains method for registering interfaces and classes in service collection
    /// </summary>
    public static class TriggerServiceExtensions
    {
        /// <summary>
        /// Register interfaces and classes in service collection
        /// </summary>
        /// <param name="services"></param>
        public static void AddTriggerService(this IServiceCollection services)
        {
            services.AddScoped<DAL.TriggerContext>();

            services.AddTransient<ILandingMiddlewareManager, LandingMiddlewareManager>();
            services.AddTransient<TriggerResourceReader>();
            services.AddTransient<IClaims, Claims>();
            services.AddScoped<DTO.Connection>();
            services.AddScoped<TriggerCatalogContext>();
            services.AddScoped<IConnectionContext, ConnectionContext>();

            //Employee
            services.AddTransient<Employee>();
            services.AddTransient<EmployeeRepository>();
            services.AddTransient<EmployeeContext>();
            services.AddTransient<UserDetailRepository>();
            services.AddTransient<AuthUserDetailsRepository>();
            services.AddTransient<ClaimsRepository>();
            services.AddTransient<NotificationRepository>();
            services.AddTransient<UserLoginRepository>();
            services.AddTransient<ClaimsCommonRepository>();
            services.AddTransient<EmpProfileRepository>();
            services.AddTransient<CompanyAdminRepository>();

            //Employee
            services.AddTransient<Company>();
            services.AddTransient<CompanyRepository>();
            services.AddTransient<CompanyContext>();
            services.AddTransient<CompanyConfigRepository>();
            services.AddTransient<EmployeeProfile>();
            services.AddTransient<EmployeeCommon>();
            services.AddTransient<EmployeeSendEmail>();
            services.AddTransient<EmployeeForDashboard>();

            //Employee
            services.AddTransient<IndustryType>();
            services.AddTransient<IndustryTypeRepository>();

            //master
            services.AddTransient<Role>();
            services.AddTransient<Country>();
            services.AddTransient<Region>();
            services.AddTransient<Ethnicity>();
            services.AddTransient<AssessmentYear>();

            //Department
            services.AddTransient<Department>();

            //Questionnaries
            services.AddTransient<Questionnaries>();

            //ExcelUpload
            services.AddTransient<ExcelUpload>();
            services.AddTransient<ExcelUploadContext>();
            services.AddTransient<ExcelUploadRepository>();
            services.AddTransient<EmployeeExcelRepository>();
            services.AddTransient<UserExcelRepository>();
            services.AddTransient<ExcelUploadHelper>();
            //Assessment
            services.AddTransient<Assessment>();
            services.AddTransient<AssessmentContext>();

            //Notification
            services.AddTransient<Notification>();

            //Login
            services.AddTransient<Login>();

            //Widgets
            services.AddTransient<Widget>();

            services.AddTransient<Dashboard>();

            services.AddTransient<ChangePassword>();
            services.AddTransient<BackgroundJobRequest>();
            services.AddTransient<TeamBackgroundJobRequest>();

            //SMSService
            services.AddTransient<BLL.SmsService.SmsVerificationCode>();
            services.AddTransient<Utility.ISmsSender, Utility.SmsSender>();
            services.AddTransient<DAL.SmsService.SmsServiceContext>();
            services.AddTransient<DAL.SmsService.SmsServiceRepository>();

            //Dimension Matrix Masters
            //Dimension Master
            services.AddTransient<BLL.DimensionMatrix.Dimension>();
            services.AddTransient<DAL.DimensionMatrix.DimensionRepository>();

            //Dimension Elements(Values)
            services.AddTransient<BLL.DimensionMatrix.DimensionElements>();
            services.AddTransient<DAL.DimensionMatrix.DimensionElementsRepository>();

            //Dimension & Actionwise Permission Configuration
            services.AddTransient<BLL.DimensionMatrix.ActionwisePermission>();
            services.AddTransient<DAL.DimensionMatrix.ActionwisePermissionRepository>();

            services.AddTransient<DAL.Shared.Interfaces.IActionPermission, ActionPermission>();

            services.AddTransient<BLL.TeamConfiguration.TeamConfiguration>();
            services.AddTransient<DAL.TeamConfiguration.TeamConfigurationRepository>();
            services.AddTransient<DAL.TeamConfiguration.TeamManagersRepository>();
            services.AddTransient<DAL.TeamConfiguration.TeamEmployeesRepository>();
            services.AddTransient<DAL.TeamConfiguration.TeamDetailsRepository>();
            services.AddTransient<TeamConfigurationContext>();

            services.AddTransient<TeamDashboard>();
            services.AddTransient<BLL.Dashboard.TeamDashboard>();
            services.AddTransient<DAL.DashBoard.TeamDashboardRepository>();
            services.AddTransient<DAL.DashBoard.TeamListRepository>();
            
            services.AddTransient<DashboardEmployeeList>();
            services.AddTransient<TriggerEmployeeList>();

            // Contact US
            services.AddTransient<ContactUs>();

            //Spark/Classification
            services.AddTransient<BLL.Spark.Classification>();
            services.AddTransient<DAL.Spark.ClassificationRepository>();

            //Spark/EmployeeSpark
            services.AddTransient<BLL.Spark.EmployeeSpark>();
            services.AddTransient<DAL.Spark.EmployeeSparkContext>();
            services.AddTransient<DAL.Spark.EmployeeSparkRepository>();
            services.AddTransient<BLL.Spark.EmployeeSparkSms>();

            //Organization Types
            services.AddTransient<OrganizationType>();
            services.AddTransient<OrganizationTypeRepository>();

            //SqlHelper
            services.AddTransient<SqlHelper>();
        }

    }
}
