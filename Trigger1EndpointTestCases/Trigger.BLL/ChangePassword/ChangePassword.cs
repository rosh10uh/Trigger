using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DTO;
using Trigger.Utility;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Microsoft.AspNetCore.Identity;
using IdentityModel.Client;
using Trigger.DAL.Shared;
using System.Net.Http;
using Trigger.DAL.Shared.Interfaces;

namespace Trigger.BLL.ChangePassword
{
    public class ChangePassword
    {
        private readonly ILogger<ChangePassword> _logger;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IConnectionContext _connectionContext;
        private readonly IClaims _iClaims;
        private readonly string _authURL;

        public ChangePassword(TriggerCatalogContext catalogDbContext, IConnectionContext connectionContext, ILogger<ChangePassword> logger, IClaims Claims)
        {
            _connectionContext = connectionContext;
            _catalogDbContext = catalogDbContext;
            _logger = logger;
            _iClaims = Claims;
            _authURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.AuthUrl.ToString()];
        }


        public virtual JsonData InvokeChangePassword(UserChangePassword userChangePassword)
        {
            try
            {
                AuthUserDetails autData = new AuthUserDetails { UserName = _iClaims["email"].Value };
                AuthUserDetails authPasswordHash = _catalogDbContext.AuthUserDetailsRepository.GetPasswordHashByUserId(autData);

                UserDataModel userDataModel = new UserDataModel() { userName = _iClaims["email"].Value };
                var userData = _connectionContext.TriggerContext.LoginRepository.InvokeLogin(userDataModel);

                Trigger.DTO.UserChangePassword authChangePassword = new Trigger.DTO.UserChangePassword();

                PasswordHasher<Trigger.DTO.UserChangePassword> password = new PasswordHasher<Trigger.DTO.UserChangePassword>();
                if (password.VerifyHashedPassword(authChangePassword, authPasswordHash.PasswordHash, userChangePassword.oldPassword) != PasswordVerificationResult.Failed)
                {
                    string newPasswordHash = password.HashPassword(authChangePassword, userChangePassword.newPassword);
                    authChangePassword.newPassword = newPasswordHash;
                    authChangePassword.userName = _iClaims["email"].Value;

                    if (userData.Message == "" || userData.Message == null)
                    {
                        var authEmp = _catalogDbContext.ChangePasswordRepository.invokeChangeAuthPassword(authChangePassword);
                        if (authEmp.result == 1)
                        {
                            if (userData.roleId != Enums.DimensionElements.TriggerAdmin.GetHashCode())
                            {
                                Task.Run(async () => await InvokeRemoveGrantsAsync(_iClaims.UserId));
                                _connectionContext.TriggerContext.UserLoginModelRepository.invokeDeleteDeviceByLoginUserId(new UserLoginModel { userId = userData.userId });
                            }
                            return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.changePassword);
                        }
                        else if (authEmp.result == 0)
                        {
                            return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_203), Messages.invalidPassword);
                        }
                        else
                        {
                            return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                        }
                    }
                    else
                    {
                        return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_203), userData.Message);
                    }
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_203), Messages.invalidPassword);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }



        /// <summary>
        /// Method to remove grant when password is reset or changed
        /// </summary>
        /// <param name="subId">Auth UserId of user</param>
        /// <returns>Status Code of message</returns>
        public virtual async Task<bool> InvokeRemoveGrantsAsync(string subId)
        {
            var disco = await DiscoveryClient.GetAsync(_authURL);
            var tokenClient = new TokenClient(disco.TokenEndpoint, Messages.triggerClient, Messages.triggerClientSecret);
            await tokenClient.RequestClientCredentialsAsync(Messages.triggerClientAPI);

            string URL = _authURL + Messages.apiManager + subId;
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(URL)
            };

            client.DefaultRequestHeaders.TryAddWithoutValidation(Messages.contantType, Messages.jsonFilePath);
            HttpResponseMessage messge = client.GetAsync(URL).Result;
            return messge.IsSuccessStatusCode;
        }
    }
}
