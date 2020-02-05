using OneRPP.Restful.Contracts.Services;
using System.Security.Claims;

namespace Trigger.BLL.Shared.Interfaces
{
    public interface IClaims
    {
        Claim this[string claimName] { get; }
        string UserId { get; }
        string ClientId { get; }
        string Role { get; }
    }
}
