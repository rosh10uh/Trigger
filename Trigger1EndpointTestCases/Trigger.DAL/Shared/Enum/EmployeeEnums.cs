namespace Trigger.DAL.Shared.Enum
{
    /// <summary>
    /// All Employee module related enums
    /// </summary>
    /// <returns></returns>
    public static class EmployeeEnums
    {
        /// <summary>
        /// Enums for Insert employee return type
        /// </summary>
        /// <returns></returns>
        public enum InsertResultType
        {
            EmployeeEmailIdExist = 0,
            EmployeeSubmit = 1,
            EmailIdExist = -2,
            PhoneNumberExist = -3
        }

        /// <summary>
        /// Enums for Edit employee return type
        /// </summary>
        /// <returns></returns>
        public enum UpdateResultType
        {
            EmployeeNotExist = 0,
            EmployeeUpdate = 1,
            EmailIdExist = 2,
            EmployeeIdExist = 3,
            PhoneNumberExist = 4
        }

        /// <summary>
        /// Enums for Update employee profile return type
        /// </summary>
        /// <returns></returns>
        public enum ProfileUpdateResultType
        {
            EmployeeNotExist = 0,
            ProfileUpdated = 1,
            EmpIsInActive = 2,
            PhoneNumberIsExist = 4
        }

        /// <summary>
        /// Enums for active sms service return type
        /// </summary>
        /// <returns></returns>
        public enum SmsServiceResultType
        {
            EmployeeNotExist = 0,
            ActiveSMSNotification = 1,
            EmpIsInActive = 2,
            PhoneNumberNotVerify = 3,
            AlreadyDone = 4
        }

        /// <summary>
        /// Enums for update salary return type
        /// </summary>
        /// <returns></returns>
        public enum UpdateSalaryResultType
        {
            EmployeeNotExist = 0,
            SalaryUpdated = 1,
            EmployeeInActive = 2,

        }
    }
}
