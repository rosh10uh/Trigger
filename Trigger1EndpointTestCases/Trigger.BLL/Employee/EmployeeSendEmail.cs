using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Employee;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    /// <summary>
    /// Class to send registration/update email to employee
    /// </summary>
    public class EmployeeSendEmail
    {
        private readonly BackgroundJobRequest _backGroundJobRequest;
        private readonly EmployeeContext _employeeContext;
        private readonly AppSettings _appSettings;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly ILogger<EmployeeSendEmail> _logger;
        private readonly EmployeeCommon _employeeCommon;

        /// <summary>
        /// Constructor of EmployeeSendEmail class
        /// </summary>
        /// <param name="employeeContext"></param>
        /// <param name="backGroundJobRequest"></param>
        /// <param name="appSettings"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="logger"></param>
        /// <param name="employeeCommon"></param>
        public EmployeeSendEmail(EmployeeContext employeeContext, BackgroundJobRequest backGroundJobRequest,
            IOptions<AppSettings> appSettings, TriggerCatalogContext catalogDbContext,
            ILogger<EmployeeSendEmail> logger, EmployeeCommon employeeCommon)
        {
            _backGroundJobRequest = backGroundJobRequest;
            _employeeContext = employeeContext;
            _appSettings = appSettings.Value;
            _catalogDbContext = catalogDbContext;
            _logger = logger;
            _employeeCommon = employeeCommon;
        }

        /// <summary>
        /// Send Mail to registration mail to selected users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employee"></param>
        /// /// <returns></returns>
        public virtual async Task<CustomJsonData> SendMailAndUpdateFlag(int userId, EmployeeModel employee)
        {
            try
            {
                string responseMessage = Messages.noRecords;
                Enums.StatusCodes responseCode = Enums.StatusCodes.status_204;
                List<EmployeeModel> employees = _employeeCommon.GetEmployeesDetail(employee.CompanyId, employee.EmpIdList);
                if (employees.Count > 0)
                {
                    employees.ForEach(s => s.UpdatedBy = userId);
                    await Task.Run(() => SendRegistrationEmail(employees));
                    responseMessage = Messages.mailSent;
                    responseCode = Enums.StatusCodes.status_200;
                }
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(responseCode), responseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method to send registration mail to employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="authUserId"></param>
        /// <param name="securityStamp"></param>
        public virtual void SendEmployeeRegistrationMail(EmployeeModel employee, string authUserId, string securityStamp)
        {
            if (employee.EmpStatus)
            {
                var companyDetailsModels = _catalogDbContext.CompanyRepository.Select<List<CompanyDetailsModel>>(new CompanyDetailsModel { compId = employee.CompanyId });
                if (companyDetailsModels != null && companyDetailsModels.Count > 0 && companyDetailsModels[0].contractStartDate.Date > DateTime.Now.Date)
                {
                    _backGroundJobRequest.SendEmail(employee, PasswordGenerateUrl(authUserId, securityStamp));
                    employee.UpdatedBy = employee.CreatedBy;
                }
            }
        }

        /// <summary>
        /// Method to get auth details 
        /// </summary>
        /// <param name="employeeModels"></param>
        /// <returns></returns>
        private List<AuthUserDetails> GetAuthUserDetails(List<EmployeeModel> employeeModels)
        {
            List<AuthUserDetails> authUserDetails = new List<AuthUserDetails>();
            foreach (var authDetails in employeeModels)
            {
                AuthUserDetails authUsersMail = new AuthUserDetails() { Email = authDetails.Email };
                authUsersMail = _catalogDbContext.AuthUserDetailsRepository.GetAuthDetailsByEmail(authUsersMail);
                authUserDetails.Add(authUsersMail);
            }
            return authUserDetails;
        }

        /// <summary>
        /// Method to send employee update email
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="authUserId"></param>
        /// <param name="securityStamp"></param>
        public virtual void SendEmailOnUpdateEmployeeDetails(EmployeeModel employee, string authUserId, string securityStamp)
        {
            try
            {
                var companyDetailsModel = _catalogDbContext.CompanyRepository.Select<CompanyDetailsModel>(new CompanyDetailsModel { compId = employee.CompanyId });
                if (companyDetailsModel.contractStartDate.Date > DateTime.Now.Date && employee.EmpStatus)
                {
                    _backGroundJobRequest.SendEmail(employee, PasswordGenerateUrl(authUserId, securityStamp));
                    employee.UpdatedBy = employee.CreatedBy;
                }
                else
                {
                    if (companyDetailsModel.contractStartDate.Date <= DateTime.Now.Date && companyDetailsModel.contractEndDate.Date.AddDays(companyDetailsModel.gracePeriod) >= DateTime.Now.Date && employee.IsMailSent && employee.EmpStatus)
                    {
                        SendEmail(employee, Messages.userUpdateEmailTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Method to send registration mail
        /// </summary>
        /// <param name="employeeModels"></param>
        private void SendRegistrationEmail(List<EmployeeModel> employeeModels)
        {
            try
            {
                List<AuthUserDetails> authUserDetails = GetAuthUserDetails(employeeModels);
                var sendMailUsers = employeeModels.Join(authUserDetails, a => a.Email, b => b.Email, (a, b) => new { a.EmpId, a.CompanyId, a.UpdatedBy, b.Email, b.Id, b.SecurityStamp });
                foreach (var user in sendMailUsers)
                {
                    EmployeeModel employeeModel = new EmployeeModel { Email = user.Email.ToString(), EmpId = user.EmpId, CompanyId = user.CompanyId, UpdatedBy = user.UpdatedBy };
                    SendEmail(employeeModel, Messages.userRegEmailTemplate, PasswordGenerateUrl(user.Id, user.SecurityStamp));
                    _employeeContext.UpdateEmpIsMailSent(employeeModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Method to send email
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="templateName"></param>
        /// <param name="url"></param>
        private void SendEmail(EmployeeModel employeeModel, string templateName, string url = "")
        {
            string body = _backGroundJobRequest.GetTemplateByName(templateName, employeeModel.CompanyId);

            if (!string.IsNullOrEmpty(employeeModel.FirstName))
                body = body.Replace(Messages.headingtext, employeeModel.FirstName);
            if (!string.IsNullOrEmpty(url))
                body = body.Replace(Messages.resetPasswordURL, url);
            body = body.Replace(Messages.userId, employeeModel.Email);
            body = body.Replace(Messages.landingURL, _appSettings.LandingURL);

            EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), employeeModel.Email, Messages.registration, body, string.Empty, true);
        }

        /// <summary>
        /// Method to pasword generate url
        /// </summary>
        /// <param name="authUserId"></param>
        /// <param name="securityStamp"></param>
        /// <returns></returns>
        private string PasswordGenerateUrl(string authUserId, String securityStamp)
        {
            return _appSettings.AuthUrl + Messages.confirmEmailPath + authUserId + Messages.code + _employeeCommon.GenerateToken(securityStamp);
        }
    }
}
