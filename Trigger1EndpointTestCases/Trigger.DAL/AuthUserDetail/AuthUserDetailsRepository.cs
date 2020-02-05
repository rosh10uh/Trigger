using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DAL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.AuthUserDetail
{
    /// <summary>
    /// Contains method to Get auth user details
    /// </summary>
    [QueryPath("Trigger.DAL.Query.AuthUserDetail.AuthUserDetail")]
    public class AuthUserDetailsRepository : DaoRepository<AuthUserDetails>
    {
        /// <summary>
        /// Method to auth sub id by email
        /// </summary>
        /// <param name="authUserDetails"></param>
        /// <returns></returns>
        public virtual AuthUserDetails GetSubIdByEmail(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, SPFileName.GetSubIdByEmail);
        }

        /// <summary>
        /// Method to get passwordhash by userid
        /// </summary>
        /// <param name="authUserDetails"></param>
        /// <returns></returns>
        public virtual AuthUserDetails GetPasswordHashByUserId(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, SPFileName.GetPasswordHashByUserName);
        }

        /// <summary>
        /// Method to auth user details by email
        /// </summary>
        /// <param name="authUserDetails"></param>
        /// <returns></returns>
        public virtual AuthUserDetails GetAuthDetailsByEmail(AuthUserDetails authUserDetails)
        {
            return ExecuteQuery<AuthUserDetails>(authUserDetails, SPFileName.GetAuthDetailsByEmail);
        }
    }
}
