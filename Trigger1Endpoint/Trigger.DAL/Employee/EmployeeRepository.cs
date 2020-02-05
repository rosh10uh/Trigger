using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;

namespace Trigger.DAL.Employee
{
    [QueryPath("Trigger.DAL.Query.Employee.Employee")]
    public class EmployeeRepository : DaoRepository<EmployeeModel>
    {
        private const string invokeempGetAllEmployee = "GetAllEmployee";
        private const string invokeEmpGetAllEmployeeWithHierachy = "GetAllEmployeeWithHierachy";
        private const string invokeEmpGetAllEmployeesHierachyWithPagination = "GetAllEmployeesHierachyWithPagination";

        private const string invokeempGetAllEmployeeByManager = "GetAllEmployeeByManager";
        private const string invokeempGetAllEmployeeForTriggerAdmin = "GetAllEmployeeForTriggerAdmin";

        private const string invokeempGetCompanyWiseEmployee = "GetCompanyWiseEmployee";
        private const string invokeempGetCompanyWiseEmployeeWithPagination = "GetCompanyWiseEmployeeWithPagination";
        private const string invokeGetCompanyWiseEmployeeWithoutPagination = "GetCompanyWiseEmployeeWithoutPagination";

        private const string invokeempGetAllEmployeeWithPagination = "GetAllEmployeeWithPagination";
        private const string invokeGetAllEmployeeWithoutPagination = "GetAllEmployeeWithoutPagination";

        private const string invokeempGetAllEmployeeByManagerWithPagination = "GetAllEmployeeByManagerWithPagination";
        private const string invokeempGetAllEmployeeForTriggerAdminWithPagination = "GetAllEmployeeForTriggerAdminWithPagination";
        private const string invokeGetAllCompanyAdminListWithoutPagination = "GetAllCompanyAdminListWithoutPagination";

        private const string invokeempGetAllEmployeeYearwise = "GetAllEmployeeYearwise";
        private const string invokeempGetAllOrgEmployeeYearwise = "GetAllOrgEmployeeYearwise";
        private const string invokeGetAllEmployeeYearwiseWithoutPagination = "GetAllEmployeeYearwiseWithoutPagination";
        private const string invokeGetAllOrgEmployeeYearwiseWithoutPagination = "GetAllOrgEmployeeYearwiseWithoutPagination";
        private const string invokeempGetAllEmployeeByManagerYearwise = "GetAllEmployeeByManagerYearwise";
        private const string invokeGetAllEmployeeByManagerYearwiseWithoutPagination = "GetAllEmployeeByManagerYearwiseWithoutPagination";

        private const string invokeempGetEmployeeByGradeYearwise = "GetEmployeeByGradeYearwise";
        private const string invokeGetEmployeeByGradeYearwiseWithoutPagination = "GetEmployeeByGradeYearwiseWithoutPagination";
        private const string invokeGetEmployeeByGradeYearwiseWithPagination = "GetEmployeeByGradeYearwiseWithPagination";
        private const string invokeGetOrgEmployeeByGradeYearwiseWithPagination = "GetOrgEmployeeByGradeYearwiseWithPagination";
        private const string invokeGetOrgEmployeeByGradeYearwiseWithoutPagination = "GetOrgEmployeeByGradeYearwiseWithoutPagination";
        private const string invokeempGetEmployeeByGradeByManagerYearwise = "GetEmployeeByGradeByManagerYearwise";
        private const string invokeGetEmployeeByGradeByManagerYearwiseWithoutPagination = "GetEmployeeByGradeByManagerYearwiseWithoutPagination";

        private const string invokeempGetEmployeeByGradeAndMonthYearwise = "GetEmployeeByGradeAndMonthYearwise";
        private const string invokeempGetOrgEmployeeByGradeAndMonthYearwise = "GetOrgEmployeeByGradeAndMonthYearwise";
        private const string invokeGetEmpByGradeAndMonthYearwiseWithoutPagination = "GetEmpByGradeAndMonthYearwiseWithoutPagination";
        private const string invokeGetOrgEmpByGradeAndMonthYearWiseWithoutPagination = "GetOrgEmpByGradeAndMonthYearwiseWithoutPagination";

        private const string invokeempGetEmployeeByGradeAndMonthByManagerYearwise = "GetEmployeeByGradeAndMonthByManagerYearwise";
        private const string invokeGetEmpByGradeAndMonthByManagerYearwiseWithoutPagination = "GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination";

        private const string invokeempGetEmployeeById = "GetEmployeeById";
        private const string invokeGetEmployeeByEmpIdsForMails = "GetEmployeeByEmpIdsForMails";
        private const string invokeGetManagersDetByManagerIds = "GetManagersDetByManagerIds";

        private const string invokeempGetDeviceInfoById = "GetDeviceInfoById";
        private const string invokeempGetNotificationById = "GetNotificationById";
        private const string GetCsvNewEmployees = "GetCSVNewEmployees";
        private const string AddUserLoginFromCsv = "AddUserLoginFromCSV";

        private const string invokeDeleteEmployee = "DeleteEmployee";
        private const string invokeDeleteUser = "DeleteUser";

        private const string invokeGetEmployee = "GetEmployee";
        private const string invokeUpdateEmpForIsMailSent = "UpdateEmpForIsMailSent";
        private const string invokeGetAllEmployeesForMail = "GetAllEmployeesForMail";
        private const string invokeGetAspNetUserCountByPhone = "GetAspNetUserCountByPhone";
        private const string invokeYearwiseEmployeeWithHierachy = "GetYearwiseEmployeeWithHierachy";
        private const string invokeGetEmpTeamRelationByManagerId = "GetEmpTeamRelationByManagerId";
        private const string invokeGetEmployeeNotPartOfTeamByManagerId = "GetEmployeeNotPartOfTeamByManagerId";

        private const string invokeGetTeamOversightEmployeesByManagerId = "GetOversightTeamEmployeesByManagerId";

        private const string invokeGetEmployeeDetailsByEmpIds = "GetEmployeeDetailsByEmpIds";
        
        public List<EmployeeModel> GetAllEmployee(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployee);
        }

        public List<EmployeeModel> GetAllEmployeeByManager(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeByManager);
        }

        public List<EmployeeModel> GetAllEmployeeForTriggerAdmin(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeForTriggerAdmin);
        }

        public List<EmployeeModel> GetCompanyWiseEmployee(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetCompanyWiseEmployee);
        }

        public List<EmployeeModel> GetCompanyWiseEmployeeWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetCompanyWiseEmployeeWithPagination);
        }

        public List<EmployeeModel> GetCompanyWiseEmployeeWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetCompanyWiseEmployeeWithoutPagination);
        }

        public List<EmployeeModel> GetAllEmployeeWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeWithPagination);
        }

        public List<EmployeeModel> GetAllEmployeeWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllEmployeeWithoutPagination);
        }

        public List<EmployeeModel> GetAllEmployeeByManagerWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeByManagerWithPagination);
        }

        public List<EmployeeModel> GetAllEmployeeForTriggerAdminWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeForTriggerAdminWithPagination);
        }

        public List<EmployeeModel> GetAllEmployeeForTriggerAdminWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllCompanyAdminListWithoutPagination);
        }

        public List<EmployeeModel> GetAllEmployeeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeYearwise);
        }

        public List<EmployeeModel> GetAllOrgEmployeeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllOrgEmployeeYearwise);
        }

        public List<EmployeeModel> GetAllEmployeeYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllEmployeeYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetAllOrgEmployeeYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllOrgEmployeeYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetAllEmployeeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeByManagerYearwise);
        }


        public List<EmployeeModel> GetAllEmployeeByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllEmployeeByManagerYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeYearwise);
        }

        public List<EmployeeModel> GetEmployeeByGradeYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmployeeByGradeYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeYearwiseWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmployeeByGradeYearwiseWithPagination);
        }

        public List<EmployeeModel> GetOrgEmployeeByGradeYearwiseWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetOrgEmployeeByGradeYearwiseWithPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeYearwiseCompAdminWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetOrgEmployeeByGradeYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeByManagerYearwise);
        }

        public List<EmployeeModel> GetEmployeeByGradeByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmployeeByGradeByManagerYearwiseWithoutPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeAndMonthYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeAndMonthYearwise);
        }

        public List<EmployeeModel> GetOrgEmployeeByGradeAndMonthYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetOrgEmployeeByGradeAndMonthYearwise);
        }

        public List<EmployeeModel> GetEmpByGradeAndMonthYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmpByGradeAndMonthYearwiseWithoutPagination);
        }
        public List<EmployeeModel> GetOrgEmpByGradeAndMonthYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetOrgEmpByGradeAndMonthYearWiseWithoutPagination);
        }

        public List<EmployeeModel> GetEmployeeByGradeAndMonthByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeAndMonthByManagerYearwise);
        }

        public List<EmployeeModel> GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmpByGradeAndMonthByManagerYearwiseWithoutPagination);
        }

        public EmployeeModel GetEmployeeById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, invokeempGetEmployeeById);
        }

        public List<EmployeeModel> GetAllEmployeesForMail(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetAllEmployeesForMail);
        }

        public List<EmployeeModel> GetEmployeeByEmpIdsForMails(EmployeeModel employeeModels)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModels, invokeGetEmployeeByEmpIdsForMails);
        }

        public List<UserLoginModel> GetDeviceInfoById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<UserLoginModel>>(employeeModel, invokeempGetDeviceInfoById);
        }

        public List<NotificationModel> GetNotificationById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<NotificationModel>>(employeeModel, invokeempGetNotificationById);
        }

        /// <summary>
        /// Get employees details from Csv
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns>EmployeeModel</returns>
        public List<EmployeeModel> GetCsvEmployees(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, GetCsvNewEmployees);
        }

        /// <summary>
        /// Get employees details from Csv
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns>EmployeeModel</returns>
        public List<EmployeeModel> AddCsvUserLogin(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, AddUserLoginFromCsv);
        }

        public UserDetails DeleteEmployee(EmployeeModel userDetails)
        {
            return ExecuteQuery<UserDetails>(userDetails, invokeDeleteEmployee);
        }

        public UserDetails DeleteUser(EmployeeModel userDetails)
        {
            return ExecuteQuery<UserDetails>(userDetails, invokeDeleteUser);
        }

        public EmployeeModel GetEmployee(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, invokeGetEmployee);
        }

        public EmployeeModel UpdateEmpForIsMailSent(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, invokeUpdateEmpForIsMailSent);
        }

        public int GetAspNetUserCountByPhone(EmployeeModel employeeModel)
        {
            return ExecuteQuery<int>(employeeModel, invokeGetAspNetUserCountByPhone);
        }

        public List<EmployeeModel> GetAllEmployeeWithHierachy(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeEmpGetAllEmployeeWithHierachy);
        }

        public List<EmployeeModel> GetAllEmployeesHierachyWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeEmpGetAllEmployeesHierachyWithPagination);
        }

        public List<EmployeeModel> GetYearwiseEmployeeWithHierachy(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeYearwiseEmployeeWithHierachy);
        }

        public List<EmployeeModel> GetEmployeeTeamRelationByManagerId(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmpTeamRelationByManagerId);
        }

        public List<EmployeeModel> GetEmployeeNotPartOfTeamByManagerId(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmployeeNotPartOfTeamByManagerId);
        }

        public List<EmployeeModel> GetTeamOversightEmployeesByManagerId(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetTeamOversightEmployeesByManagerId);
        }

        public List<EmployeeModel> GetEmployeeDetailsByEmpIds(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeGetEmployeeDetailsByEmpIds);
        }
    }
}
