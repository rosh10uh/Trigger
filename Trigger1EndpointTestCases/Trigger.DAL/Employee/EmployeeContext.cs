using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;

namespace Trigger.DAL.Employee
{
    /// <summary>
    /// Contains method to manage multiple operation.
    /// </summary>
    public class EmployeeContext
    {
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IConnectionContext _connectionContext;

        /// <summary>
        /// Constructor of EmployeeContext class
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="catalogDbContext"></param>
        public EmployeeContext(IConnectionContext connectionContext, TriggerCatalogContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
            _connectionContext = connectionContext;
        }

        /// <summary>
        /// Method to update user details
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public virtual int UpdateUser(UserDetails userDetails)
        {
            var emp = _connectionContext.TriggerContext.UserDetailRepository.UpdateUser(userDetails);
            return emp.result;
        }

        /// <summary>
        /// Method to update user auth details
        /// </summary>
        /// <param name="authUserDetails"></param>
        /// <returns></returns>
        public virtual void Update1AuthUser(AuthUserDetails authUserDetails)
        {
            _catalogDbContext.AuthUserDetailsRepository.Update(authUserDetails);
        }

        /// <summary>
        /// Method to update user claims details
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public virtual void UpdateAuthUserClaims(Claims claims)
        {
            _catalogDbContext.ClaimsCommonRepository.Update(claims);
        }

        /// <summary>
        /// Method to update employee isMailsent flag on registration email sent
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public virtual int UpdateEmpIsMailSent(EmployeeModel employee)
        {
            var emp = _connectionContext.TriggerContext.EmployeeRepository.UpdateEmpForIsMailSent(employee);
            return emp.Result;
        }

        /// <summary>
        /// Method to delete auth user delete
        /// </summary>
        /// <param name="authUserDetails"></param>
        /// <returns></returns>
        public virtual AuthUserDetails DeleteAuthUser(AuthUserDetails authUserDetails)
        {
            return _catalogDbContext.AuthUserDetailsRepository.Delete(authUserDetails);
        }
    }
}
