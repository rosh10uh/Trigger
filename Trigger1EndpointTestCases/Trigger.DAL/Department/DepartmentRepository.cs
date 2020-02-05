using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.BLL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.Department
{
    [QueryPath("Trigger.DAL.Query.Department.Department")]
    public class DepartmentRepository : DaoRepository<DepartmentModel>
    {
        public virtual List<CompanyWiseDepartmentModel> GetCompanyAndYearwiseDepartment(DepartmentModel departmentModel)
        {
            return ExecuteQuery<List<CompanyWiseDepartmentModel>>(departmentModel, Messages.invokeGetCompanyAndYearWiseDepartments);
        }

        public virtual List<CompanyWiseDepartmentModel> GetCompanywiseDepartment(DepartmentModel departmentModel)
        {
            return ExecuteQuery<List<CompanyWiseDepartmentModel>>(departmentModel, Messages.invokeGetCompanyWiseDepartments);
        }
    }
}
