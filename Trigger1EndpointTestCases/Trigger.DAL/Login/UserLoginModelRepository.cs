using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using Trigger.BLL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.Login
{
    [QueryPath("Trigger.DAL.Query.Login.Login")]
    public class UserLoginModelRepository : DaoRepository<UserLoginModel>
    {
        public const string invokeRegisterDevice = "RegisterDevice";
        private const string DeleteDeviceByLoginUserId = "DeleteDeviceByLoginUserId";
        public virtual UserLoginModel RegisterUserDeviceInfo(UserLoginModel userLoginModel)
        {
            return ExecuteQuery<UserLoginModel>(userLoginModel, invokeRegisterDevice);
        }

        public virtual UserLoginModel invokeDeleteDeviceInfo(UserLoginModel userLoginModel)
        {
            return ExecuteQuery<UserLoginModel>(userLoginModel, Messages.invokeDeleteDeviceInfo);
        }
        
        public virtual UserLoginModel invokeDeleteDeviceByLoginUserId(UserLoginModel userChangePassword)
        {
            return ExecuteQuery<UserLoginModel>(userChangePassword, DeleteDeviceByLoginUserId);
        }
    }
}
