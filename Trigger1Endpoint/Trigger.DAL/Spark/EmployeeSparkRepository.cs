using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.Spark;

namespace Trigger.DAL.Spark
{
    /// <summary>
    /// Class Name   :   EmployeeSparkRepository
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   09 Aug 2019
    /// Purpose      :   Repository for employee spark CRUD operations
    /// Revision     :  
    /// </summary>
    [QueryPath("Trigger.DAL.Query.Spark.EmployeeSpark")]
    public class EmployeeSparkRepository : DaoRepository<EmployeeSparkModel>
    {
        public const string invokeAddEmployeeSparkDetails = "AddEmployeeSparkDetails";
        public const string invokeUpdateEmployeeSparkDetails = "UpdateEmployeeSparkDetails";
        public const string invokeUpdateSparkApprovalStatus = "UpdateSparkApprovalStatus";
        public const string invokeGetEmployeeSparkDetails = "GetEmployeeSparkDetails";
        public const string invokeGetUnApprovedSparkByManager = "GetUnApprovedSparkByManager";
        public const string invokeGetUnApprovedSparkCountByManager = "GetUnApprovedSparkCountByManager";
        public const string invokeGetEmployeeSparkDetailsBySparkId = "GetEmployeeSparkDetailsBySparkId";
        public const string invokeDeleteEmployeeSparkDetails = "DeleteEmployeeSparkDetails";
        public const string invokeDeleteEmployeeSparkAttachment = "DeleteEmployeeSparkAttachment";
        public const string invokeGetAspnetUserByPhoneNumber = "GetAspnetUserByPhoneNumber";
        public const string invokeGetEmployeeByEmployeeId = "GetEmployeeByEmployeeId";
        public const string invokeGetTriggerAdminList = "GetTriggerAdminList";
        public const string invokeGetComapnyAdminList = "GetCompanyAdminList";
        public const string invokeGetSparkRejectionList = "GetSparkRejectionDetails";

        //Get employee spark details for particular spark
        public EmployeeSparkModel GetEmployeeSparkDetailsBySparkId(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeGetEmployeeSparkDetailsBySparkId);
        }

        //Get list of Sparks for employee
        public List<EmployeeSparkModel> GetEmployeeSparkDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<List<EmployeeSparkModel>>(employeeSparkModel, invokeGetEmployeeSparkDetails);
        }

        //Get list of unapproved Spark for employees
        public List<EmployeeSparkModel> GetUnApprovedSparkDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<List<EmployeeSparkModel>>(employeeSparkModel, invokeGetUnApprovedSparkByManager);
        }

        //Get count of unapproved Spark for employees
        public int GetUnApprovedSparkCount(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<int>(employeeSparkModel, invokeGetUnApprovedSparkCountByManager);
        }

        //Add new spark details for employee
        public EmployeeSparkModel InsertEmployeeSparkDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeAddEmployeeSparkDetails);
        }

        //Update spark approval status
        public EmployeeSparkModel UpdateSparkApprovalStatus(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeUpdateSparkApprovalStatus);
        }

        //Update existing spark details for employee
        public EmployeeSparkModel UpdateEmployeeSparkDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeUpdateEmployeeSparkDetails);
        }

        //Delete specific spark details for employee
        public EmployeeSparkModel DeleteEmployeeSparkDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeDeleteEmployeeSparkDetails);
        }

        //Delete attachment of specific spark for employee
        public EmployeeSparkModel DeleteEmployeeSparkAttachment(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeSparkModel>(employeeSparkModel, invokeDeleteEmployeeSparkAttachment);
        }

        //Get aspnet user details by phone number for sms spark
        public List<AspnetUserDetails> GetAspnetUserByPhoneNumber(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<List<AspnetUserDetails>>(employeeSparkModel, invokeGetAspnetUserByPhoneNumber);
        }

        //Get employee details by employeeid for sms spark
        public EmployeeModel GetEmployeeDetailsByEmployeeId(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<EmployeeModel>(employeeSparkModel, invokeGetEmployeeByEmployeeId);
        }

        //Get list of Trigger Admins for mail send
        public List<EmployeeModel> GetTriggerAdminList(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeSparkModel, invokeGetTriggerAdminList);
        }

        //Get list of Company Admins of Company for mail send 
        public List<EmployeeModel> GetCompanyAdminList(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<List<EmployeeModel>>(employeeSparkModel, invokeGetComapnyAdminList);
        }

        //Get employee spark rejection details for particular spark by EmpId & SparkId
        public SparkRejectionDetails GetSparkRejectionDetails(EmployeeSparkModel employeeSparkModel)
        {
            return ExecuteQuery<SparkRejectionDetails>(employeeSparkModel, invokeGetSparkRejectionList);
        }
    }
}
