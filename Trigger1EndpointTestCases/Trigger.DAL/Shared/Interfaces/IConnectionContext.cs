using Trigger.DAL;

namespace Trigger.DAL.Shared.Interfaces
{
    public interface IConnectionContext
    {
        TriggerContext TriggerContext { get; }
    }
}
