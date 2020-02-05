using System.Threading.Tasks;

namespace Trigger.Utility
{
    public interface ISmsSender
    {
         Task<int> SendSmsAsync(string number, string message);
    }
}
