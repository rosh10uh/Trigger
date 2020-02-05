using System;
using System.Collections.Generic;

namespace Trigger.DTO
{
    public class WidgetLibrary
    {
        public int userId { get; set; }
        public int widgetId { get; set; }
        public string widgetName { get; set; }
        public string widgetActualName { get; set; }
        public Int64 sequenceNumber { get; set; }
        public Int64 tileSequence { get; set; }
        public Nullable<decimal> position { get; set; }
        public bool isActive { get; set; }
        public int isSelected { get; set; }
        public int roleId { get; set; }
        public int createdBy { get; set; }
        public int result { get; set; }
        public int widgetType { get; set; }

    }

    public class WidgetByDashboardType
    {
        public string dashBoardType { get; set; }
        public string widgetName { get; set; }
        public int dashBoardTypeId { get; set; }
        public int widgetId { get; set; }
        public string widgetActualName { get; set; }
        public int roleId { get; set; }
        public int selected { get; set; }
        public int companyId { get; set; }
    }

    public class WidgetType
    {
        public int widgetTypeId { get; set; }
        public string widgetTypeName { get; set; }
        public List<DTO.WidgetLibrary> lstWidget { get; set; }

        public WidgetType()
        {
            lstWidget = new List<DTO.WidgetLibrary>();

        }
    }

    public class RoleWiseWidget
    {
        public int updatedBy { get; set; }
        public int companyId { get; set; }
        public int roleId { get; set; }
        public int widgetId { get; set; }
        public int widgetTypeId { get; set; }
    }

}
