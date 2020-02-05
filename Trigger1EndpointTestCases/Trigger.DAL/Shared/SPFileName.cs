namespace Trigger.DAL.Shared
{
    public static class SPFileName
    {
        //EmployeeRepository
        public const string InvokeEmpGetAllEmployee = "GetAllEmployee";
        public const string InvokeEmpGetAllEmployeeWithHierachy = "GetAllEmployeeWithHierachy";
        public const string InvokeEmpGetAllEmployeesHierachyWithPagination = "GetAllEmployeesHierachyWithPagination";
        public const string InvokeEmpGetAllEmployeeByManager = "GetAllEmployeeByManager";
        public const string InvokeEmpGetAllEmployeeForTriggerAdmin = "GetAllEmployeeForTriggerAdmin";
        public const string InvokeEmpGetCompanyWiseEmployee = "GetCompanyWiseEmployee";
        public const string InvokeEmpGetCompanyWiseEmployeeWithPagination = "GetCompanyWiseEmployeeWithPagination";
        public const string InvokeGetCompanyWiseEmployeeWithoutPagination = "GetCompanyWiseEmployeeWithoutPagination";
        public const string InvokeEmpGetAllEmployeeWithPagination = "GetAllEmployeeWithPagination";
        public const string InvokeGetAllEmployeeWithoutPagination = "GetAllEmployeeWithoutPagination";
        public const string InvokeEmpGetAllEmployeeByManagerWithPagination = "GetAllEmployeeByManagerWithPagination";
        public const string InvokeEmpGetAllEmployeeForTriggerAdminWithPagination = "GetAllEmployeeForTriggerAdminWithPagination";
        public const string InvokeGetAllCompanyAdminListWithoutPagination = "GetAllCompanyAdminListWithoutPagination";
        public const string InvokeEmpGetAllEmployeeYearwise = "GetAllEmployeeYearwise";
        public const string InvokeGetAllEmployeeYearwiseWithoutPagination = "GetAllEmployeeYearwiseWithoutPagination";
        public const string InvokeEmpGetAllEmployeeByManagerYearwise = "GetAllEmployeeByManagerYearwise";
        public const string InvokeGetAllEmployeeByManagerYearwiseWithoutPagination = "GetAllEmployeeByManagerYearwiseWithoutPagination";
        public const string InvokeEmpGetEmployeeByGradeYearwise = "GetEmployeeByGradeYearwise";
        public const string InvokeGetEmployeeByGradeYearwiseWithoutPagination = "GetEmployeeByGradeYearwiseWithoutPagination";
        public const string InvokeEmpGetEmployeeByGradeByManagerYearwise = "GetEmployeeByGradeByManagerYearwise";
        public const string InvokeGetEmployeeByGradeByManagerYearwiseWithoutPagination = "GetEmployeeByGradeByManagerYearwiseWithoutPagination";
        public const string InvokeEmpGetEmployeeByGradeAndMonthYearwise = "GetEmployeeByGradeAndMonthYearwise";
        public const string InvokeGetEmpByGradeAndMonthYearwiseWithoutPagination = "GetEmpByGradeAndMonthYearwiseWithoutPagination";
        public const string InvokeEmpGetEmployeeByGradeAndMonthByManagerYearwise = "GetEmployeeByGradeAndMonthByManagerYearwise";
        public const string InvokeGetEmpByGradeAndMonthByManagerYearwiseWithoutPagination = "GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination";
        public const string InvokeEmpGetEmployeeById = "GetEmployeeById";
        public const string InvokeGetEmployeeByEmpIdsForMails = "GetEmployeeByEmpIdsForMails";
        public const string InvokeEmpGetDeviceInfoById = "GetDeviceInfoById";
        public const string InvokeEmpGetNotificationById = "GetNotificationById";
        public const string GetExcelNewEmployees = "GetExcelNewEmployees";
        public const string AddUserLoginFromExcel = "AddUserLoginFromExcel";   //Also used in UserExcelRepository
        public const string InvokeDeleteEmployee = "DeleteEmployee";
        public const string InvokeDeleteUser = "DeleteUser";
        public const string InvokeGetEmployee = "GetEmployee";
        public const string InvokeUpdateEmpForIsMailSent = "UpdateEmpForIsMailSent";
        public const string InvokeGetAllEmployeesForMail = "GetAllEmployeesForMail";
        public const string InvokeGetAspNetUserCountByPhone = "GetAspNetUserCountByPhone";
        public const string InvokeYearwiseEmployeeWithHierachy = "GetYearwiseEmployeeWithHierachy";
        public const string InvokeGetEmployeeDetailsByEmpIds = "GetEmployeeDetailsByEmpIds";

        //CompanyAdminRepository
        public const string InvokeAddCompanyAdminDetails = "AddCompanyAdminDetails";
        public const string InvokeDeleteCompanyAdminDetails = "DeleteCompanyAdminDetails";

        //EmployeeProfileRepository
        public const string InvokeUpdateSmsService = "UpdateOptForSMS";

        //AuthUserClaimExcelRepository
        public const string AddAuthClaimsFromExcel = "AddAuthClaimsFromExcel";

        //AuthUserExcelRepository
        public const string AddAuthLoginFromExcel = "AddAuthLoginFromExcel";
        public const string GetExistingAuthUsers = "GetExistingAuthUsers";
        public const string InvokeGetEmpByPhnNumberCatalog = "GetEmployeeByPhoneNumberCatalog";
        public const string InvokeGetEmpByPhnNumberTenant = "GetEmployeeByPhoneNumberTenant";

        //EmployeeExcelRepository
        public const string AddEmpolyeeFromExcel = "AddEmpolyeeFromExcel";
        public const string AddCompanyAdminFromExcel = "AddCompanyAdminFromExcel";

        //ExcelUploadRepository
        public const string GetAllMastersForExcel = "GetAllMastersForExcel";
        public const string TempEmployeeDelete = "DeleteTempEmployee";
        public const string GetExcelDataWithCount = "GetExcelDataWithCount";

        //AuthUserDetailsRepository
        public const string GetSubIdByEmail = "GetSubIdByEmail";
        public const string GetPasswordHashByUserName = "GetPasswordHashByUserName";
        public const string GetAuthDetailsByEmail = "GetAuthDetailsByEmail";

        //EmailTemplateRepository
        public const string InvokeGetTemplateByName = "GetTemplateByName";
    }
}
