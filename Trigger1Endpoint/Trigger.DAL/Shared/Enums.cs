namespace Trigger.BLL.Shared
{
    public static class Enums
    {
        public enum StatusCodes
        {
            status_100 = 100, //
            status_200 = 200, //Ok success code
            status_203 = 203, //Invalid Password
            status_204 = 204, //You have not been given access to connected services. Please contact your firm administrator for access
            status_208 = 208, //Entity(company,department,email) not exists
            status_209 = 209, //Employee ID Already exists
            status_304 = 304, //phone number already verified/sms service already on/off
            status_400 = 400, //Phone Number not available
            status_403 = 403, //Access denied in case dimension wise permission not set
            status_401 = 401, //Unauthorized access
            status_402 = 402, //user deleted/inactive,company delete or contract expire
            status_404 = 404, //Not Found
            status_426 = 426, //status code used for Forcfully Mobile Apps Update
            status_410 = 410, //verification code expired or Invalid verification code
            status_412 = 412, // Server side validation fail
            status_500 = 500  //Internal server error (default message for system exception)
        }

        public enum ResultTypeForInset
        {
            EmployeeEmailIdExist = 0,
            EmployeeSubmit = 1,
            EmailIdExist = -2,
            PhoneNumberExist = -3
        }
        public enum ResultTypeForUpdate
        {
            EmployeeNotExist = 0,
            EmployeeUpdate = 1,
            EmailIdExist = 2,
            EmployeeIdExist = 3,
            PhoneNumberExist = 4
        }

        public enum ClaimType
        {
            CompId,
            RoleId,
            EmpId,
            Key
        }

        public enum Guid
        {
            D,
            B
        }

        public enum DeviceType
        {
            Android,
            iOS
        }
        public enum TemplateType
        {
            Email = 1,
            SMS = 2
        }

        public enum DimensionType
        {
            Role = 1,
            Relation = 2,
            Department = 3,
            Team = 4

        }

        public enum DimensionElements
        {
            #region Role

            TriggerAdmin = 1,
            CompanyAdmin = 2,
            Manager = 3,
            Executive = 4,
            NonManager = 5,

            #endregion

            #region Relation

            Direct = 1,
            Hierarchal = 2,
            Indirect = 3,

            #endregion

            #region Department

            Inside = 1,
            Outside = 2,

            #endregion

            #region Team

            Connected = 1,
            Oversight = 2,
            #endregion
        }

        public enum Actions
        {
            TriggerEmployee = 1,
            SparkEmployee = 2,
            EmployeeDashboard = 3,
            Comment = 4,
            TeamConfiguration = 5,
            TeamDashboard = 6
        }

        public enum PermissionType
        {
            CanView = 1,
            CanAdd = 2,
            CanEdit = 3,
            CanDelete = 4
        }

        public enum TransactionType
        {
            Insert = 1,
            Update = 2,
            Delete = 3
        }

        public enum Category
        {
            Performance = 1,
            Attitude = 2,
            Maintenance = 3,
            General = 4
        }

        public enum Classifications
        {
            GroupMeeting = 1,
            OnetoOneMeeting = 2,
            PassingConversation = 3,
            Observation = 4,
            WorkProductReceived = 5,
            General = 6
        }

        public enum SparkStatus
        {
            UnApproved = 0,
            Approved = 1,
            Rejected = 2
        }
    }
}
