using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;


namespace Trigger.DAL.ChangePassword
{
    [QueryPath("Trigger.DAL.Query.ChangePassword.ChangePassword")]
    public class ChangePasswordRepository : DaoRepository<UserChangePassword>
    {

        private const string ChangeAuthPassword = "ChangeAuthPassword";
   
        public virtual UserChangePassword invokeChangeAuthPassword(UserChangePassword userChangePassword)
        {
            return ExecuteQuery<UserChangePassword>(userChangePassword, ChangeAuthPassword);
        }
    }
}
