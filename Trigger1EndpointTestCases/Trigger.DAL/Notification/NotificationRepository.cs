using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.DTO;

namespace Trigger.DAL.NotificationRepo
{    
    [QueryPath("Trigger.DAL.Query.Notification.Notification")]
    public class NotificationRepository : DaoRepository<NotificationModel>    
    {
        private const string invokeUpdateNotificationFlagIsSent = "UpdateNotificationFlagIsSent";
        public const string invokeUpdateNotificationMarkAsRead = "UpdateNotificationMarkAsRead";
        public const string invokeGetAllNotification = "GetAllNotification";

        public virtual NotificationModel UpdateNotificationFlagIsSent(NotificationModel notification)
        {
            return ExecuteQuery<NotificationModel>(notification, invokeUpdateNotificationFlagIsSent);
        }

        public virtual NotificationModel InvokeUpdateNotificationStatus(NotificationModel notification)
        {
            return ExecuteQuery<NotificationModel>(notification, invokeUpdateNotificationMarkAsRead);
        }

        public virtual List<NotificationModel> GetAllNotification(NotificationModel notification)
        {
            return ExecuteQuery<List<NotificationModel>>(notification, invokeGetAllNotification);
        }

    }
}
