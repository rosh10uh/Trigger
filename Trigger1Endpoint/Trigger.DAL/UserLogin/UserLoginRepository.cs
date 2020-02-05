using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.UserLoginRepo
{
    [QueryPath("Trigger.DAL.Query.UserLogin.UserLogin")]
    public class UserLoginRepository : DaoRepository<UserLogin>
    {
        private const string invokeGetUserDetails = "GetUserDetails";

        public UserLogin GetUserDetails(UserLogin userLogin)
        {
            return ExecuteQuery<UserLogin>(userLogin, invokeGetUserDetails);
        }        
    }
}
