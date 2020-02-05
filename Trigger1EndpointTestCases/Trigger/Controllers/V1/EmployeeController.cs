using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// controller to perform action for employee
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly Employee _employee;
        private readonly EmployeeProfile _employeeProfile;
        private readonly EmployeeForDashboard _employeeForDashboard;
        private readonly EmployeeSendEmail _employeeSendEmail;

        /// <summary>
        /// employee constructor 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeeProfile"></param>
        /// <param name="employeeForDashboard"></param>
        /// <param name="employeeSendEmail"></param>
        public EmployeeController(Employee employee, EmployeeProfile employeeProfile,
            EmployeeForDashboard employeeForDashboard, EmployeeSendEmail employeeSendEmail)
        {
            _employee = employee;
            _employeeProfile = employeeProfile;
            _employeeForDashboard = employeeForDashboard;
            _employeeSendEmail = employeeSendEmail;
        }

        /// <summary>
        /// Get employee details by given empId for edit profile
        /// </summary>
        /// <param name="empid">Pass EmpId for getting details </param>
        /// <returns></returns>
        //(GetEmployeeByIdForEditProfile)
        [HttpGet]
        [Route("api/employee/editProfile/{empid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByIdForEditProfile(int empid)
        {
            return await _employeeProfile.GetEmployeeByIdForEditProfileAsync(empid);
        }

        /// <summary>
        /// Api to edit profile of logged in employee
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/employee/editProfile/{userid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<CustomJsonData> EditProfile(int userid, [FromBody] DTO.EmployeeProfileModel employee)
        {
            return await _employeeProfile.UpdateProfileAsync(userid, employee);
        }

        /// <summary>
        /// Api to allow sms service for inactivity notification of manager
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/employee/allowSms/{userid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> AllowSms(int userid, [FromBody] DTO.EmployeeProfileModel employee)
        {
            return await _employeeProfile.UpdateAllowSmsAsync(userid, employee);
        }

        //Changeprofile Picture
        /// <summary>
        /// Api to change profile pic for logged in employee
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="empProfile"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/changeprofile/{userid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> ChangeProfile(int userid, [FromBody] EmployeeProfilePicture empProfile)
        {
            return await _employeeProfile.UpdateProfilePicAsync(userid, empProfile);
        }

        ///api/dashboardemployees/{yearid}/{companyid}/{managerid}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}
        /// <summary>
        /// Api to get year wise employee list when redirecting from employee count 
        /// </summary>
        /// <param name="Yearid"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearid}/{companyId}/{Managerid}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllEmployeesWithPaginationYearWise([FromRoute] int Yearid, int companyId, int managerId, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = Yearid, CompanyId = companyId, ManagerId = managerId, PageNumber = pagenumber, PageSize = pagesize, SearchString = searchstring, DepartmentList = departmentlist };
            return await _employeeForDashboard.GetAllEmployeesWithPaginationYearWiseAsync(employeeModel);
        }

        ///api/dashboardemployees/{yearid}/{companyid}/{managerid}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}
        /// <summary>
        /// Api to get year wise and grade wise employee list with pagination when redirecting from progressive or pie graph
        /// </summary>
        /// <param name="Yearid"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearid}/{companyid}/{managerid}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByGradeWithPagination([FromRoute] int Yearid, int companyId, int managerId, string grade, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = Yearid, CompanyId = companyId, ManagerId = managerId, Grade = grade, PageNumber = pagenumber, PageSize = pagesize, SearchString = searchstring, DepartmentList = departmentlist };

            return await _employeeForDashboard.GetEmployeeByGradeWithPaginationAsync(employeeModel);
        }

        ///api/dashboardemployees/{yearid}/{companyid}/{managerid}/{month}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}
        /// <summary>
        /// Api to get year wise, month wise  and grade wise employee list with pagination when redirecting from bar graph
        /// </summary>
        /// <param name="Yearid"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="month"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearid}/{companyid}/{managerid}/{month}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithPagination([FromRoute] int Yearid, int companyId, int managerId, string month, string grade, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = Yearid, CompanyId = companyId, ManagerId = managerId, Month = month, Grade = grade, PageNumber = pagenumber, PageSize = pagesize, SearchString = searchstring, DepartmentList = departmentlist };
            return await _employeeForDashboard.GetEmployeeByGradeAndMonthWithPaginationAsync(employeeModel);
        }

        /// <summary>
        /// Api to get year and grade wise employees when redirect from progressive/pie chart
        /// Get Employee Of My Direct Reports Today (Grade wise)
        /// Get Employee Of My Direct Reports Today (Grade wise , pi chart)
        /// Get Employee Of My Organization Today (Grade wise , pi chart)
        /// </summary>
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearId}/{companyId}/{managerId}/{grade}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByGradeWithoutPagination([FromRoute] int yearId, int companyId, int managerId, string grade, string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = yearId, CompanyId = companyId, ManagerId = managerId, Grade = grade, DepartmentList = departmentlist };
            return await _employeeForDashboard.GetEmployeeByGradeWithoutPaginationAsync(employeeModel);
        }

        /// <summary>
        /// Api to get year wise employees when redirect from direct and organization count
        /// Get Employee of Total number of Direct reports today
        /// Get Employee Of total number of employee in my Organization (uncount admin/executive and inactive employee)
        /// </summary>
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearId}/{companyId}/{managerId}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllEmployeesWithoutPaginationYearWise([FromRoute] int yearId, int companyId, int managerId, string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = yearId, CompanyId = companyId, ManagerId = managerId, DepartmentList = departmentlist };
            return await _employeeForDashboard.GetAllEmployeesWithoutPaginationYearWiseAsync(employeeModel);
        }

        /// <summary>
        /// Api to get year wise employees when redirect from direct and organization bar graph
        /// Get Emploee of My Direct Reports Today (Month wise)
        /// Get Employee Of My Organization Today (Month wise)
        /// </summary>
        /// <param name="yearId"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="month"></param>
        /// <param name="grade"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dashboardemployees/{yearId}/{companyId}/{managerId}/{month}/{grade}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByGradeAndMonthWithoutPagination([FromRoute] int yearId, int companyId, int managerId, string month, string grade, string departmentlist = "")
        {
            var employeeModel = new EmployeeModel { YearId = yearId, CompanyId = companyId, ManagerId = managerId, Month = month, Grade = grade, DepartmentList = departmentlist };
            return await _employeeForDashboard.GetEmployeeByGradeAndMonthWithoutPaginationAsync(employeeModel);
        }

        /// <summary>
        /// Api to add employee(Admin/Executive/Manager/NonManager)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/employee/{userid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<CustomJsonData> Post(int userid, [FromBody] EmployeeModel employee)
        {
            return await _employee.InsertAsync(userid, employee);
        }

        /// <summary>
        /// Api to update employee
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/employee/{userid}")]
        [Authorize]
        [DynamicConnection]
        [ParameterValidation]
        public async Task<CustomJsonData> Put(int userid, [FromBody] EmployeeModel employee)
        {
            return await _employee.UpdateAsync(userid, employee);
        }

        /// <summary>
        /// Api to delete employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="empId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/employee/{companyId}/{empId}/{updatedBy}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Delete([FromRoute]int companyId, int empId, int updatedBy)
        {
            return await _employee.DeleteAsync(companyId, empId, updatedBy);
        }

        /// <summary>
        /// Api to change employee's current salary.Only Manager have this rights only.
        /// </summary>
        /// <param name="userid">userId of logged in manager</param>
        /// <param name="empSalary">Employee Salary object which has required fields.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/changeempsalary/{userid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> ChangeEmpSalary(int userid, [FromBody] EmployeeSalary empSalary)
        {
            return await _employee.UpdateEmpSalaryAsync(userid, empSalary);
        }

        /// <summary>
        /// Api to send mail for user registration or updatation from employee listing page
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/sendmail/{userId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> EmployeeSendMail(int userId, [FromBody] EmployeeModel employee)
        {
            return await _employeeSendEmail.SendMailAndUpdateFlag(userId, employee);
        }

        /// <summary>
        /// Get employee details by given empId
        /// </summary>
        /// <param name="empid">Pass EmpId for getting details </param>
        /// <returns></returns>
        //(GetEmployeeById)
        [HttpGet]
        [Route("api/employee/{empid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeById(int empid)
        {
            return await _employee.GetEmployeeByIdAsync(empid);
        }

        //api/employee/companyId/Empid
        //(GetEmployeeById with companyId)
        /// <summary>
        /// Get companywise employee details by passing companyId, empId
        /// </summary>
        /// <param name="companyId">Pass companyId of employee from the company he/she belongs</param>
        /// <param name="empid">Pass empId of employee to get details</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employees/{companyId}/{empid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetEmployeeByIdWithCompanyId([FromRoute]int companyId, int empid)
        {
            return await _employee.GetEmployeeByIdAsync(empid);
        }

        //Get employee details by id from tenant only for Trigger Admin case
        /// <summary>
        /// Get company admin details by the Id
        /// </summary>
        /// <param name="companyId">Pass companyId of employee from the company he/she belongs</param>
        /// <param name="empid">Pass empId of employee to get details</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/companyadmin/{companyId}/{empid}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetCompanyAdimnById([FromRoute]int companyId, int empid)
        {
            return await _employee.GetEmployeeByIdAsync(empid);
        }

        //api/employee/companyId / managerId
        //(getAllEmployees)
        /// <summary>
        /// Get all employees details as per reporting person's Id
        /// </summary>
        /// <param name="companyId">Pass companyId of employee from the company he/she belongs</param>
        /// <param name="managerId">Logged in empId</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> Get([FromRoute]int companyId, int managerId)
        {
            return await _employee.GetAllEmployeesAsync(companyId, managerId);
        }

        //api/allemployee/ companyId
        //(getAllManager)
        /// <summary>
        /// Get all employees of the company whose roles are Admin, Executive or Manager
        /// </summary>
        /// <param name="companyId">Pass companyId</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/allemployee/{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllManager([FromRoute]int companyId)
        {
            return await _employee.GetCompanyWiseEmployeeAsync(companyId);
        }

        //api/allemployees/ companyId / pageno / pagesize / searchtext / departmentId
        //(getAllManagers_SD)
        /// <summary>
        /// Api to get all employees with pagination when trigger admin redirect to perticular company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/allemployees/{companyId}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllManagers_SD([FromRoute]int companyId, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            EmployeeModel employee = new EmployeeModel { CompanyId = companyId, PageNumber = pagenumber, PageSize = pagesize, SearchString = searchstring, DepartmentList = departmentlist };
            return await _employee.GetCompanyWiseEmployeeWithPaginationAsync(employee);
        }

        //api/employees/ companyId / managerId /pageno /pagesize / searchtext / departmentId
        //(getAllEmployee_SD)
        /// <summary>
        /// Api to get all employees with pagination for employee listing page
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employees/{companyId}/{managerid}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllEmployee_SD([FromRoute]int companyId, int managerId, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            var employeeModel = new EmployeeModel
            {
                CompanyId = companyId,
                ManagerId = managerId,
                PageNumber = pagenumber,
                PageSize = pagesize,
                SearchString = searchstring,
                DepartmentList = departmentlist
            };

            return await _employee.GetAllEmployeesWithPaginationAsync(employeeModel);
        }

        /// <summary>
        /// unsed API to resond with app version upgrade message for mobile apps
        /// Api to get manager wise grade wise employee list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/{companyid}/{managerid}/{grade}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetEmployeeByGrade([FromRoute]int companyId, int managerId, string grade)
        {
            return await _employee.GetEmployee();
        }

        /// <summary>
        /// UnUsed API to resond with app version upgrade message for mobile apps
        ///  Api to get manager wise month wise and grade wise employee list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="month"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/{companyid}/{managerid}/{month}/{grade}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetEmployeeByGradeAndMonth([FromRoute]int companyId, int managerId, string month, string grade)
        {
            return await _employee.GetEmployee();
        }

        /// <summary>
        /// UnUsed API to resond with app version upgrade message for mobile apps
        ///  Api to get department wise, manager wise grade wise employee list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employees/{companyid}/{managerid}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetEmployeeByGradeWithPagination([FromRoute]int companyId, int managerId, string grade, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            return await _employee.GetEmployee();
        }

        /// <summary>
        /// UnUsed API to resond with app version upgrade message for mobile apps
        /// Api to get department wise, manager wise month wise and grade wise employee list 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="grade"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="searchstring"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employees/{companyid}/{managerid}/{month}/{grade}/{pagenumber}/{pagesize}/{searchstring}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<JsonData> GetEmployeeByGradeAndMonthWithPagination([FromRoute]int companyId, int managerId, string grade, int pagenumber = 0, int pagesize = 0, string searchstring = "", string departmentlist = "")
        {
            return await _employee.GetEmployee();
        }

        /// <summary>
        /// Api to get list of employees without pagination
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employees/{companyId}/{managerId}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllEmployees([FromRoute]int companyId, int managerId, string departmentlist = "")
        {
            return await _employee.GetAllEmployeesWithoutPaginationAsync(companyId, managerId, departmentlist);
        }

        /// <summary>
        /// Api to Get all employees list for Trigger admin when redirect to perticular client
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentlist"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/allemployees/{companyId}/{departmentlist}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetAllEmployeesListForTriggerAdmin([FromRoute]int companyId, string departmentlist = "")
        {
            return await _employee.GetCompanyWiseEmployeeWithoutPaginationAsync(companyId, departmentlist);
        }

        /// <summary>
        /// Api to get employee list for Trigger employee 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/triggeremplist/{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetTriggerEmpList([FromRoute]int companyId, int managerId)
        {
            return await _employee.GetTriggerEmpList(companyId, managerId);
        }

        /// <summary>
        /// Api to get employee list for Employee Dashboard
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/dashboardemplist/{companyId}/{managerId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetDashboardEmpList([FromRoute]int companyId, int managerId)
        {
            return await _employeeForDashboard.GetDashboardEmpList(companyId, managerId);
        }

        /// <summary>
        /// Api to get all non manager list
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/nonmanagers/{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetNonManagersList([FromRoute]int companyId)
        {
            return await _employee.GetNonManagersListAsync(companyId);
        }


        /// <summary>
        /// Get all employees of the company whose roles are Admin, Executive or Manager and they are generate password
        /// Api to get all login user 
        /// Used : Userd in team creation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/employee/ActiveManagerList/{companyId}")]
        [Authorize]
        [DynamicConnection]
        public async Task<CustomJsonData> GetActiveManagerList(int companyId)
        {
            return await _employee.GetActiveManagerList(companyId);
        }
    }
}
