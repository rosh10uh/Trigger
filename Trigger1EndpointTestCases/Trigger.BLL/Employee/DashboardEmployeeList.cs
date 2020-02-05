using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.Utility;

namespace Trigger.BLL.Employee
{
    public class DashboardEmployeeList
    {

        private readonly IClaims _iClaims;
        private readonly IConnectionContext _connectionContext;
        private readonly IActionPermission _actionPermission;
        private readonly ILogger<DashboardEmployeeList> _logger;
        private readonly Employee _employee;

        public DashboardEmployeeList(IConnectionContext connectionContext, IClaims Claims, IActionPermission actionPermission, ILogger<DashboardEmployeeList> logger, Employee employee)
        {
            _connectionContext = connectionContext;
            _iClaims = Claims;
            _actionPermission = actionPermission;
            _logger = logger;
            _employee = employee;
        }

        public async Task<CustomJsonData> GetDashboardEmpListV2_1(int companyId, int managerId)
        {
            try
            {
                List<EmployeeListModel> allEmployee = null;

                if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    EmployeeListModel employeeModel = new EmployeeListModel { companyId = companyId, managerId = managerId };
                    allEmployee = _connectionContext.TriggerContext.EmployeeListRepository.GetAllEmployee(employeeModel);
                }
                else
                {
                    ActionList allPermission = await Task.FromResult(_actionPermission.GetPermissionsV2_1(managerId == 0 ? Convert.ToInt32(_iClaims["EmpId"].Value) : managerId).FirstOrDefault(x => (x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode())));

                    List<ActionwisePermissionModel> permission = null;
                    permission = allPermission.ActionPermissions.Where(x => x.CanView).ToList();

                    if (permission != null && permission.Count > 0)
                    {
                        allEmployee = _employee.GetEmployeeListForActionsV2_1(permission, companyId, managerId);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(new List<EmployeeListModel>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                    }
                }

                if (allEmployee.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(allEmployee, Convert.ToInt32(Enums.StatusCodes.status_204), Messages.accessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
    }
}
