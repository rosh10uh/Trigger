using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Enum;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    /// <summary>
    /// Class to manage common method of employee module
    /// </summary>
    public class EmployeeCommon
    {
        private readonly string _blobContainer = Messages.profilePic;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<EmployeeCommon> _logger;
        private readonly AppSettings _appSettings;
        private readonly IClaims _iClaims;
        private readonly IActionPermission _actionPermission;

        /// <summary>
        /// Constructor of EmployeeCommon class
        /// </summary>
        /// <param name="catalogDbContext"></param>
        /// <param name="connectionContext"></param>
        /// <param name="employeeContext"></param>
        /// <param name="logger"></param>
        /// <param name="appSettings"></param>
        /// <param name="iClaims"></param>
        /// <param name="actionPermission"></param>
        public EmployeeCommon(TriggerCatalogContext catalogDbContext, IConnectionContext connectionContext,
            EmployeeContext employeeContext, ILogger<EmployeeCommon> logger,
            IOptions<AppSettings> appSettings, IClaims iClaims,
            IActionPermission actionPermission)
        {
            _catalogDbContext = catalogDbContext;
            _connectionContext = connectionContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _iClaims = iClaims;
            _actionPermission = actionPermission;
        }

        /// <summary>
        /// Method to check given phone number exists or not
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="isAddMode"></param>
        /// <returns></returns>
        public virtual Boolean IsPhoneNumberExistAspNetUser(EmployeeModel employee, Boolean isAddMode = false)
        {
            if (string.IsNullOrEmpty(employee.PhoneNumber))
                return false;
            if (isAddMode)
                employee.Email = string.Empty;
            int PhonumberCount = _catalogDbContext.EmployeeRepository.GetAspNetUserCountByPhone(employee);
            return PhonumberCount > 0;
        }

        /// <summary>
        /// Method which get employee details before updation of employee data
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public virtual EmployeeModel GetExistingEmployeeDetails(int empId)
        {
            EmployeeModel employeeModel = new EmployeeModel() { EmpId = empId };
            return _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeById(employeeModel);
        }

        /// <summary>
        /// Method which returns Employee updation message based on result
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        public virtual CustomJsonData GetResponseMessageForUpdate(int resultId)
        {
            switch ((EmployeeEnums.UpdateResultType)resultId)
            {
                case EmployeeEnums.UpdateResultType.EmployeeNotExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeNotExist);
                case EmployeeEnums.UpdateResultType.EmployeeUpdate:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.updateEmployee);
                case EmployeeEnums.UpdateResultType.EmailIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.employeeEmailIdIsExists);
                case EmployeeEnums.UpdateResultType.EmployeeIdExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_209), Messages.employeeIdIsExists);
                case EmployeeEnums.UpdateResultType.PhoneNumberExist:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_208), Messages.phoneNumberIsExists);
                default:
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_401), Messages.unauthorizedToUpdateEmployee);
            }
        }

        /// <summary>
        /// Update Users details in Authority
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="subId"></param>
        public virtual void UpdateAuthUser(EmployeeModel employee, string subId)
        {
            try
            {
                AuthUserDetails authUserDetails = new AuthUserDetails
                {
                    NormalizedEmail = employee.Email.ToUpper(),
                    UserName = employee.Email.ToUpper(),
                    NormalizedUserName = employee.Email.ToUpper(),
                    Email = employee.Email,
                    SubId = subId,
                    EmailConfirmed = employee.EmpStatus,
                    PhoneNumber = employee.PhoneNumber,
                    PhoneNumberConfirmed = false
                };

                _catalogDbContext.AuthUserDetailsRepository.Update(authUserDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method to Company Admin 
        /// </summary>
        /// <param name="employee"></param>
        public virtual void InsertCompanyAdmin(EmployeeModel employee)
        {
            if (employee.RoleId == Convert.ToInt32(EmployeeModel.Emprole.Admin))
            {
                employee.CreatedBy = employee.UpdatedBy;
                _catalogDbContext.CompanyAdminRepository.AddCompanyAdminDetails(employee);
            }
        }

        /// <summary>
        /// Method to Generate token
        /// </summary>
        /// <param name="securityStamp"></param>
        /// <returns></returns>
        public virtual string GenerateToken(string securityStamp)
        {
            var b = System.Text.ASCIIEncoding.ASCII.GetBytes(securityStamp);
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// Method Name  :   GetRedirectEmpListWithHierachyForActions
        /// Author       :   Vivek Bhavsar
        /// Creation Date:   09 July 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee when redirect from dashboard
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        public virtual void GetRedirectEmpListWithHierachyForActions(EmployeeModel employeeModel, List<EmployeeModel> allEmployee)
        {
            employeeModel.ManagerId = employeeModel.ManagerId == 0 ? GetEmployeeIdFromClaims() : employeeModel.ManagerId;
            List<EmployeeModel> allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeRepository.GetYearwiseEmployeeWithHierachy(employeeModel).Where(x => x.EmpStatus).ToList();
            allEmployee.Where(x => x.ManagerId == employeeModel.ManagerId).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.EmpId).ToList();
            allEmployee.Where(x => empids.Contains(x.EmpId) && x.ManagerId != Math.Abs(employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Hierarchal.GetHashCode());
            allEmployee.Where(x => x.EmpRelation == 0).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Indirect.GetHashCode());
        }

        /// <summary>
        /// Method to set employee profile parth to list of employee 
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> SetEmployeeProfilePic(List<EmployeeModel> employees)
        {
            string imgPath = _appSettings.StorageAccountURL + Messages.slash + _blobContainer + Messages.slash;
            employees?.ToList().ForEach(e => e.EmpImgPath = (e.EmpImgPath == null || e.EmpImgPath == string.Empty) ?
            string.Empty : imgPath + e.EmpImgPath);
            return employees;
        }

        /// <summary>
        /// Method to set employee profile parth to employee 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual EmployeeModel SetEmployeeProfilePic(EmployeeModel employee)
        {
            if (string.IsNullOrEmpty(employee.EmpImgPath))
            {
                employee.EmpImgPath = String.Empty;
            }
            else
            {
                employee.EmpImgPath = _appSettings.StorageAccountURL + Messages.slash + _blobContainer + Messages.slash + employee.EmpImgPath;
            }
            return employee;
        }

        /// <summary>
        /// Method to Get list of employee by empId's 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="empIdList"></param>
        /// <returns></returns>
        public virtual List<EmployeeModel> GetEmployeesDetail(int companyId, string empIdList)
        {
            EmployeeModel employeeModel = new EmployeeModel { CompanyId = companyId, EmpIdList = empIdList };
            return _connectionContext.TriggerContext.EmployeeRepository.GetEmployeeByEmpIdsForMails(employeeModel);
        }

        /// <summary>
        /// Method Name  :   GetEmpListWithHierachyForActions
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   2 July 2019
        /// Purpose      :   Get employeeList with hierachy to get employee relation with logged in employee
        /// </summary>
        /// <param name="employeeModel"></param>
        /// <param name="allEmployee"></param>
        public virtual void GetEmpListWithHierachyForActions(EmployeeListModel employeeModel, List<EmployeeListModel> allEmployee)
        {
            List<EmployeeListModel> allEmployeeHierachy = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployeeWithHierachy(employeeModel).Where(x => x.EmpStatus).ToList();

            allEmployee.Where(x => x.ManagerId == employeeModel.ManagerId).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Direct.GetHashCode());
            var empids = allEmployeeHierachy.Select(x => x.EmpId).ToList();
            allEmployee.Where(x => empids.Contains(x.EmpId) && x.ManagerId != Math.Abs(employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Hierarchal.GetHashCode());

            allEmployee.Where(x => x.EmpRelation == 0).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Indirect.GetHashCode());
        }

        /// <summary>
        /// Common method to get employee list as per action permission
        /// </summary>
        /// <param name="actionWisePermissionModel"></param>
        /// <param name="companyId"></param>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public virtual List<EmployeeListModel> GetEmployeeListForActions(List<ActionwisePermissionModel> actionWisePermissionModel, int companyId, int managerId)
        {
            EmployeeListModel employeeModel;
            List<EmployeeListModel> allEmployee;

            if (GetRoleIdFromClaims() == Enums.DimensionElements.CompanyAdmin.GetHashCode() || actionWisePermissionModel.Any(x => x.DimensionId == Enums.DimensionType.Role.GetHashCode() || x.DimensionId == Enums.DimensionType.Department.GetHashCode()))
            {
                employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = managerId };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                allEmployee.Where(x => x.ManagerId == (employeeModel.ManagerId == 0 ? GetEmployeeIdFromClaims() : employeeModel.ManagerId)).ToList().ForEach(x => x.EmpRelation = Enums.DimensionElements.Direct.GetHashCode());
            }
            else
            {
                employeeModel = new EmployeeListModel { CompanyId = companyId, ManagerId = 0 };
                allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);

                int empId = managerId == 0 ? GetEmployeeIdFromClaims() : managerId;
                GetEmpListWithHierachyForActions(new EmployeeListModel { CompanyId = companyId, ManagerId = empId }, allEmployee.Where(x => x.EmpId != empId).ToList());

                List<int> dimensionValues = actionWisePermissionModel.Where(x => x.DimensionId == Enums.DimensionType.Relation.GetHashCode()).Select(x => x.DimensionValueid).ToList();
                allEmployee = allEmployee.Where(x => dimensionValues.Contains(x.EmpRelation)).ToList();
            }

            return allEmployee;
        }

        /// <summary>
        /// Method to ger responsewith check no records
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual CustomJsonData GetResponseWithCheckNoRecord<T>(T data)
        {
            if (data != null)
            {
                return JsonSettings.UserCustomDataWithStatusMessage(data, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            else
            {
                return JsonSettings.UserCustomDataWithStatusMessage(data, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.noRecords);
            }
        }

        /// <summary>
        /// Method to ger responsewith check no AccessDenied
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual CustomJsonData GetResponseWitCheckAccessDenied<T>(T data)
        {
            if (data != null)
            {
                return JsonSettings.UserCustomDataWithStatusMessage(data, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            else
            {
                return JsonSettings.UserCustomDataWithStatusMessage(data, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
            }
        }

        /// <summary>
        /// Method to get role id from claims
        /// </summary>
        /// <returns></returns>
        public virtual int GetRoleIdFromClaims()
        {
            return Convert.ToInt32(_iClaims["RoleId"].Value);
        }

        /// <summary>
        /// Method to get employee id from claims
        /// </summary>
        /// <returns></returns>
        public virtual int GetEmployeeIdFromClaims()
        {
            return Convert.ToInt32(_iClaims["EmpId"].Value);
        }

        /// <summary>
        /// Method to get action wise permission
        /// </summary>
        /// <returns></returns>
        public virtual ActionList GetAllPermission(int managerId, int actionId)
        {
            var allPermission = _actionPermission.GetPermissions(managerId);
            ActionList actionPermission = allPermission.FirstOrDefault(x => x.ActionId == actionId);
            return actionPermission;
        }
    }
}
