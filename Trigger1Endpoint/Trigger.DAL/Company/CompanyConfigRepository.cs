using OneRPP.Restful.DataAnnotations;
using OneRPP.Restful.DAO;
using Trigger.DTO;
namespace Trigger.DAL.Company
{
	[QueryPath("Trigger.DAL.Query.CompanyConfig.CompanyConfig")]
	public class CompanyConfigRepository : DaoRepository<CompanyConfigModel>
	{
        private const string invokeCompanyConfigInsert = "CompanyConfigInsert";

        /// <summary>
        /// insert role based on policy
        /// </summary>
        /// <param name="companyConfigModel">The CompanyConfigModel</param>
        /// <returns>boolean value</returns>
        public virtual CompanyConfigModel InsertCompanyConfig(CompanyConfigModel companyConfigModel)
		{
			return ExecuteQuery<CompanyConfigModel>(companyConfigModel, invokeCompanyConfigInsert);
		}
    }
}
