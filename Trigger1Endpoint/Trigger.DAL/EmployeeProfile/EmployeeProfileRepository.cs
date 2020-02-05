using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;

namespace Trigger.DAL.EmployeeProfile
{
    [QueryPath("Trigger.DAL.Query.EmployeeProfile.EmployeeProfile")]
    public class EmployeeProfileRepository : DaoRepository<DTO.EmployeeProfile>
    {
        private const string invokeUpdateSmsService = "UpdateOptForSMS";

        public DTO.EmployeeProfile UpdateAllowSMS(DTO.EmployeeProfile employeeProfile)
        {
            return ExecuteQuery<DTO.EmployeeProfile>(employeeProfile, invokeUpdateSmsService);
        }
    }
}
