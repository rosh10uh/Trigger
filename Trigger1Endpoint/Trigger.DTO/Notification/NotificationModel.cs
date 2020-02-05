using System;

namespace Trigger.DTO
{
    public class NotificationModel
    {
        public int id { get; set; }
        public int empId { get; set; }
        public string message { get; set; }
        public int managerId { get; set; }
        public string action { get; set; }
        public bool isSent { get; set; }
        public bool markAs { get; set; }
        public int createdBy { get; set; }
        public DateTime createdDtStamp { get; set; }
        public string type { get; set; }
        public int result { get; set; }
        public string ids { get; set; }
    }
}
