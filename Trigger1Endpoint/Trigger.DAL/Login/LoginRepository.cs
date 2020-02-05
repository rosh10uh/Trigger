using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.DTO;

namespace Trigger.DAL.Login
{
    [QueryPath("Trigger.DAL.Query.Login.Login")]
    public class LoginRepository : DaoRepository<UserDataModel>
    {
        public const string invokeLogin = "Login";

        public UserDataModel InvokeLogin(UserDataModel userDataModel)
        {
            return ExecuteQuery<UserDataModel>(userDataModel, invokeLogin);
        }
    }
}
