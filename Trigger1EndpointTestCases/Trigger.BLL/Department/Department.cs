using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.Department
{
    public class Department
    {
        private readonly ILogger<Department> _logger;
        private readonly IConnectionContext _connectionContext;

        public Department(IConnectionContext connectionContext, ILogger<Department> logger)
        {
            _connectionContext = connectionContext;
            _logger = logger;
        }

        /// <summary>
        /// Use to get all department list
        /// </summary>
        /// <returns>List of DepartmentModel</returns>
        public virtual async Task<CustomJsonData> GetAllDepartmentAsync()
        {
            try
            {
                var departments = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.SelectAll());
                return JsonSettings.UserCustomDataWithStatusMessage(departments, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Use to get company & yearwise all department list
        /// </summary>
        /// <returns>List of DepartmentModel</returns>
        public virtual async Task<CustomJsonData> GetCompanyAndYearwiseDepartments(int companyId,string yearId)
        {
            try
            {
                var departmentModel = new DepartmentModel { companyId = companyId, yearId = yearId };
                var departments = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.GetCompanyAndYearwiseDepartment(departmentModel));
                return JsonSettings.UserCustomDataWithStatusMessage(departments, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Use to get companywise department list
        /// </summary>
        /// <returns>List of DepartmentModel</returns>
        public virtual async Task<CustomJsonData> GetCompanywiseDepartments(int companyId)
        {
            try
            {
                var departmentModel = new DepartmentModel { companyId = companyId};
                var departments = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.GetCompanywiseDepartment(departmentModel));
                return JsonSettings.UserCustomDataWithStatusMessage(departments, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Use to add new department
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> AddDepartmentAsync(DepartmentModel departmentModel)
        {
            try
            {
                var department = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.Insert(departmentModel));

                if (department.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.addDepartment);
                }
                else if (department.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.departmentIsExists);
                }

                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }

        }

        /// <summary>
        /// Use to update department
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> UpdateDepartmentAsync(DepartmentModel departmentModel)
        {
            try
            {
                var department = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.Update(departmentModel));

                if (department.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.updateDepartment);
                }
                else if (department.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.departmentIsExists);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Use to delete department
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        public virtual async Task<JsonData> DeleteDepartmentAsync(int companyId, int departmentId, string updatedBy)
        {
            try
            {
                var departmentModel = new DepartmentModel { companyId = companyId, departmentId = departmentId, updatedBy = updatedBy };
                var department = await Task.FromResult(_connectionContext.TriggerContext.DepartmentRepository.Delete(departmentModel));

                if (department.result == 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.cannotDeleteDepartment);
                }
                else if (department.result > 0)
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.deleteDepartment);
                }
                else
                {
                    return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
