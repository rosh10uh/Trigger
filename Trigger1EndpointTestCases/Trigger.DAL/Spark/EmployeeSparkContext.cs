using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Utility;

namespace Trigger.DAL.Spark
{
    public class EmployeeSparkContext
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<EmployeeSparkContext> _logger;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Constructor for connection & logger initialization
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="appSettings"></param>
        /// <param name="logger"></param>
        public EmployeeSparkContext(IConnectionContext connectionContext, IOptions<AppSettings> appSettings, ILogger<EmployeeSparkContext> logger)
        {
            _connectionContext = connectionContext;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Method to add spark for employee including document attachment
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public virtual async Task<EmployeeSparkModel> AddEmployeeSpark(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                if (employeeSparkModel.DocumentName.Length > 0 && employeeSparkModel.DocumentContents.Length > 0)
                {
                    employeeSparkModel.DocumentName = FileActions.GenerateUniqueDocumentName(employeeSparkModel.DocumentName);
                }

                var result = _connectionContext.TriggerContext.EmployeeSparkRepository.InsertEmployeeSparkDetails(employeeSparkModel);

                if (result.Result > 0 && employeeSparkModel.DocumentName.Length > 0 && employeeSparkModel.DocumentContents.Length > 0)
                {
                   await FileActions.UploadtoBlobAsync(employeeSparkModel.DocumentName, employeeSparkModel.DocumentContents, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, Messages.AssessmentDocPath);
                }

                _connectionContext.TriggerContext.Commit();

                employeeSparkModel.Result = result.Result;
                employeeSparkModel.SparkId = result.Result > 0 ? result.Result : 0;

                return await Task.FromResult(GetSparkResponse(employeeSparkModel));
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return employeeSparkModel;
            }
        }

        /// <summary>
        /// Method to update spark details including document attachment
        /// </summary>
        /// <param name="employeeSparkModel"></param>
<<<<<<< HEAD
        /// <returns>Task<EmployeeSparkModel></returns>
        public async Task<EmployeeSparkModel> UpdateEmployeeSpark(EmployeeSparkModel employeeSparkModel)
=======
        /// <returns></returns>
        public virtual async Task<EmployeeSparkModel> UpdateEmployeeSpark(EmployeeSparkModel employeeSparkModel)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            int result=0;
            try
            {
                _connectionContext.TriggerContext.BeginTransaction();

                if (employeeSparkModel.DocumentName.Length == 0 && employeeSparkModel.DocumentContents.Length == 0)
                {
                    await DeleteAttachmentAsync(employeeSparkModel);
                }
                else if (employeeSparkModel.DocumentName.Length > 0 && employeeSparkModel.DocumentContents.Length > 0)
                {
                    await DeleteAttachmentAsync(employeeSparkModel, false);
                    employeeSparkModel.DocumentName = FileActions.GenerateUniqueDocumentName(employeeSparkModel.DocumentName);
                }

                if (employeeSparkModel.DocumentName.Length == 0 && employeeSparkModel.DocumentContents.Length > 0)
                {
                    result = 0;
                }
                else
                {
                    result = await UpdateEmployeeSparkDetail(employeeSparkModel);

                    _connectionContext.TriggerContext.Commit();
                }
                employeeSparkModel.Result = result;
                return GetSparkResponse(employeeSparkModel);
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.RollBack();
                _logger.LogError(ex.Message.ToString());
                return employeeSparkModel;
            }
        }

        /// <summary>
        /// Method to update spark details including document attachment
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns>Task<int></returns>
        private async Task<int> UpdateEmployeeSparkDetail(EmployeeSparkModel employeeSparkModel)
        {
            int result = _connectionContext.TriggerContext.EmployeeSparkRepository.UpdateEmployeeSparkDetails(employeeSparkModel).Result;
            if (result > 0 && employeeSparkModel.DocumentName.Length > 0 && employeeSparkModel.DocumentContents.Length > 0)
            {
                await FileActions.UploadtoBlobAsync(employeeSparkModel.DocumentName, employeeSparkModel.DocumentContents, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, Messages.AssessmentDocPath);
            }

            return result;
        }

        /// <summary>
        /// Method to delete spark details for an employee including attachment delete
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync(EmployeeSparkModel employeeSparkModel)
        {
            try
            {
                var documentName = _connectionContext.TriggerContext.EmployeeSparkRepository.Select(employeeSparkModel);
                var result = _connectionContext.TriggerContext.EmployeeSparkRepository.DeleteEmployeeSparkDetails(employeeSparkModel);

                if (result.Result > 0 && documentName.DocumentName.Length > 0)
                {
                    await FileActions.DeleteFileFromBlobStorage(documentName.DocumentName, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, Messages.AssessmentDocPath);
                }

                return result.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return 0;
            }
        }

        /// <summary>
        /// Method to delete spark attachment of an employee
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        public virtual async Task<EmployeeSparkModel> DeleteAttachmentAsync(EmployeeSparkModel employeeSparkModel, bool deleteFromDatabase = true)
        {
            int result = 0;
            try
            {
                var documentName = _connectionContext.TriggerContext.EmployeeSparkRepository.Select(employeeSparkModel);

                if (documentName.DocumentName.Length > 0)
                {
                    result = deleteFromDatabase ? _connectionContext.TriggerContext.EmployeeSparkRepository.DeleteEmployeeSparkAttachment(employeeSparkModel).Result : 0;
                    if (result > 0 || !deleteFromDatabase)
                    {
                        await FileActions.DeleteFileFromBlobStorage(documentName.DocumentName, _appSettings.StorageAccountName, _appSettings.StorageAccountAccessKey, Messages.AssessmentDocPath);
                    }
                }

                employeeSparkModel.Result = result;
                return GetSparkResponse(employeeSparkModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return employeeSparkModel;
            }
        }

        /// <summary>
        /// Method to get response with document path
        /// </summary>
        /// <param name="employeeSparkModel"></param>
        /// <returns></returns>
        private EmployeeSparkModel GetSparkResponse(EmployeeSparkModel employeeSparkModel)
        {
            if (employeeSparkModel.Result > 0)
            {
                employeeSparkModel = _connectionContext.TriggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(new EmployeeSparkModel { EmpId = employeeSparkModel.EmpId, SparkId = employeeSparkModel.SparkId });
                employeeSparkModel.Result = 1;

                if (employeeSparkModel.DocumentName.Length > 0)
                {
                    employeeSparkModel.DocumentName = string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.AssessmentDocPath, Messages.slash, employeeSparkModel.DocumentName);
                }
                employeeSparkModel.SparkByImgPath = employeeSparkModel.SparkByImgPath?.Length > 0 ? string.Concat(_appSettings.StorageAccountURL, Messages.slash, Messages.profilePic, Messages.slash, employeeSparkModel.SparkByImgPath) : employeeSparkModel.SparkByImgPath;
            }

            return employeeSparkModel;
        }

    }
}
