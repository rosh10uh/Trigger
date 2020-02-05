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

<<<<<<< HEAD
        public EmployeeRepository EmployeeRepository { get; set; }
=======
        public virtual EmployeeRepository EmployeeRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual UserDetailRepository UserDetailRepository { get; set; }

        public virtual NotificationRepository NotificationRepository { get; set; }

        public virtual UserLoginRepository UserLoginRepository { get; set; }

        public virtual EmpProfileRepository EmpProfileRepository { get; set; }

        public virtual EmployeeSalaryRepository EmpSalaryRepository { get; set; }

        public virtual EmployeeProfileRepository EmployeeProfileRepository { get; set; }

        [QueryPath("Trigger.DAL.Query.ExcelUpload.ExcelUpload")]
<<<<<<< HEAD
        public ExcelUploadRepository ExcelUploadRepository { get; set; }

        public EmployeeCsvRepository EmployeeCsvRepository { get; set; }
        public AuthUserCsvRepository AuthUserCsvRepository { get; set; }
=======
        public virtual ExcelUploadRepository ExcelUploadRepository { get; set; }

        public virtual EmployeeExcelRepository EmployeeExcelRepository { get; set; }
        public virtual AuthUserExcelRepository AuthUserExcelRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual UserExcelRepository UserExcelRepository { get; set; }

        [QueryPath("Trigger.DAL.Query.Department.Department")]
<<<<<<< HEAD
        public DepartmentRepository DepartmentRepository { get; set; }
=======
        public virtual DepartmentRepository DepartmentRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        [QueryPath("Trigger.DAL.Query.Questionnaries.Questionnaries")]
        public virtual QuestionnariesRepository QuestionnariesRepository { get; set; }

        public virtual AssessmentYearRepository AssessmentYearRepository { get; set; }

        public virtual AssessmentRepository AssessmentRepository { get; set; }

<<<<<<< HEAD
        public AssessmentDetailRepository AssessmentDetailRepository { get; set; }
=======
        public virtual AssessmentDetailRepository AssessmentDetailRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual AssessmentScoreRepository AssessmentScoreRepository { get; set; }

<<<<<<< HEAD
        public EmpAssessmentScoreRepository EmpAssessmentScoreRepository { get; set; }
=======
        public virtual EmpAssessmentScoreRepository EmpAssessmentScoreRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual LoginRepository LoginRepository { get; set; }

        public virtual UserLoginModelRepository UserLoginModelRepository { get; set; }

        public virtual WidgetLibraryRepository WidgetLibraryRepository { get; set; }

        public virtual WidgetPositionRepository WidgetPositionRepository { get; set; }

        public virtual EmployeeDashboardRepository EmployeeDashboardRepository { get; set; }

        public virtual SmsServiceRepository SmsServiceRepository { get; set; }

        public virtual DimensionElementsRepository DimensionElementsRepository { get; set; }

        public virtual ActionwisePermissionRepository ActionwisePermissionRepository { get; set; }

        public virtual RoleRepository RoleRepository { get; set; }

        public virtual EmployeeListRepository EmployeeListRepository { get; set; }

        public TeamConfigurationRepository TeamConfigurationRepository { get; set; }

        public TeamManagersRepository TeamManagersRepository { get; set; }

        public TeamEmployeesRepository TeamEmployeesRepository { get; set; }

        public TeamDashboardRepository TeamDashboardRepository { get; set; }

        public TeamListRepository TeamListRepository { get; set; }

        public EmployeeBasicRepository EmployeeBasicRepository { get; set; }

        public ClassificationRepository ClassificationRepository { get; set; }

        public virtual EmployeeSparkRepository EmployeeSparkRepository { get; set; }

    }

    public class TriggerCatalogContext : DaoContext
    {
        public TriggerCatalogContext(IServiceProvider serviceProvider) : base(serviceProvider, "DefaultConnection")
        {

<<<<<<< HEAD
        }

        public virtual CompanyRepository CompanyRepository { get; set; }
=======
        }        
       
		public virtual CompanyRepository CompanyRepository { get; set; }

        public virtual CompanyAdminRepository CompanyAdminRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual IndustryTypeRepository IndustryTypeRepository { get; set; }

        public virtual OrganizationTypeRepository OrganizationTypeRepository { get; set; }

        public virtual CompanyConfigRepository CompanyConfigRepository { get; set; }

<<<<<<< HEAD
        public EmployeeRepository EmployeeRepository { get; set; }

        public EmployeeBasicRepository EmployeeBasicRepository { get; set; }
=======
        public virtual EmployeeRepository EmployeeRepository { get; set; }

        public virtual AuthUserDetailsRepository AuthUserDetailsRepository { get; set; }

        public virtual ClaimsRepository ClaimsRepository { get; set; }

        public virtual ClaimsCommonRepository ClaimsCommonRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual EmployeeDashboardRepository EmployeeDashboardRepository { get; set; }

        public virtual EmployeeExcelRepository EmployeeExcelRepository { get; set; }

<<<<<<< HEAD
        public ClaimsCommonRepository ClaimsCommonRepository { get; set; }
=======
        public virtual AuthUserClaimExcelRepository AuthUserClaimExcelRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual AuthUserExcelRepository AuthUserExcelRepository { get; set; }

<<<<<<< HEAD
        public AuthUserClaimCsvRepository AuthUserClaimCsvRepository { get; set; }

        public AuthUserCsvRepository AuthUserCsvRepository { get; set; }

        public DaoRepository<UserDetails> UserDetailsRepository { get; set; }

        public CountryRepository CountryRepository { get; set; }

        public RegionRepository RegionRepository { get; set; }
=======
        public virtual DaoRepository<UserDetails> UserDetailsRepository { get; set; }

        public virtual CountryRepository CountryRepository { get; set; }

        public virtual RegionRepository RegionRepository { get; set; }
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686

        public virtual EthnicityRepository EthnicityRepository { get; set; }

        [QueryPath("DbConnection")]
        public virtual DaoRepository<CompanyDbConfig> CompanyDbConfig { get; set; }

        public virtual LoginRepository LoginRepository { get; set; }

        public virtual ChangePasswordRepository ChangePasswordRepository { get; set; }

        public virtual UserLoginRepository UserLoginRepository { get; set; }

        public virtual UserLoginModelRepository UserLoginModelRepository { get; set; }

        public virtual SmsServiceRepository SmsServiceRepository { get; set; }

        public virtual DimensionRepository DimensionRepository { get; set; }

        public virtual EmailTemplateRepository EmailTemplateRepository { get; set; }

        public ActionwisePermissionRepository ActionwisePermissionRepository { get; set; }

        public EmployeeSparkRepository EmployeeSparkRepository { get; set; }
    }
}
