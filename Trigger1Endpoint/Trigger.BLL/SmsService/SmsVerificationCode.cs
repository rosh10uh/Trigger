using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.SmsService;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.SmsService
{
    /// <summary>
    /// Class Name      :   SmsVerificationCode
    /// Author          :   Vivek Bhavsar
    /// Creation Date   :   24 Apr 2019
    /// Purpose         :   Class to handle business Logic for SmsServiceController(API)
    /// Revision        :   
    /// </summary>
    public class SmsVerificationCode
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<SmsVerificationCode> _logger;
        private readonly SmsServiceContext _smsServiceContext;
        private readonly SmsSettings _smsSettings;

        /// <summary>
        /// Name            :   SmsVerificationCode
        /// Author          :   Vivek Bhavsar
        /// Creation Date   :   24 Apr 2019
        /// Purpose         :   constructor for SmsVerificationCode
        /// Revision        :   
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="smsServiceContext"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="logger"></param>
        public SmsVerificationCode(IOptions<SmsSettings> smsSettings, IConnectionContext connectionContext, SmsServiceContext smsServiceContext, TriggerCatalogContext catalogDbContext, ILogger<SmsVerificationCode> logger)
        {
            _connectionContext = connectionContext;
            _smsServiceContext = smsServiceContext;
            _smsSettings = smsSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Method Name     :   SendSMSVerificationCode
        /// Author          :   Vivek Bhavsar
        /// Creation Date   :   24 Apr 2019
        /// Purpose         :   Generate & Save Verification Code, Send to user
        /// Revision        :   
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        /// <returns></returns>
        public async Task<JsonData> SendSMSVerificationCode(DTO.SmsService.SmsVerificationCode smsVerificationCode)
        {
            try
            {
                smsVerificationCode.verificationCodeTimeOut = _smsSettings.VerificationCodeTimeOut;
                smsVerificationCode.phoneNumber = smsVerificationCode.phoneNumber.Replace(" ", string.Empty);
                
                var verificationCode = await Task.FromResult(_connectionContext.TriggerContext.SmsServiceRepository.GetVerificationCodeForUser(_smsSettings));
                smsVerificationCode.verificationCode = verificationCode;
                smsVerificationCode.smsCodeMessage = string.Format(_smsSettings.VerificationCodeMessage, verificationCode, smsVerificationCode.verificationCodeTimeOut);

                var result = await Task.FromResult(_connectionContext.TriggerContext.SmsServiceRepository.Insert(smsVerificationCode));
                if (result.result > 0)
                {
                    _smsServiceContext.SendSMSVerificationCode(smsVerificationCode);
                    return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_200.GetHashCode(), Messages.verificationCodeSent);
                }
                else
                {
                    return GetResponseForSendSMS(result.result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Get Response as per result of verification code sent as sms
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private JsonData GetResponseForSendSMS(int result)
        {
            int statusCode = Enums.StatusCodes.status_500.GetHashCode();
            string statusMessage = Messages.internalServerError;

            switch (result)
            {
                case -1:
                    statusCode = Enums.StatusCodes.status_403.GetHashCode();
                    statusMessage = Messages.unauthorizedAcces;
                    break;
                case -2:
                    statusCode = Enums.StatusCodes.status_208.GetHashCode();
                    statusMessage = Messages.phoneNumberNotExists;
                    break;
                case -3:
                    statusCode = Enums.StatusCodes.status_304.GetHashCode();
                    statusMessage = Messages.phoneNumberAlreadyVerified;
                    break;
            }

            return JsonSettings.UserDataWithStatusMessage(null, statusCode, statusMessage);
        }

        /// <summary>
        /// Method Name     :   VerifySmsVerificationCode
        /// Author          :   Vivek Bhavsar
        /// Creation Date   :   24 Apr 2019
        /// Purpose         :   Verify verification code for user
        /// Revision        : 
        /// </summary>
        /// <param name="smsVerificationCode"></param>
        /// <returns></returns>
        public async Task<JsonData> VerifySmsVerificationCode(DTO.SmsService.SmsVerificationCode smsVerificationCode)
        {
            try
            {
                DTO.SmsService.SmsCodeVerification smsCodeVerification = new DTO.SmsService.SmsCodeVerification { smsVerified = false };

                var result = await Task.FromResult(_connectionContext.TriggerContext.SmsServiceRepository.Select(smsVerificationCode));
                return GetResponseSMSVerification(result.result, smsCodeVerification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Enums.StatusCodes.status_500.GetHashCode(), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Get response for SMS Verification based on result
        /// </summary>
        /// <param name="result"></param>
        /// <param name="smsCodeVerification"></param>
        /// <returns></returns>
        private JsonData GetResponseSMSVerification(int result, DTO.SmsService.SmsCodeVerification smsCodeVerification)
        {
            int statusCode = Enums.StatusCodes.status_500.GetHashCode();
            string statusMessage = Messages.internalServerError;

            switch (result)
            {
                case 0:
                    statusMessage = Messages.invalidVerificationCode;
                    statusCode = Enums.StatusCodes.status_410.GetHashCode();
                    break;
                case 1:
                    smsCodeVerification.smsVerified = true;
                    statusMessage = Messages.smsCodeVerified;
                    statusCode = Enums.StatusCodes.status_200.GetHashCode();
                    break;
                case -3:
                    smsCodeVerification.smsVerified = true;
                    statusCode = Enums.StatusCodes.status_304.GetHashCode();
                    statusMessage = Messages.phoneNumberAlreadyVerified;
                    break;
                case -2:
                    statusMessage = Messages.verificationCodeTimeExpired;
                    statusCode = Enums.StatusCodes.status_410.GetHashCode();
                    break;
            }

            return JsonSettings.UserDataWithStatusMessage(smsCodeVerification, statusCode, statusMessage);
        }
    }
}
