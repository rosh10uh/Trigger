using System;
using System.Collections.Generic;

namespace Trigger.DTO
{
    public class UserLogin
    {
        public int userId { get; set; }
        public int empId { get; set; }
        public int roleId { get; set; }
        public int companyid { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string companyname { get; set; }
        public string role { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public string empEmailId { get; set; }
        public EmployeeModel employee { get; set; }
        public bool bActive { get; set; }
        public int result { set; get; }
        public int existingEmpId { set; get; }
        public UserLogin()
        {
            employee = new EmployeeModel();
        }
        public string key { get; set; }
    }

    public class UserDetails
    {
        public int empId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int createdBy { get; set; }
        public int updatedBy { get; set; }
        public bool bActive { get; set; }
        public int result { set; get; }
        public int CompId { set; get; }
        public int existingEmpId { set; get; }        
    }

    public class AuthUserDetails
    {
        public string Id { get; set; }

        public string UserId { set; get; }

        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset LockoutEnd { get; set; }

        public string NormalizedEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UserName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string Token { get; set; }

        public string UserClient { get; set; }

        public List<Claims> Claims { get; set; }

        public List<Roles> Roles { get; set; }

        public DateTime TokenExpiration { get; set; }

        public string FullName { get; set; }

        public string ExistingEmail { get; set; }

        public string SubId { get; set; }
    }

    public class Claims
    {
        public string Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string AuthUserId { get; set; }
    }

    public class Roles
    {
        public string Name { get; set; }

        public string UserId { get; set; }
    }
}
