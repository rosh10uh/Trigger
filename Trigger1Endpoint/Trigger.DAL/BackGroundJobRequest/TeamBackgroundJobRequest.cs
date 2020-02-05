using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.DTO.TeamConfiguration;
using Trigger.Utility;

namespace Trigger.DAL.BackGroundJobRequest
{
    /// <summary>
    /// Class Name   :   TeamBackgroundJobRequest
    /// Author       :   Mayur Patel
    /// Creation Date:   16 september 2019
    /// Purpose      :   Create hangfire recuring job 
    /// Revision     :  
    /// </summary>
    public class TeamBackgroundJobRequest
    {
        private readonly ILogger<TeamBackgroundJobRequest> _logger;
        private readonly AppSettings _appSettings;
        private readonly int _mailHours;
        private readonly int _mailMinutes;

        public TeamBackgroundJobRequest(ILogger<TeamBackgroundJobRequest> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            string mailTime = _appSettings.MailScheduleTime;
            if (!string.IsNullOrEmpty(mailTime))
            {
                string[] mailHours = mailTime.Split(":");
                if (mailHours.Length > 1)
                {
                    _mailHours = Convert.ToInt32(mailHours[0].ToString());
                    _mailMinutes = Convert.ToInt32(mailHours[1].ToString());
                }
                else
                {
                    _mailHours = Convert.ToInt32(mailHours[0].ToString());
                    _mailMinutes = 0;
                }
            }
        }

        /// <summary>
        /// Method for create job for send team create email notification 
        /// </summary>
        /// <returns></returns>
        public void SendTeamNotification(List<EmployeeBasicModel> employeeModel, TeamConfigurationModel teamConfigurationModel)
        {
            teamConfigurationModel.TeamEmployees = null;
            teamConfigurationModel.TeamManagers = null;
            BackgroundJob.Enqueue(() => SendTeamNotificationMail(employeeModel, teamConfigurationModel));
        }

        /// <summary>
        /// Method for create job for send team create email notification 
        /// </summary>
        /// <returns></returns>
        public void SendNotificationOnTeamConfigChange(List<EmployeeBasicModel> employeeModel, List<EmployeeBasicModel> newManagers, List<EmployeeBasicModel> removedManagers, TeamConfigurationModel teamConfigurationModel)
        {
            teamConfigurationModel.TeamEmployees = null;
            teamConfigurationModel.TeamManagers = null;
            BackgroundJob.Enqueue(() => SendEmailNotificationOnTeamConfigUpdate(employeeModel, newManagers, removedManagers, teamConfigurationModel));
        }
               
        /// <summary>
        /// Method for create job for send team Inactive email notification 
        /// </summary>
        /// <returns></returns>
        public void SendInactiveTeamNotification(List<EmployeeBasicModel> employeeModel, TeamConfigurationModel teamConfigurationModel)
        {
            teamConfigurationModel.TeamEmployees = null;
            teamConfigurationModel.TeamManagers = null;
            BackgroundJob.Enqueue(() => SendInactiveTeamNotificationMail(employeeModel, teamConfigurationModel));
        }


        /// <summary>
        /// Method for send team Creation email notification 
        /// </summary>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void SendTeamNotificationMail(List<EmployeeBasicModel> employeeModel, TeamConfigurationModel teamConfigurationModel)
        {
            var teamManagersEmailIds = string.Join(',', employeeModel.Select(x => x.Email).ToList());
            int companyId = employeeModel.Select(x => x.Companyid).FirstOrDefault();
            string managersList = PrepareEmpListForTeamEmailNotification(ConvertToDataTable.ToDataTable(employeeModel));
            try
            {
                string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamConfigurationEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);

                body = body.Replace(EmailTemplateFields.TeamName, teamConfigurationModel.Name);
                body = body.Replace(EmailTemplateFields.TeamStartDate, teamConfigurationModel.StartDate.ToLongDateString());
                body = body.Replace(EmailTemplateFields.TeamEndDate, teamConfigurationModel.EndDate == Convert.ToDateTime("01-01-1900 12:00:00 AM") ? "Never End" : teamConfigurationModel.EndDate.ToLongDateString());
                body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());
                body = body.Replace(EmailTemplateFields.ManagerList, managersList);
                body = body.Replace(EmailTemplateFields.TeamDescription, teamConfigurationModel.Description);
                body = body.Replace(EmailTemplateFields.TriggerActivity, teamConfigurationModel.TriggerActivityDays.ToString());

                try
                {
                    EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(Messages.TeamConfiguration, teamConfigurationModel.Name), body, string.Empty, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Method for send team Creation email notification 
        /// </summary>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void SendEmailNotificationOnTeamConfigUpdate(List<EmployeeBasicModel> employeeModel, List<EmployeeBasicModel> newManagers, List<EmployeeBasicModel> removedManagers, TeamConfigurationModel teamConfigurationModel)
        {
            var teamManagersEmailIds = string.Join(',', employeeModel.Select(x => x.Email).ToList());
            int companyId = employeeModel.Select(x => x.Companyid).FirstOrDefault();

            string removedManagersList = string.Empty;

            try
            {
                string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamConfigurationUpdateEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);

                body = body.Replace(EmailTemplateFields.TeamName, teamConfigurationModel.Name);
                body = body.Replace(EmailTemplateFields.TeamStartDate, teamConfigurationModel.StartDate.ToLongDateString());
                body = body.Replace(EmailTemplateFields.TeamEndDate, teamConfigurationModel.EndDate == Convert.ToDateTime("01-01-1900 12:00:00 AM") ? "Never End" : teamConfigurationModel.EndDate.ToLongDateString());
                body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());
                body = body.Replace(EmailTemplateFields.TeamDescription, teamConfigurationModel.Description);
                body = body.Replace(EmailTemplateFields.TriggerActivity, teamConfigurationModel.TriggerActivityDays.ToString());

                if (newManagers.Count > 0)
                {
                    string newManagersList = PrepareEmpListForTeamEmailNotification(ConvertToDataTable.ToDataTable(newManagers));
                    body = body.Replace(EmailTemplateFields.NewManagerList, EmailTemplateFields.NewManagerListHeader + newManagersList);
                }
                else
                {
                    body = body.Replace(EmailTemplateFields.NewManagerList, string.Empty);
                }

                if (removedManagers.Count > 0)
                {
                    removedManagersList = PrepareEmpListForTeamEmailNotification(ConvertToDataTable.ToDataTable(removedManagers));
                    body = body.Replace(EmailTemplateFields.RemovedManagerList, EmailTemplateFields.RemovedManagerListHeader + removedManagersList);
                }
                else
                {
                    body = body.Replace(EmailTemplateFields.RemovedManagerList, string.Empty);
                }

                try
                {
                    EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(Messages.TeamConfigurationUpdate, teamConfigurationModel.Name), body, string.Empty, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Method for send team inactive email notification 
        /// </summary>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void SendInactiveTeamNotificationMail(List<EmployeeBasicModel> employeeModel, TeamConfigurationModel teamConfigurationModel)
        {
            var teamManagersEmailIds = string.Join(',', employeeModel.Select(x => x.Email).ToList());
            int companyId = employeeModel.Select(x => x.Companyid).FirstOrDefault();
            //foreach (EmployeeBasicModel employees in employeeModel)
            //{
            try
            {
                string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamInactiveEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);
                body = body.Replace(EmailTemplateFields.TeamName, teamConfigurationModel.Name);
                body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());

                try
                {
                    EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(Messages.TeamConfigurationUpdate, teamConfigurationModel.Name), body, string.Empty, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            // }
        }

        /// <summary>
        /// Method for create recuring job request
        /// </summary>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void ScheduleTeamNotiifcationJob()
        {
            string recurringJobId = "TeamNotificationJob";
            //Executes as per UTC Datae Time default, set TimeZoneInfo.Local for local time
            RecurringJob.AddOrUpdate(recurringJobId.ToString(), () => SendEmailNotification(), Cron.Daily(_mailHours, _mailMinutes));//Cron.MinuteInterval(30)
        }

        /// <summary>
        /// Method for called by hangfire on recuing job call
        /// Check all company's teams inactivity employee, team expiring date  and team expired date and based on then send email
        /// </summary>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void SendEmailNotification()
        {
            try
            {
                DataTable companyDetails = SqlHelper.GetDataByStoreProcedure(Messages.GetCompanyDetailsForTeamNotification, _appSettings.DefaultConnection.ConnectionString);

                if (companyDetails != null && companyDetails.Rows.Count > 0)
                {
                    DateTime teamStartDate;
                    DateTime teamEndDate;
                    int teamId;
                    string tenantConnString;
                    DataTable teamsDetails;
                    int companyId;
                    string teamName;
                    int triggerActivityDays;
                    foreach (DataRow company in companyDetails.Rows)
                    {
                        try
                        {
                            tenantConnString = string.Format(_appSettings.TenantConnectionString.ConnectionString, company["tenantName"]);
                            teamsDetails = SqlHelper.GetDataByStoreProcedure(Messages.GetTeamDetailsForTeamNotification, tenantConnString);
                            companyId = Convert.ToInt32(company["compid"]);

                            foreach (DataRow team in teamsDetails.Rows)
                            {
                                teamStartDate = Convert.ToDateTime(team["StartDate"]);
                                teamEndDate = string.IsNullOrEmpty(team["EndDate"].ToString()) ? Convert.ToDateTime("01-01-1900 12:00:00 AM") : Convert.ToDateTime(team["EndDate"]);
                                teamId = Convert.ToInt32(team["TeamId"]);
                                teamName = team["Name"].ToString();
                                triggerActivityDays = Convert.ToInt32(team["TriggerActivityDays"]);

                                if (teamStartDate.AddDays(triggerActivityDays).Date < DateTime.Now.Date)
                                {
                                    SendTeamInactivityEmailNotification(teamId, triggerActivityDays, teamName, companyId, tenantConnString);
                                }

                                if (teamEndDate != Convert.ToDateTime("01-01-1900 12:00:00 AM") && DateTime.Today.AddDays(7) == teamEndDate)
                                {
                                    SendTeamExpiringNotification(teamId, companyId, tenantConnString, teamName, teamEndDate.ToLongDateString());
                                }
                                else if (teamEndDate != Convert.ToDateTime("01-01-1900 12:00:00 AM") && DateTime.Today.AddDays(-1) >= teamEndDate)
                                {
                                    SqlHelper.InActiveTeam(teamId, tenantConnString);
                                    SendTeamExpiredNotification(teamId, companyId, tenantConnString, teamName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method of send email notification to team managers for team employee spark is pending
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="triggerActivityDays"></param>
        /// <param name="teamName"></param>
        /// <param name="companyId"></param>
        /// <param name="tenantConnString"></param>
        /// <returns></returns>
        public void SendTeamInactivityEmailNotification(int teamId, int triggerActivityDays, string teamName, int companyId, string tenantConnString)
        {
            DataTable teamInactivityEmployee = SqlHelper.GetTeamInactivityEmployeByTeamId(teamId, tenantConnString, triggerActivityDays);

            if (teamInactivityEmployee != null && teamInactivityEmployee.Rows.Count > 0)
            {
                string teamManagersEmailIds = SqlHelper.GetTeamManagersEmailIds(teamId, tenantConnString);
                if (!string.IsNullOrEmpty(teamManagersEmailIds))
                {
                    string employeeList = PrepareEmpListForTeamEmailNotification(teamInactivityEmployee);

                    string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamInactivityEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);
                    body = body.Replace(EmailTemplateFields.TeamName, teamName);
                    body = body.Replace(EmailTemplateFields.TriggerActivity, triggerActivityDays.ToString());
                    body = body.Replace(EmailTemplateFields.EmployeeList, employeeList);
                    body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());

                    EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(EmailTemplateFields.TeamInActivityEmailSubject, teamName), body, string.Empty, true);

                    TeamInActivityLogModel teamInActivityLogModel = new TeamInActivityLogModel
                    {
                        TeamId = teamId,
                        EmailTo = teamManagersEmailIds,
                        EmailText = body,
                        TriggerActivitydays = triggerActivityDays,
                        CreatedBy = 1
                    };
                    SqlHelper.AddTeamInActivityLog(tenantConnString, teamInActivityLogModel);
                }
            }
        }

        /// <summary>
        /// Method of send email notification to team managers for team will be expiring after week
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="companyId"></param>
        /// <param name="tenantConnString"></param>
        /// <param name="teamName"></param>
        /// <param name="teamEndDate"></param>
        /// <returns></returns>
        public void SendTeamExpiringNotification(int teamId, int companyId, string tenantConnString, string teamName, string teamEndDate)
        {
            string teamManagersEmailIds = SqlHelper.GetTeamManagersEmailIds(teamId, tenantConnString);

            if (!string.IsNullOrEmpty(teamManagersEmailIds))
            {
                string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamExpiringEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);
                body = body.Replace(EmailTemplateFields.TeamName, teamName);
                body = body.Replace(EmailTemplateFields.TeamEndDate, teamEndDate);
                body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());

                EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(EmailTemplateFields.TeamExpiringEmailSubject, teamName), body, string.Empty, true);
            }
        }

        /// <summary>
        /// Method of send email notification to team managers for team has been expired 
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="companyId"></param>
        /// <param name="tenantConnString"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public void SendTeamExpiredNotification(int teamId, int companyId, string tenantConnString, string teamName)
        {
            string teamManagersEmailIds = SqlHelper.GetTeamManagersEmailIds(teamId, tenantConnString);

            if (!string.IsNullOrEmpty(teamManagersEmailIds))
            {
                DataTable teamEmployee = SqlHelper.GetTeamEmployeeDetailsByTeamId(teamId, tenantConnString, Messages.GetTeamEmployeesDetailsByTeamId);
                DataTable teamManagers = SqlHelper.GetTeamEmployeeDetailsByTeamId(teamId, tenantConnString, Messages.GetTeamManagersDetailsByTeamId);

                string managersList = PrepareEmpListForTeamEmailNotification(teamManagers);

                string body = SqlHelper.GetTemplateByName(EmailTemplateFields.TeamExpiredEmailTemplate, companyId, _appSettings.DefaultConnection.ConnectionString);
                body = body.Replace(EmailTemplateFields.TeamName, teamName);
                body = body.Replace(EmailTemplateFields.ManagerList, managersList);
                if (teamEmployee != null && teamEmployee.Rows.Count > 0)
                {
                    string employeeList = PrepareEmpListForTeamEmailNotification(teamEmployee);
                    body = body.Replace(EmailTemplateFields.EmployeeList, EmailTemplateFields.EmployeeListHeadr + employeeList);
                }
                else
                {
                    body = body.Replace(EmailTemplateFields.EmployeeList, string.Empty);
                }

                body = body.Replace(EmailTemplateFields.CurrentYear, DateTime.Now.Year.ToString());

                EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), teamManagersEmailIds, string.Format(EmailTemplateFields.TeamExpiredEmailSubject, teamName), body, string.Empty, true);
            }
        }

        /// <summary>
        /// Method of prepare team manager/employeelist
        /// </summary>
        /// <param name="employeelist"></param>
        /// <returns></returns>
        public string PrepareEmpListForTeamEmailNotification(DataTable employeelist)
        {
            int teamMemberNo = 1;
            string employeeName;
            string employeeTemplate;
            StringBuilder employeeList = new StringBuilder();

            foreach (DataRow dr in employeelist.Rows)
            {
                employeeName = string.Concat(dr["firstname"].ToString(), (char)32, dr["lastname"].ToString());
                employeeTemplate = EmailTemplateFields.HtmlForEmployeeList;
                employeeTemplate = employeeTemplate.Replace(EmailTemplateFields.EmpNo, teamMemberNo.ToString());
                employeeTemplate = employeeTemplate.Replace(EmailTemplateFields.EmployeeName, employeeName);
                employeeList.Append(employeeTemplate);
                teamMemberNo = teamMemberNo + 1;
            }

            return employeeList.ToString();
        }
    }
}
