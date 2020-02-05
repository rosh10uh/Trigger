using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System;
using Trigger.DAL.Company;
using Trigger.DAL.Employee;
using Trigger.DAL.Questionnaries;
using Trigger.DAL.IndustryType;
using Trigger.DTO;
using Trigger.DAL.Role;
using Trigger.DAL.Country;
using Trigger.DAL.Region;
using Trigger.DAL.Ethnicity;
using Trigger.DAL.AssessmentYear;
using Trigger.DAL.DashBoard;
using Trigger.DAL.Department;
using Trigger.DAL.ExcelUpload;
using Trigger.DAL.UserDetail;
using Trigger.DAL.AuthUserDetail;
using Trigger.DAL.ClaimRepo;
using Trigger.DAL.NotificationRepo;
using Trigger.DAL.UserLoginRepo;
using Trigger.DAL.Assessment;
using Trigger.DAL.Login;
using Trigger.DAL.EmpProfileRepo;
using Trigger.DAL.WidgetLibraryRepo;
using Trigger.DAL.ChangePassword;
using Trigger.DAL.EmpSalary;
using Trigger.DAL.SmsService;
using Trigger.DAL.EmployeeProfile;
using Trigger.DAL.DimensionMatrix;
using Trigger.DAL.EmailTemplate;
using Trigger.DAL.EmployeeList;
using Trigger.DAL.TeamConfiguration;
using Trigger.DAL.Spark;
using Trigger.DAL.OrganizationType;

namespace Trigger.DAL
{
    public class TriggerContext : DaoContext
    {
        public TriggerContext(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public EmployeeRepository EmployeeRepository { get; set; }

        public UserDetailRepository UserDetailRepository { get; set; }

        public NotificationRepository NotificationRepository { get; set; }

        public UserLoginRepository UserLoginRepository { get; set; }

        public EmpProfileRepository EmpProfileRepository { get; set; }

        public EmployeeSalaryRepository EmpSalaryRepository { get; set; }

        public EmployeeProfileRepository EmployeeProfileRepository { get; set; }

        [QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
        public ExcelUploadRepository ExcelUploadRepository { get; set; }

        public EmployeeCsvRepository EmployeeCsvRepository { get; set; }
        public AuthUserCsvRepository AuthUserCsvRepository { get; set; }

        public UserCsvRepository UserCsvRepository { get; set; }

        [QueryPath("Trigger.DAL.Query.Department.Department")]
        public DepartmentRepository DepartmentRepository { get; set; }

        [QueryPath("Trigger.DAL.Query.Questionnaries.Questionnaries")]
        public QuestionnariesRepository QuestionnariesRepository { get; set; }

        public AssessmentYearRepository AssessmentYearRepository { get; set; }

        public AssessmentRepository AssessmentRepository { get; set; }

        public AssessmentDetailRepository AssessmentDetailRepository { get; set; }

        public AssessmentScoreRepository AssessmentScoreRepository { get; set; }

        public EmpAssessmentScoreRepository EmpAssessmentScoreRepository { get; set; }

        public LoginRepository LoginRepository { get; set; }

        public UserLoginModelRepository UserLoginModelRepository { get; set; }

        public WidgetLibraryRepository WidgetLibraryRepository { get; set; }

        public WidgetPositionRepository WidgetPositionRepository { get; set; }

        public EmployeeDashboardRepository EmployeeDashboardRepository { get; set; }

        public SmsServiceRepository SmsServiceRepository { get; set; }

        public DimensionElementsRepository DimensionElementsRepository { get; set; }

        public ActionwisePermissionRepository ActionwisePermissionRepository { get; set; }

        public RoleRepository RoleRepository { get; set; }

        public EmployeeListRepository EmployeeListRepository { get; set; }

        public TeamConfigurationRepository TeamConfigurationRepository { get; set; }

        public TeamManagersRepository TeamManagersRepository { get; set; }

        public TeamEmployeesRepository TeamEmployeesRepository { get; set; }

        public TeamDashboardRepository TeamDashboardRepository { get; set; }

        public TeamListRepository TeamListRepository { get; set; }

        public EmployeeBasicRepository EmployeeBasicRepository { get; set; }

        public ClassificationRepository ClassificationRepository { get; set; }

        public EmployeeSparkRepository EmployeeSparkRepository { get; set; }

    }

    public class TriggerCatalogContext : DaoContext
    {
        public TriggerCatalogContext(IServiceProvider serviceProvider) : base(serviceProvider, "DefaultConnection")
        {

        }

        public virtual CompanyRepository CompanyRepository { get; set; }

        public virtual IndustryTypeRepository IndustryTypeRepository { get; set; }

        public virtual OrganizationTypeRepository OrganizationTypeRepository { get; set; }

        public virtual CompanyConfigRepository CompanyConfigRepository { get; set; }

        public EmployeeRepository EmployeeRepository { get; set; }

        public EmployeeBasicRepository EmployeeBasicRepository { get; set; }

        public AuthUserDetailsRepository AuthUserDetailsRepository { get; set; }

        public ClaimsRepository ClaimsRepository { get; set; }

        public ClaimsCommonRepository ClaimsCommonRepository { get; set; }

        public EmployeeDashboardRepository EmployeeDashboardRepository { get; set; }

        public AuthUserClaimCsvRepository AuthUserClaimCsvRepository { get; set; }

        public AuthUserCsvRepository AuthUserCsvRepository { get; set; }

        public DaoRepository<UserDetails> UserDetailsRepository { get; set; }

        public CountryRepository CountryRepository { get; set; }

        public RegionRepository RegionRepository { get; set; }

        public EthnicityRepository EthnicityRepository { get; set; }

        [QueryPath("DbConnection")]
        public DaoRepository<CompanyDbConfig> CompanyDbConfig { get; set; }

        public LoginRepository LoginRepository { get; set; }

        public ChangePasswordRepository ChangePasswordRepository { get; set; }

        public UserLoginRepository UserLoginRepository { get; set; }

        public UserLoginModelRepository UserLoginModelRepository { get; set; }

        public SmsServiceRepository SmsServiceRepository { get; set; }

        public DimensionRepository DimensionRepository { get; set; }

        public EmailTemplateRepository EmailTemplateRepository { get; set; }

        public ActionwisePermissionRepository ActionwisePermissionRepository { get; set; }

        public EmployeeSparkRepository EmployeeSparkRepository { get; set; }
    }
}
