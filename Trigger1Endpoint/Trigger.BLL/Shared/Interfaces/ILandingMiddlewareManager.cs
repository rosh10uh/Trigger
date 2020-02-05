using Trigger.DTO;

namespace Trigger.BLL.Shared.Interfaces
{
    public interface ILandingMiddlewareManager
    {
        UserDataModel CheckUserLogin(string connectionString, string userName);
    }
}
