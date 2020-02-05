using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.AuthUserDetail
{
    [QueryPath("Trigger.DAL.Query.AuthUserDetail.AuthUserDetail")]
    public class AuthUserDetailsRepository : DaoRepository<AuthUserDetails>
    {
        private const string invokeGetSubIdByEmail = "GetSubIdByEmail";
        private const string GetPasswordHashByUserName = "GetPasswordHashByUserName";
        private const string invokeGetAuthDetailsByEmail = "GetAuthDetailsByEmail";

        public AuthUserDetails GetSubIdByEmail(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, invokeGetSubIdByEmail);
        }
        public AuthUserDetails invokeGetPasswordHashByUserId(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, GetPasswordHashByUserName);
        }
        public AuthUserDetails GetAuthDetailsByEmail(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, invokeGetAuthDetailsByEmail);
        }
    }
}
