using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.UserDetail
{
    [QueryPath("Trigger.DAL.Query.UserDetail.UserDetail")]
    public class UserDetailRepository : DaoRepository<UserDetails>
    {
        private const string invokeAddUser = "AddUser";
        private const string invokeUpdateUser = "UpdateUser";        
        private const string invokeGetUserDetails = "GetUserDetails";

        public virtual UserLogin AddUser(UserDetails userDetails)
        {
            return ExecuteQuery<UserLogin>(userDetails, invokeAddUser);
        }

        public virtual UserLogin UpdateUser(UserDetails userDetails)
        {
            return ExecuteQuery<UserLogin>(userDetails, invokeUpdateUser);
        }        

        public virtual UserDetails GetUserDetails(UserDetails userDetails)
        {
            return ExecuteQuery<UserDetails>(userDetails, invokeGetUserDetails);
        }

    }
}
