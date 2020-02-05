
namespace Trigger.DAL.Shared
{
    /// <summary>
    /// Class Name      : EmailTemplateFields
    /// Author          : Mayur Patel
    /// Creation Date   : 16 september 2019
    /// Purpose         : Class to manage email template name and replacing field
    /// Revision        : 
    /// </summary>
    public static class EmailTemplateFields
    {
        // Email Template Name
        public const string TeamExpiringEmailTemplate = "TeamExpiringEmailTemplate";
        public const string TeamExpiredEmailTemplate = "TeamExpiredEmailTemplate";
        public const string TeamInactivityEmailTemplate = "TeamInactivityEmailTemplate";
        public const string TeamConfigurationEmailTemplate = "TeamConfigurationEmailTemplate";
        public const string TeamConfigurationUpdateEmailTemplate = "TeamConfigurationUpdateEmailTemplate";
        public const string TeamInactiveEmailTemplate = "TeamInactiveEmailTemplate";

        //Email subject name
        public const string TeamExpiringEmailSubject = "Trigger Transformation | Team Expiring Reminder - {0}";
        public const string TeamExpiredEmailSubject = "Trigger Transformation | Team Expired Alert - {0}";
        public const string TeamInActivityEmailSubject = "Trigger Transformation | Team InActivity Alert - {0}";


        //Email template fieldname

        // Employee registration email field
        public const string HeadingText = "{Headingtext}";
        public const string UserId = "{UserId}";
        public const string LandingURL = "{LandingURL}";
        public const string CurrentYear = "{CurrentYear}";

        //Reset password email 
        public const string ResetPasswordURL = "{ResetPasswordURL}";

        //Team Notification
        public const string TeamName = "{TeamName}";
        public const string TeamStartDate = "{TeamStartDate}";
        public const string TeamEndDate = "{TeamEndDate}";
        public const string ManagerList = "{ManagerList}";
        public const string NewManagerListHeader = " <tr>  <td><strong>New Team Managers</strong></td> </tr> ";
        public const string NewManagerList = "{NewManagerList}";
        public const string RemovedManagerListHeader = "<tr>  <td><strong> Team Managers Removed</strong></td>   </tr>";
        public const string RemovedManagerList = "{RemovedManagerList}";
        public const string EmployeeList = "{EmployeeList}";
        public const string EmployeeListHeadr = "<tr><td><strong>Team Members</strong></td></tr>";

        public const string EmpNo = "{No}";
        public const string EmployeeName = "{EmployeeName}";
        public const string TriggerActivity = "{TriggerActivity}";
        public const string TeamDescription = "{TeamDescription}";
        public const string headingtext = "{HeadingText}";
        public const string HtmlForEmployeeList = "<tr><td style='font-size: 14px; font-weight: 500'>{No}. {EmployeeName}</td> </tr>";
    }
}
