using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.Employee
{
<<<<<<< HEAD
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
        private const string invokeGetAllEmployeeYearwiseWithoutPagination = "GetAllEmployeeYearwiseWithoutPagination";
        private const string invokeempGetAllEmployeeByManagerYearwise = "GetAllEmployeeByManagerYearwise";
        private const string invokeGetAllEmployeeByManagerYearwiseWithoutPagination = "GetAllEmployeeByManagerYearwiseWithoutPagination";

        private const string invokeempGetEmployeeByGradeYearwise = "GetEmployeeByGradeYearwise";
        private const string invokeGetEmployeeByGradeYearwiseWithoutPagination = "GetEmployeeByGradeYearwiseWithoutPagination";
        private const string invokeempGetEmployeeByGradeByManagerYearwise = "GetEmployeeByGradeByManagerYearwise";
        private const string invokeGetEmployeeByGradeByManagerYearwiseWithoutPagination = "GetEmployeeByGradeByManagerYearwiseWithoutPagination";

        private const string invokeempGetEmployeeByGradeAndMonthYearwise = "GetEmployeeByGradeAndMonthYearwise";
        private const string invokeGetEmpByGradeAndMonthYearwiseWithoutPagination = "GetEmpByGradeAndMonthYearwiseWithoutPagination";

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
=======
    /// <summary>
    /// Class to get employee list from databased
    /// </summary>
    [QueryPath("Trigger.DAL.Query.Employee.Employee")]
    public class EmployeeRepository : DaoRepository<EmployeeModel>
    {
        /// <summary>
        /// Method to get all employee list
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployee(EmployeeModel employeeModel)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployee);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetAllEmployeeWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeWithPagination);
        }

        public List<EmployeeModel> GetAllEmployeeWithoutPagination(EmployeeModel employeeModel)
=======
        /// <summary>
        /// Method to get company wise employee list 
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetCompanyWiseEmployee(EmployeeModel employeeModel)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetCompanyWiseEmployee);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetAllEmployeeByManagerWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeByManagerWithPagination);
        }

        public List<EmployeeModel> GetAllEmployeeForTriggerAdminWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeForTriggerAdminWithPagination);
=======
        /// <summary>
        /// Method to get company wise employee list with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetCompanyWiseEmployeeWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetCompanyWiseEmployeeWithPagination);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get company wise employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetCompanyWiseEmployeeWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetCompanyWiseEmployeeWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetAllEmployeeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeYearwise);
=======
        /// <summary>
        /// Method to get employee list with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeWithPagination);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetAllEmployeeWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetAllEmployeeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetAllEmployeeByManagerYearwise);
=======
        /// <summary>
        /// Method to get employee list by manager with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeByManagerWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeByManagerWithPagination);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get employee list for trigger admin with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeForTriggerAdminWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeForTriggerAdminWithPagination);
        }

        /// <summary>
        /// Method to get employee list for trigger admin without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeForTriggerAdminWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetAllCompanyAdminListWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetEmployeeByGradeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeYearwise);
=======
        /// <summary>
        /// Method to get year wise employee list
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeYearwise);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get year wise employee list without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetAllEmployeeYearwiseWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetEmployeeByGradeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeByManagerYearwise);
=======
        /// <summary>
        /// Method to get year wise employee list by manager
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeByManagerYearwise);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get year wise employee list by manager without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetAllEmployeeByManagerYearwiseWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetEmployeeByGradeAndMonthYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeAndMonthYearwise);
=======
        /// <summary>
        /// Method to get employee list by grade and year  (not used)
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetEmployeeByGradeYearwise);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get employee list by grade and year without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmployeeByGradeYearwiseWithoutPagination);
        }

<<<<<<< HEAD
        public List<EmployeeModel> GetEmployeeByGradeAndMonthByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, invokeempGetEmployeeByGradeAndMonthByManagerYearwise);
=======
        /// <summary>
        /// Method to get employee list by grade, manager and year
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetEmployeeByGradeByManagerYearwise);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get employee list by grade, manager and year without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmployeeByGradeByManagerYearwiseWithoutPagination);
        }

<<<<<<< HEAD
        public EmployeeModel GetEmployeeById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, invokeempGetEmployeeById);
=======
        /// <summary>
        /// Method to get employee list by grade, moth and year
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeAndMonthYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetEmployeeByGradeAndMonthYearwise);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get employee list by grade, moth and year without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmpByGradeAndMonthYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmpByGradeAndMonthYearwiseWithoutPagination);
        }

        /// <summary>
        /// Method to get employee list by grade, manager, moth and year 
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByGradeAndMonthByManagerYearwise(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetEmployeeByGradeAndMonthByManagerYearwise);
        }

<<<<<<< HEAD
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
=======
        /// <summary>
        /// Method to get employee list by grade, manager, moth and year without pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmpByGradeAndMonthByManagerYearwiseWithoutPagination);
        }

        /// <summary>
        /// Method to get employee by empid
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual EmployeeModel GetEmployeeById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, SPFileName.InvokeEmpGetEmployeeById);
        }

        /// <summary>
        /// Method to get employee by empId's for send registration mail
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeeByEmpIdsForMails(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmployeeByEmpIdsForMails);
        }

        /// <summary>
        /// Method to get device info by user id
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<UserLoginModel> GetDeviceInfoById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<UserLoginModel>>(employeeModel, SPFileName.InvokeEmpGetDeviceInfoById);
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// Method to get notification by manager id
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<NotificationModel> GetNotificationById(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<NotificationModel>>(employeeModel, SPFileName.InvokeEmpGetNotificationById);
        }

        /// <summary>
        /// Get employees details from Csv
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns>EmployeeModel</returns>
        public virtual List<EmployeeModel> GetExcelEmployees(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.GetExcelNewEmployees);
        }

        /// <summary>
        /// Method to delete employee from tenant
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public virtual UserDetails DeleteEmployee(EmployeeModel userDetails)
        {
            return ExecuteQuery<UserDetails>(userDetails, SPFileName.InvokeDeleteEmployee);
        }

<<<<<<< HEAD
        public EmployeeModel UpdateEmpForIsMailSent(EmployeeModel employeeModel)
=======
        /// <summary>
        /// Method to delete employee from tenant (login details)
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public virtual UserDetails DeleteUser(EmployeeModel userDetails)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            return ExecuteQuery<UserDetails>(userDetails, SPFileName.InvokeDeleteUser);
        }

        /// <summary>
        /// Method to set Ismailsent falg true on registraion mail sent
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual EmployeeModel UpdateEmpForIsMailSent(EmployeeModel employeeModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeModel, SPFileName.InvokeUpdateEmpForIsMailSent);
        }

        /// <summary>
        /// Method to get aspnetuser count by phone number
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual int GetAspNetUserCountByPhone(EmployeeModel employeeModel)
        {
            return ExecuteQuery<int>(employeeModel, SPFileName.InvokeGetAspNetUserCountByPhone);
        }

        /// <summary>
        /// Method to get employee list with hierachy
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeeWithHierachy(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeeWithHierachy);
        }

        /// <summary>
        /// Method to get employee list with hierachy with pagination
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetAllEmployeesHierachyWithPagination(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeEmpGetAllEmployeesHierachyWithPagination);
        }

<<<<<<< HEAD
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

=======
        /// <summary>
        /// Method to get employee list by uesr with hierachy 
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetYearwiseEmployeeWithHierachy(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeYearwiseEmployeeWithHierachy);
        }

>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        public List<EmployeeModel> GetEmployeeDetailsByEmpIds(EmployeeModel employeeModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeModel, SPFileName.InvokeGetEmployeeDetailsByEmpIds);
        }
    }
}
