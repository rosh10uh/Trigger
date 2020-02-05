using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.EmployeeProfile
{
    /// <summary>
    /// Class to manage employee profile
    /// </summary>
    [QueryPath("Trigger.DAL.Query.EmployeeProfile.EmployeeProfile")]
    public class EmployeeProfileRepository : DaoRepository<EmployeeProfileModel>
    {

        /// <summary>
        /// Method to update allow sms flag
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <returns></returns>
        public virtual EmployeeProfileModel UpdateAllowSMS(EmployeeProfileModel employeeProfile)
        {
            return ExecuteQuery<EmployeeProfileModel>(employeeProfile, SPFileName.InvokeUpdateSmsService);
        }
    }
}
