using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Trigger.BLL.Shared;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.DTO.InActivityManager;
using Trigger.DTO.TeamConfiguration;
using Trigger.Utility;

namespace Trigger.DAL.BackGroundJobRequest
{
    public class BackgroundJobRequest
    {

        private readonly ILogger<BackgroundJobRequest> _logger;
        private readonly string _catalogConnectionString;
        private readonly string _fromMailId;
        private readonly string _triggerSupportMailId;
        private readonly string _clientHost;
        private readonly int _portNo;
        private readonly string _password;
        private readonly string _landingURL;
        private readonly int _mailHours;
        private readonly int _mailMinutes;
        private readonly ISmsSender _smsSender;


        public BackgroundJobRequest(ILogger<BackgroundJobRequest> logger, IHttpContextAccessor contextAccessor, EmployeeContext employeeContext, ISmsSender smsSender)
        {
            _logger = logger;
            _catalogConnectionString = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.ConnectionString.ToString()];
            _fromMailId = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SenderEmailID.ToString()];
            _triggerSupportMailId = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TriggerSupportEMail.ToString()];
            _clientHost = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SmtpServer.ToString()];
            _portNo = Convert.ToInt32(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SmtpPort.ToString()]);
            _password = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.SenderPassword.ToString()];
            _landingURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.LandingURL.ToString()];
            _smsSender = smsSender;

            string mailTime = Shared.Dictionary.ConfigDictionary[Shared.Dictionary.ConfigurationKeys.MailScheduleTime.ToString()];
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

        public void AddCompanyAdmin(EmployeeModel employeeAdmin, PerformContext context, EmployeeContext _employeeContext)
        {
            try
            {
                _employeeContext.InsertCompanyAdmin(employeeAdmin, _catalogConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());

            }
        }

        public void DeleteCompanyAdmin(EmployeeModel employeeAdmin, PerformContext context, EmployeeContext _employeeContext)
        {
            try
            {
                _employeeContext.DeleteCompanyAdmin(employeeAdmin, _catalogConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void SendEmail(EmployeeModel employee, string url, PerformContext performContext)
        {
            string body = GetTemplateByName(Messages.userRegEmailTemplate, employee.companyid);

            if (!string.IsNullOrEmpty(employee.firstName))
                body = body.Replace(Messages.headingtext, employee.firstName);
            body = body.Replace(Messages.userId, employee.email);
            body = body.Replace(Messages.resetPasswordURL, url);
            body = body.Replace(Messages.landingURL, _landingURL);

            string recurringJobId = employee.companyid.ToString() + GenerateRandomNumber().ToString();

            //Executes as per UTC Datae Time default, set TimeZoneInfo.Local for local time
            RecurringJob.AddOrUpdate(recurringJobId.ToString(), () => SendMailOnContractStart(recurringJobId, employee.companyid, employee.email, employee.empId, body, null), Cron.Daily(_mailHours, _mailMinutes));//Cron.MinuteInterval(5)

        }

        public int GenerateRandomNumber()
        {
            Random randomNumber = new Random();
            return randomNumber.Next(100, 100000);
        }

        /// <summary>
        /// Use to add company admin details from Csv
        /// </summary>
        /// <param name="dtbEmployee"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public void AddCompanyAdminFromCsv(DataTable dtbEmployee, PerformContext context)
        {
            try
            {
                AddCompanyAdminFromCsv(dtbEmployee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void AddCompanyAdminFromCsv(DataTable dtbEmployee)
        {
            try
            {
                var sqlParameter = new SqlParameter("@tblEmployee", dtbEmployee);
                ExecuteNonQueryWithDataTable(_catalogConnectionString, "usp_AddCompanyAdminFromCSV", sqlParameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        public void SendEmailToUpdatedUser(DataTable employee, int companyId)
        {
            string body = string.Empty;
            string emailBody = GetTemplateByName(Messages.userUpdateEmailTemplate, companyId);

            foreach (DataRow email in employee.Rows)
            {
                try
                {
                    body = emailBody;
                    body = body.Replace(Messages.headingtext, email["firstName"].ToString());
                    body = body.Replace(Messages.userId, email["email"].ToString());
                    body = body.Replace(Messages.landingURL, _landingURL);

                    BackgroundJob.Enqueue(() => SendMail(email["email"].ToString(), body, Messages.registration));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Use to execute non query with data table
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        [AutomaticRetry(Attempts = 0)]
        public static int ExecuteNonQueryWithDataTable(string connectionString, string commandText, SqlParameter commandParameters)
        {
            using (var sqlCommand = new SqlCommand())
            {
                var sqlConnection = new SqlConnection(connectionString);
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = commandText;
                commandParameters.SqlDbType = SqlDbType.Structured;
                sqlCommand.Parameters.AddWithValue(commandParameters.ParameterName, commandParameters.Value);
                var value = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                return value;
            }
        }

        public void SendRegistrationEmail(EmployeeModel employee, string url, string hostName, PerformContext performContext)
        {
            string body = GetTemplateByName(Messages.userRegEmailTemplate, employee.companyid);

            if (!string.IsNullOrEmpty(employee.firstName))
                body = body.Replace(Messages.headingtext, employee.firstName);
            body = body.Replace(Messages.userId, employee.email);
            body = body.Replace(Messages.resetPasswordURL, url);
            body = body.Replace(Messages.landingURL, _landingURL);

            try
            {
                EmailConfiguration.SendMail(_fromMailId, _password, _clientHost, _portNo, employee.email, Messages.registration, body, string.Empty, true);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw ex;
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void SendMail(string email, string body,string emailSubject)
        {
            EmailConfiguration.SendMail(_fromMailId, _password, _clientHost, _portNo, email, emailSubject, body, string.Empty, true);
        }

        [AutomaticRetry(Attempts = 0)]
        public void SendMailOnContractStart(string recurringJobId, int companyId, string email, int empId, string body, PerformContext context)
        {
            try
            {
                DateTime contractStartDate = GetContractStartDate(companyId);
                var tenantName = GetTenantName(companyId);
                bool isMailSent = GetIsMailSent(tenantName, companyId, empId);
                if (isMailSent)
                {
                    RecurringJob.RemoveIfExists(recurringJobId);
                }
                else
                {
                    if (contractStartDate == DateTime.Today.Date)
                    {
                        EmailConfiguration.SendMail(_fromMailId, _password, _clientHost, _portNo, email, Messages.registration, body, string.Empty, true);
                        UpdateIsMailSent(tenantName, companyId, empId);
                        RecurringJob.RemoveIfExists(recurringJobId);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }

        }

        public bool GetIsMailSent(string tenantName, int companyId, int empId)
        {
            var tenantConnString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], tenantName);
            SqlConnection sqlConnection = new SqlConnection(tenantConnString);
            bool isMailSent = false;

            try
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("SELECT ISNULL(isMailSent,0) as isMailSent FROM employeedetails WHERE bactive = 1 AND empstatus = 1 AND roleId <> 5 AND companyid = " + companyId + " AND empid = " + empId, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    isMailSent = (bool)sqlCommand.ExecuteScalar();
                }

                return isMailSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return isMailSent;
        }

        public void UpdateIsMailSent(string tenantName, int companyId, int empId)
        {
            var tenantConnString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], tenantName);
            SqlConnection sqlConnection = new SqlConnection(tenantConnString);
            sqlConnection.Open();
            using (var sqlCommand = new SqlCommand("UPDATE employeedetails set isMailsent = 1 WHERE companyId = " + companyId + " and empid = " + empId, sqlConnection))
            {
                sqlCommand.CommandTimeout = 0;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
        }

        public string GetTenantName(int companyId)
        {
            SqlConnection sqlConnection = new SqlConnection(_catalogConnectionString);
            string tenantName = string.Empty;

            try
            {

                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("SELECT dbname FROM companydbconfig WHERE bactive = 1 AND companyid = " + companyId, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    tenantName = (string)sqlCommand.ExecuteScalar();
                }


                return tenantName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return tenantName;
        }

        public DateTime GetContractStartDate(int companyId)
        {
            SqlConnection sqlConnection = new SqlConnection(_catalogConnectionString);
            DateTime companyContractStartDate = default(DateTime);

            try
            {

                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand("SELECT contractStartDate FROM companydetails WHERE bactive = 1 AND compid = " + companyId, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    companyContractStartDate = (DateTime)sqlCommand.ExecuteScalar();
                }

                return companyContractStartDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return companyContractStartDate;
        }

        public string GetTemplateByName(string templateName, int companyId)
        {
            string template = string.Empty;
            SqlConnection sqlConnection = new SqlConnection(_catalogConnectionString);
            try
            {
                using (var sqlCommand = new SqlCommand())
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = Messages.GetTemplateByName;
                    sqlCommand.Parameters.AddWithValue("@companyId", companyId);
                    sqlCommand.Parameters.AddWithValue("@templateName", templateName);
                    template = (string)sqlCommand.ExecuteScalar();
                    sqlCommand.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return template;
        }

        /// <summary>
        /// Method Name  :   ScheduleInActivityReminder
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Method to schedule Inactivity Reminder Mail & SMS
        /// Revision     :  Modify by Mayur Patel ||  Send InActivity email to all company's employee || 06 Jun 2019 
        /// </summary>
        /// <param name="hostName">path for trigger logo</param>
        /// <param name="inActivityManagers">get companyid & created by from class</param>
        [AutomaticRetry(Attempts = 0)]
        public void ScheduleInActivityReminder()
        {
            string recurringJobId = "InActivityReminderJob";
            //Executes as per UTC Datae Time default, set TimeZoneInfo.Local for local time
            RecurringJob.AddOrUpdate(recurringJobId.ToString(), () => SendInActivityMail(), Cron.Daily(_mailHours, _mailMinutes));//Cron.MinuteInterval(30)
        }

        /// <summary>
        /// Method Name  :   AddInActivityManagers
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Method to insert inactivity reminder log
        /// Revision     : 
        /// </summary>
        /// <param name="connectionString">database connection string</param>
        /// <param name="inActivityManagers">contain require parameters for inactivity log insert</param>
        /// <returns></returns>
        public int AddInActivityManagers(string connectionString, InActivityManagers inActivityManagers)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            int inactivityLog = 0;

            try
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand(Messages.usp_AddInactivityManagers, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(SetSqlParameter("@EmpID", ParameterDirection.Input, SqlDbType.Int, inActivityManagers.empId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@FirstName", ParameterDirection.Input, SqlDbType.NVarChar, inActivityManagers.firstName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@LastName", ParameterDirection.Input, SqlDbType.NVarChar, inActivityManagers.lastName));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Email", ParameterDirection.Input, SqlDbType.NVarChar, inActivityManagers.email));
                    sqlCommand.Parameters.Add(SetSqlParameter("@SmsText", ParameterDirection.Input, SqlDbType.NVarChar, inActivityManagers.smsText));
                    sqlCommand.Parameters.Add(SetSqlParameter("@EmailText", ParameterDirection.Input, SqlDbType.NVarChar, inActivityManagers.emailText));
                    sqlCommand.Parameters.Add(SetSqlParameter("@PhoneNumber", ParameterDirection.Input, SqlDbType.VarChar, inActivityManagers.phoneNumber));
                    sqlCommand.Parameters.Add(SetSqlParameter("@InActivityDays", ParameterDirection.Input, SqlDbType.Int, inActivityManagers.inActivityDays));
                    sqlCommand.Parameters.Add(SetSqlParameter("@CreatedBy", ParameterDirection.Input, SqlDbType.Int, inActivityManagers.createdBy));
                    sqlCommand.Parameters.Add(SetSqlParameter("@Result", ParameterDirection.InputOutput, SqlDbType.Int, inactivityLog));

                    inactivityLog = sqlCommand.ExecuteNonQuery();
                }

                return inactivityLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return inactivityLog;
        }

        /// <summary>
        /// Method Name  :   GetInActivityManagers
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Method to get list of managers,admin & executive who didn't triggered employee since inactivity days
        /// Revision     : 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="companyDetails">contain require parameters for getting list of inactivity managers</param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public DataTable GetInActivityManagers(CompanyDetailsModel companyDetails, string connString)
        {
            DataTable inactivityEmployees = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(connString);
            try
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(Messages.usp_GetInactivityManagers, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(SetSqlParameter("@CompanyId", ParameterDirection.Input, SqlDbType.Int, companyDetails.compId));
                    sqlCommand.Parameters.Add(SetSqlParameter("@InActivityDays", ParameterDirection.Input, SqlDbType.Int, companyDetails.inActivityDays));
                    sqlCommand.Parameters.Add(SetSqlParameter("@ReminderDays", ParameterDirection.Input, SqlDbType.Int, companyDetails.reminderDays));

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(inactivityEmployees);
                    sqlConnection.Close();
                    sqlDataAdapter.Dispose();

                    return inactivityEmployees;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return inactivityEmployees;
        }

        /// <summary>
        /// Method Name  :   GetCompanyDetails
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Method to get company details by company id
        /// Revision     : 
        /// </summary>
        /// <param name="companyId">companyid to get company details</param>
        /// <returns></returns>
        public DataTable GetCompanyDetails()
        {
            DataTable companyDetails = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(_catalogConnectionString);
            try
            {

                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(Messages.getCompanyDetails, sqlConnection))
                {
                    sqlCommand.CommandTimeout = 0;
                    sqlCommand.CommandType = CommandType.Text;

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(companyDetails);
                    sqlConnection.Close();
                    sqlDataAdapter.Dispose();

                    return companyDetails;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
            return companyDetails;
        }

        /// <summary>
        /// Method Name  :   SetSqlParameter
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Common Method to set sql parameter object for sql command
        /// Revision     : 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="parameterDirection"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter SetSqlParameter(string paramName, ParameterDirection parameterDirection, SqlDbType sqlDbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter(paramName, value)
            {
                Direction = parameterDirection,
                SqlDbType = sqlDbType
            };
            return sqlParameter;
        }

        /// <summary>
        /// Method Name  :   SendInActivityMail
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   01 May 2019
        /// Purpose      :   Method to send mail & sms to admin,manager & executive who had not triggered employee since inactivity days
        /// Revision     :   Modify by Mayur Patel ||  Send InActivity email to all company's employee || 06 Jun 2019 
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public void SendInActivityMail()
        {
            try
            {
                DataTable companyDetails = GetCompanyDetails();
                if (companyDetails != null && companyDetails.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in companyDetails.Rows)
                    {
                        try
                        {
                            CompanyDetailsModel companyDetailsModel = GetCompanyDetailsModel(dataRow);

                            string emailBody = GetTemplateByName(Messages.userInactivityEmailTemplate, companyDetailsModel.compId);
                            string smsBody = GetTemplateByName(Messages.userInactivitySmsTemplate, companyDetailsModel.compId);

                            int daysDiff = (DateTime.Today.Date - companyDetailsModel.contractStartDate).Days;
                            if (companyDetailsModel.contractStartDate <= DateTime.Today.Date && daysDiff >= companyDetailsModel.inActivityDays && (companyDetailsModel.contractEndDate.AddDays(companyDetailsModel.gracePeriod) >= DateTime.Today.Date))
                            {
                                var tenantName = GetTenantName(companyDetailsModel.compId);
                                var tenantConnString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], tenantName);

                                foreach (DataRow dr in GetInActivityManagers(companyDetailsModel, tenantConnString).Rows)
                                {
                                    InActivityManagers inActivityManager = GetInActivityModel(dr, companyDetailsModel.compId, smsBody, emailBody, 1);
                                    SendInactivityReminder(tenantConnString, inActivityManager);
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
        /// Method Name  :   SendInactivityReminder
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   14 May 2019
        /// Purpose      :   Method to send mail & sms to admin,manager & executive who had not triggered employee since inactivity days
        /// Revision     :
        /// </summary>
        /// <param name="tenantConnString">connection string for tenant</param>
        /// <param name="inActivityManager">dto object containing details of person to send reminder</param>
        public void SendInactivityReminder(string tenantConnString, InActivityManagers inActivityManager)
        {
            if (inActivityManager.isMailSent)
            {
                SendInActivityEmail(inActivityManager.email, inActivityManager.emailText);

                if (inActivityManager.phoneConfirmed && inActivityManager.optForSms)
                {
                    _smsSender.SendSmsAsync(inActivityManager.phoneNumber, inActivityManager.smsText);
                }
                else
                {
                    inActivityManager.phoneNumber = string.Empty;
                    inActivityManager.smsText = string.Empty;
                }

                AddInActivityManagers(tenantConnString, inActivityManager);
            }
        }

        /// <summary>
        /// Method Name  :   SendInActivityEmail
        /// Author       :   Mayur patel
        /// Creation Date:   26 Jun 2019
        /// Purpose      :   Method to send inactivity email 
        /// Revision     :  
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailText"></param>
        public void SendInActivityEmail(string email, string emailText)
        {
            try
            {
                EmailConfiguration.SendMail(_fromMailId, _password, _clientHost, _portNo, email, Messages.inActivityReminder, emailText, string.Empty, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method Name  :   GetCompanyDetailsModel
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   08 May 2019
        /// Purpose      :   Method to generate model for company detail
        /// Revision     :  
        /// </summary>
        /// <param name="companyDetails"></param>
        /// <returns></returns>
        public CompanyDetailsModel GetCompanyDetailsModel(DataRow companyDetails)
        {
            return new CompanyDetailsModel
            {
                compId = Convert.ToInt32(companyDetails["compid"]),
                contractStartDate = Convert.ToDateTime(companyDetails["contractstartdate"]),
                contractEndDate = Convert.ToDateTime(companyDetails["contractenddate"]),
                inActivityDays = Convert.ToInt32(companyDetails["inactivitydays"]),
                reminderDays = Convert.ToInt32(companyDetails["reminderdays"]),
                gracePeriod = Convert.ToInt32(companyDetails["graceperiod"])
            };
        }

        /// <summary>
        /// Method Name  :   GetInActivityModel
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   08 May 2019
        /// Purpose      :   Method to generate model for Inactivity employees
        /// Revision     :   
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="companyId"></param>
        /// <param name="smsBody"></param>
        /// <param name="emailBody"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public InActivityManagers GetInActivityModel(DataRow dr, int companyId, string smsBody, string emailBody, int createdById)
        {
            string emailText = emailBody.Replace(Messages.headingtext, Convert.ToString(dr["firstname"]));
            emailText = emailText.Replace(Messages.inActivityDays, Convert.ToString(dr["inactivitydays"]));

            return new InActivityManagers
            {
                companyId = companyId,
                empId = Convert.ToInt32(dr["empid"]),
                firstName = Convert.ToString(dr["firstname"]),
                lastName = Convert.ToString(dr["lastname"]),
                email = Convert.ToString(dr["email"]),
                smsText = string.Format(smsBody, Convert.ToString(dr["firstname"]), Convert.ToInt32(dr["inactivitydays"])),
                emailText = emailText,
                phoneNumber = Convert.ToString(dr["phonenumber"]),
                inActivityDays = Convert.ToInt32(dr["inactivitydays"]),
                isMailSent = Convert.ToBoolean(dr["ismailsent"]),
                phoneConfirmed = Convert.ToBoolean(dr["phoneconfirmed"]),
                optForSms = Convert.ToBoolean(dr["optforsms"]),
                createdBy = createdById
            };
        }

        /// <summary>
        /// Method to send mail to Trigger Admin regarding spark via sms by unauthorized user
        /// </summary>
        /// <param name="templateName"></param>
        public void SendUnAuthorizedSparkMailsToTriggerAdmin(string phoneNumber,string templateName)
        {
            try
            {
                string templateBody = GetTemplateByName(templateName,1);
                templateBody = templateBody.Replace(Messages.PhoneNumber,phoneNumber);

                BackgroundJob.Enqueue(() => SendMail(_triggerSupportMailId, templateBody, Messages.UnauthorizedSparkEmail));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

    }
}
