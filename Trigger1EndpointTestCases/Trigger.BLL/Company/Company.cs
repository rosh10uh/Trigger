using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Company;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Company
{
    /// <summary>
    /// Contains method to manage company.
    /// </summary>
    public class Company
    {
        /// <summary>
        /// Use to get service of OneAuthorityPolicyContext
        /// </summary>
        private readonly TriggerCatalogContext _catalogDbContext;

        /// <summary>
        /// Use to get service of DAL.CompanyContext
        /// </summary>        
        private readonly CompanyContext _companyContext;

        private readonly string _blobContainer = Messages.companyLogo;
        private readonly ILogger<Company> _iLogger;
        private readonly BackgroundJobRequest _backGroundJobRequest;
<<<<<<< HEAD
        private readonly TeamBackgroundJobRequest _teamBackgroundJobRequest;

        public Company(TriggerCatalogContext catalogDbContext, CompanyContext companyContext, BackgroundJobRequest backGroundJobRequest,
            IHttpContextAccessor contextAccessor, ILogger<Company> iLogger, TeamBackgroundJobRequest teamBackgroundJobRequest)
=======
        private readonly AppSettings _appSettings;

        public Company(TriggerCatalogContext catalogDbContext, CompanyContext companyContext, 
            BackgroundJobRequest backGroundJobRequest, IHttpContextAccessor contextAccessor, ILogger<Company> iLogger,
            IOptions<AppSettings> appSettings)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            _catalogDbContext = catalogDbContext;
            _companyContext = companyContext;
            _backGroundJobRequest = backGroundJobRequest;
<<<<<<< HEAD
            _teamBackgroundJobRequest = teamBackgroundJobRequest;
=======
            _appSettings = appSettings.Value;
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
            _iLogger = iLogger;
        }

        /// <summary>
        ///  This async method is responsible to get list of all companies
        /// </summary>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> SelectAllAsync()
        {
            try
            {
                var company = await Task.FromResult(_catalogDbContext.CompanyRepository.SelectAll());
                company.ForEach(s => s.compImgPath = _appSettings.StorageAccountURL+ Messages.slash + _blobContainer + Messages.slash + s.compImgPath);

                return JsonSettings.UserCustomDataWithStatusMessage(company,
                    Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        ///  This async method is responsible to get list of all company by company Id
        /// </summary>
        /// <param name="companyDetailsModel"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> SelectAsync(CompanyDetailsModel companyDetailsModel)
        {
            try
            {
                var company = await Task.FromResult(_catalogDbContext.CompanyRepository.Select<List<CompanyDetailsModel>>(companyDetailsModel));
                company.ForEach(s => s.compImgPath = _appSettings.StorageAccountURL+ Messages.slash + _blobContainer + Messages.slash + s.compImgPath);
                return JsonSettings.UserCustomDataWithStatusMessage(company,
                    Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// This async method is responsible to insert company
        /// </summary>
        /// <param name="companyDetailsModel"></param>
        /// <returns>CompanyDetailsModel</returns>
        public virtual async Task<JsonData> InsertAsync(CompanyDetailsModel companyDetailsModel)
        {
            try
            {
                _catalogDbContext.BeginTransaction();
                var companyDetail = await Task.FromResult(_companyContext.InsertCompany(companyDetailsModel));
                if (companyDetail.result > 0)
                {
                    _catalogDbContext.Commit();
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.companySubmitted);
                }
                else if (companyDetail.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.companyExists);
                }
                else if (companyDetail.result == -2)
                {
                    _catalogDbContext.RollBack();
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.failToCreateNewTenant);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedForAddCompany);
                }
            }
            catch (Exception ex)
            {
                _catalogDbContext.RollBack();
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// This async method is responsible to update company
        /// </summary>
        /// <param name="companyDetailsModel"></param>
        /// <returns>CompanyDetailsModel</returns>
        public virtual async Task<JsonData> UpdateAsync(CompanyDetailsModel companyDetailsModel)
        {
            try
            {
                companyDetailsModel.compImgPath = companyDetailsModel.compImgPath == string.Empty ? string.Empty : Guid.NewGuid().ToString() + companyDetailsModel.compImgPath;
                var result = _catalogDbContext.CompanyRepository.Update(companyDetailsModel).result;

                if (result != 1) return await Task.FromResult(GetUpdateResponse(result, null));

                UploadCompanyLogo(companyDetailsModel);

                var compLogo = new CompLogoModel()
                {
                    compImage = string.Empty,
                    compImgPath = Convert.ToString(companyDetailsModel.compImgPath == string.Empty ? string.Empty : _appSettings.StorageAccountURL+ Messages.slash + _blobContainer + Messages.slash + companyDetailsModel.compImgPath),
                    compFolderPath = string.Empty
                };

                return await Task.FromResult(GetUpdateResponse(result, compLogo));
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// This method is used for set response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="compLogoModel"></param>
        /// <returns></returns>
        private JsonData GetUpdateResponse(int result, CompLogoModel compLogoModel)
        {
            switch (result)
            {
                case 0:
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.companyExists);
                case 1:
                    return JsonSettings.UserDataWithStatusMessage(compLogoModel, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.updatedCompany);
                case 3:
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.companyIdIsExists);
                default:
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedForEditCompany);
            }
        }

        /// <summary>
        /// Update Company/Client logo
        /// </summary>
        /// <param name="companyDetailsModel">company model</param>
        private void UploadCompanyLogo(CompanyDetailsModel companyDetailsModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyDetailsModel.compImage))
                {
                    FileActions.UploadtoBlobAsync(companyDetailsModel.compImgPath, companyDetailsModel.compImage, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, _blobContainer).Wait();
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// This async method is responsible to delete company
        /// </summary>
        /// <param name="companyDetailsModel"></param>
        /// <returns>CompanyDetailsModel</returns>
        public virtual async Task<JsonData> DeleteAsync(CompanyDetailsModel companyDetailsModel)
        {
            try
            {
                var result = await Task.FromResult(_catalogDbContext.CompanyRepository.Delete(companyDetailsModel).result);

                if (result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.deleteCompany);
                }
                else if (result == 0)
                {
                    return (JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.inactiveCompany));
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedForDeleteCompany);
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// AddInactivityScheduler To add scheduler for existing companies
        /// </summary>
        /// <returns></returns>
        public virtual async Task<JsonData> AddInactivityScheduler()
        {
            try
            {
                _backGroundJobRequest.ScheduleInActivityReminder();
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
            }

            return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), "Inactivity Scheduler Added Successfully"));
        }

        /// <summary>
        /// schedule job for send Team notification
        /// </summary>
        /// <returns></returns>
        public virtual async Task<JsonData> AddTeamNotiifcationScheduler()
        {
            try
            {
                _teamBackgroundJobRequest.ScheduleTeamNotiifcationJob();
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
            }

            return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), "Team Notiifcation scheduler added successfully"));
        }

    }
}
