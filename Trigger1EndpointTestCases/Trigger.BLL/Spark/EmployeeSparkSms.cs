using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Spark;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.DTO.EmailTemplate;
using Trigger.DTO.SmsService;
using Trigger.DTO.Spark;
using Trigger.Utility;
using Twilio.Rest.Api.V2010.Account;

namespace Trigger.BLL.Spark
{
    /// <summary>
    /// Class Name   :   SparkSms
    /// Author       :   Vivek Bhavsar
    /// Creation Date:   28 Aug 2019
    /// Purpose      :   Business logic to ready spark from sms and convert to standard format to store spark for employee 
    /// Revision     : 
    /// </summary>
    public class EmployeeSparkSms
    {
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<EmployeeSparkSms> _logger;
        private readonly EmployeeSparkContext _employeeSparkContext;
        private readonly IActionPermission _permission;
        private readonly BackgroundJobRequest _backgroundJobRequest;
        private readonly AppSettings _appSettings;
        private readonly TwilioSettings _twilioSettings;

        /// <summary>
        /// Constructor to initialized connection & logger objects
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="logger"></param>
        public EmployeeSparkSms(IConnectionContext connectionContext, TriggerCatalogContext catalogDbContext,
            EmployeeSparkContext employeeSparkContext, IActionPermission permission, IOptions<AppSettings> appSettings, IOptions<TwilioSettings> twilioSettings,
            BackgroundJobRequest backgroundJobRequest, ILogger<EmployeeSparkSms> logger)
        {
            _connectionContext = connectionContext;
            _catalogDbContext = catalogDbContext;
            _employeeSparkContext = employeeSparkContext;
            _backgroundJobRequest = backgroundJobRequest;
            _permission = permission;
            _logger = logger;
            _appSettings = appSettings.Value;
            _twilioSettings = twilioSettings.Value;
        }

        /// <summary>
        /// Method to receive sms details for Spark an employee
        /// </summary>
        /// <param name="smsSender"></param>
        /// <param name="smsBody"></param>
        /// <returns></returns>
        public async Task<string> SparkBySms(string smsSender, string smsBody,string messageSId)
        {
            string responseMessage;
            try
            {
                if (string.IsNullOrEmpty(smsBody?.Trim()))
                {
                    return Messages.InvalidSMSFormat;
                }
                
                string[] smsBodySplit = smsBody.Split(Messages.SmsBodySplitter);
                if (smsBodySplit.Length > 1 && smsBodySplit.Length < 3 && (smsBodySplit.Length > 1 && !string.IsNullOrEmpty(smsBodySplit[0].Trim())) && (smsBodySplit.Length > 1 && !string.IsNullOrEmpty(smsBodySplit[1].Trim())))
                {
                    UserDataModel userDataModel = GetUserDataByPhoneNumber(smsSender);
                    if (!string.IsNullOrEmpty(userDataModel?.Error))
                    {
                        return userDataModel?.Error;
                    }

                    if (_permission.CheckActionPermission(GetActionPermissionParameters(Convert.ToInt32(userDataModel?.roleId), Convert.ToInt32(userDataModel?.empId))))
                    {
                        var employeeModel = _connectionContext.TriggerContext.EmployeeSparkRepository.GetEmployeeDetailsByEmployeeId(new EmployeeSparkModel { EmployeeId = smsBodySplit[0]?.ToString().Trim() });

                        if (employeeModel?.EmpId > 0)
                        {
                            if (!employeeModel.EmpId.Equals(userDataModel?.empId))
                            {
                                EmployeeSparkModel employeeSparkModel = GetModelForSpark(userDataModel, smsBodySplit, smsSender, employeeModel, messageSId);
                                responseMessage = await AddEmployeeSpark(employeeSparkModel, employeeModel, userDataModel);
                            }
                            else
                            {
                                responseMessage = Messages.SparkForSelf;
                            }
                        }
                        else
                        {
                            responseMessage = Messages.employeeNotExist;
                        }
                    }
                    else
                    {
                        responseMessage = Messages.UnauthorizedForSparkSms;
                    }
                }
                else
                {
                    return Messages.InvalidSMSFormat;
                }

                return responseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return Messages.somethingWentWrong;
            }

        }

        /// <summary>
        /// Check user login validations & phone number confirmed?
        /// </summary>
        /// <param name="userDataModel"></param>
        private string CheckValidUser(int empId)
        {
            var employeeProfile = _connectionContext.TriggerContext.EmployeeProfileRepository.Select(new EmployeeProfileModel() { EmpId = empId });
            if (!employeeProfile.PhoneConfirmed)
            {
                return Messages.phoneNumberIsNotVerified;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get spark details and return ready model for spark
        /// </summary>
        /// <param name="userDataModel"></param>
        /// <param name="smsBody"></param>
        /// <param name="SenderPhoneNumber"></param>
        /// <returns></returns>
        private EmployeeSparkModel GetModelForSpark(UserDataModel userDataModel, string[] sparkDetail, string SenderPhoneNumber, EmployeeModel employeeModel,string messageSId)
        {
            int approvalStatus = Enums.SparkStatus.UnApproved.GetHashCode();
            if (userDataModel.empId == employeeModel.ManagerId)
            {
                approvalStatus = Enums.SparkStatus.Approved.GetHashCode();
            }

            return new EmployeeSparkModel()
            {
                EmpId = employeeModel.EmpId,
                CreatedBy = userDataModel.userId,
                SparkBy = userDataModel.userId,
                SparkDate = GetMessageReceivedDateTime(messageSId),
                ApprovalStatus = approvalStatus,
                ApprovalBy = userDataModel.userId,
                Remarks = sparkDetail[1]?.ToString().Trim(),
                CategoryId = Enums.Category.General.GetHashCode(),
                ClassificationId = Enums.Classifications.General.GetHashCode(),
                SenderPhoneNumber = SenderPhoneNumber,
                ViaSms = true,
                Result = 1
            };
        }

        /// <summary>
        /// Method to fetch message received date time by message id
        /// </summary>
        /// <param name="messageSId"></param>
        /// <returns></returns>
        private DateTime GetMessageReceivedDateTime(string messageSId)
        {
            if (!string.IsNullOrEmpty(messageSId))
            {
                Twilio.TwilioClient.Init(_twilioSettings.Sid, _twilioSettings.Token);
                MessageResource messageResource = MessageResource.Fetch(messageSId);
                return Convert.ToDateTime(messageResource.DateCreated);
            }
            else
            {
                return Convert.ToDateTime(DateTime.UtcNow.ToString(Messages.DateTimeFormat),DateTimeFormatInfo.InvariantInfo);
            }
        }

        /// <summary>
        /// Send spark approval email to direct reporting manager
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="senderEmpID"></param>
        /// <param name="managerId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private void SendSparkApprovalEmail(int empId, int senderEmpID, int managerId, int companyId)
        {
            string empIdList = string.Join(",", new int[] { empId, senderEmpID, managerId });
            List<EmployeeModel> employeeModels = _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeDetailsByEmpIds(new EmployeeModel { EmpIdList = empIdList });

            EmailTemplateDetails emailTemplateDetails = new EmailTemplateDetails { templateName = Messages.SparkApprovalEmailTemplate, companyId = companyId };
            string templateBody = _catalogDbContext.EmailTemplateRepository.GetEmailTemplateByName(emailTemplateDetails);

            var empDetails = employeeModels.FirstOrDefault(x => x.EmpId == empId);
            templateBody = templateBody.Replace(Messages.EmployeeName, string.Concat(empDetails.FirstName, (char)32, empDetails.LastName));

            empDetails = employeeModels.FirstOrDefault(x => x.EmpId == senderEmpID);
            templateBody = templateBody.Replace(Messages.SparkBy, string.Concat(empDetails.FirstName, (char)32, empDetails.LastName));

            var managerDetails = employeeModels.FirstOrDefault(x => x.EmpId == managerId);
            templateBody = templateBody.Replace(Messages.ManagerName, string.Concat(managerDetails.FirstName, (char)32, managerDetails.LastName));

            templateBody = templateBody.Replace(Messages.CurrentYear, DateTime.Now.Year.ToString());


            EmailConfiguration.SendMail(_appSettings.SenderEmailID, _appSettings.SenderPassword, _appSettings.SmtpServer, Convert.ToInt32(_appSettings.SmtpPort), managerDetails.Email, Messages.SparkApprovalEmailSubject, templateBody, string.Empty, true);
        }

        /// <summary>
        /// method to get user details using phone number
        /// </summary>
        /// <param name="smsSender"></param>
        /// <returns></returns>
        private UserDataModel GetUserDataByPhoneNumber(string smsSender)
        {
            smsSender = smsSender.Insert(smsSender.Length - 10, " ");
            UserDataModel userDataModel = new UserDataModel();
            List<AspnetUserDetails> aspnetUserDetails = _catalogDbContext.EmployeeSparkRepository.GetAspnetUserByPhoneNumber(new EmployeeSparkModel { PhoneNumber = smsSender });

            if (aspnetUserDetails?.Count > 0)
            {
                userDataModel = _connectionContext.TriggerContext.LoginRepository.InvokeLogin(new UserDataModel { userName = aspnetUserDetails.FirstOrDefault().Email });
                if (string.IsNullOrEmpty(userDataModel.Error))
                {
                    userDataModel.Error = CheckValidUser(Convert.ToInt32(userDataModel?.empId));
                }
                userDataModel.companyid = Convert.ToInt32(aspnetUserDetails?.FirstOrDefault(x => x.ClaimType == Enums.ClaimType.CompId.ToString()).ClaimValue);
            }
            else
            {
                _backgroundJobRequest.SendUnAuthorizedSparkMailsToTriggerAdmin(smsSender, Messages.SparkByUnknownUser);
                userDataModel.Error = Messages.UnknownUser;
            }

            return userDataModel;
        }

        /// <summary>
        /// method to get checkpermission parameter dto
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="empId"></param>
        /// <returns></returns>
        private CheckPermission GetActionPermissionParameters(int roleId, int empId)
        {
            return new CheckPermission
            {
                RoleId = roleId,
                EmpId = empId,
                ActionId = Enums.Actions.SparkEmployee.GetHashCode(),
                PermissionType = Enums.PermissionType.CanAdd.ToString()
            };
        }

        /// <summary>
        /// Method to call AddEmployeeSpark for spark an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        private async Task<string> AddEmployeeSpark(EmployeeSparkModel employeeSparkModel, EmployeeModel employeeModel, UserDataModel userDataModel)
        {
            string responseMessage = string.Empty;
            if (employeeSparkModel?.Result > 0)
            {
                var result = await _employeeSparkContext.AddEmployeeSpark(employeeSparkModel);

                switch (result.Result)
                {
                    case 0:
                        responseMessage = Messages.InvalidSparkDetails;
                        break;
                    case 1:
                        if (employeeSparkModel.ApprovalStatus == Enums.SparkStatus.UnApproved.GetHashCode())
                        {
                            SendSparkApprovalEmail(employeeModel.EmpId, userDataModel.empId, employeeModel.ManagerId, userDataModel.companyid);
                        }
                        responseMessage = Messages.AddEmployeeSpark;
                        break;
                    case -1:
                        responseMessage = Messages.UnauthorizedForSpark;
                        break;
                    default:
                        responseMessage = Messages.internalServerError;
                        break;
                }
            }
            else
            {
                responseMessage = Messages.employeeNotExist;
            }

            return responseMessage;
        }

    }
}
