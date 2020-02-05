using System.Text;

namespace Trigger.BLL.Shared
{
    /// <summary>
    /// Class Name      : MagicStrings
    /// Author          : Mitali Patel
    /// Creation Date   : 25 Jan 2019
    /// Purpose         : General class to provide collection of all Magic string ,StatusCode & Constant Messages values
    /// Revision        : 
    /// </summary>
    public static class Messages
    {
        public const string ok = "Ok";
        public const string From = "From";
        public const string allReadTriggerEmployee = "Whoops! It looks like you have already Triggered this employee today, come back tomorrow.";
        public const string notTriggerEmployee = "Whoops! It looks like you haven't Triggered this employee yet.";
        public const string partialAssement = "Partial Assessmnet Done Successfully.";
        public const string internalServerError = "Internal Server Error.";
        public const string companySubmitted = "Company Submitted.";
        public const string companyExists = "Company is already exists.";
        public const string unauthorizedForAddCompany = "You are not authorized to add company.";
        public const string updatedCompany = "Company Updated.";
        public const string companyIdIsExists = "Company id is already exists.";
        public const string unauthorizedForEditCompany = "You are not authorized to edit company.";
        public const string deleteCompany = "Company Deleted.";
        public const string inactiveCompany = "Company is inactive.";
        public const string unauthorizedForDeleteCompany = "You are not authorized to delete company.";
        public const string employeeSubmit = "Employee Submitted.";
        public const string employeeExist = "Employee already exists.";
        public const string employeeIdIsExists = "Employee id already exist.";
        public const string employeeEmailIdIsExists = "Employee email id already exist.";
        public const string phoneNumberIsExists = "Phone number already exist.";
        public const string smsserviceallowed = "SMS notification is already opted.";
        public const string smsservicenotallowed = "SMS notification is already denied.";
        public const string profilePhotoUpdated = "Profile Photo Updated.";
        public const string empProfileoUpdated = "Employee Profile Updated.";
        public const string empSalaryUpdated = "Employee Salary Updated.";
        public const string empIsInactive = "Employee is inactive.";
        public const string phoneNumberIsNotVerified = "Phone number is not verified.";
        public const string employeeNotExist = "Employee not exist.";
        public const string updateEmployee = "Employee Updated.";
        public const string deleteEmployee = "Employee Deleted.";
        public const string addDepartment = "New Department Added successfully.";
        public const string departmentIsExists = "Department Already Exists.";
        public const string updateDepartment = "Department Updated successfully.";
        public const string cannotDeleteDepartment = "Cannot delete department in use by an employee.";
        public const string deleteDepartment = "Department Deleted successfully.";
        public const string configuredDepartment = "Department Configured successfully.";
        public const string accessDenied = "You have not been given access to connected services. Please contact your firm administrator for access.";
        public const string employeeAccessDenied = "Your permission has been changed, please re-login again.";
        public const string unauthorizedToApproveSpark = "You are not authorized to approve or reject spark.";
        public const string noRecords = "No Records Found.";
        public const string unauthorizedToDeleteEmployee = "You are not authorized to delete employee.";
        public const string unauthorizedToAddEmployee = "You are not authorized to add employee.";
        public const string unauthorizedToUpdateEmployee = "You are not authorized to update employee.";
        public const string contactRequestSent = "Request has been submitted.";
        public const string somethingWentWrong = "Something went wrong.";
        public const string receiveNewsletters = "Thank you for subscribed. You will receive newsletters from Trigger Transformation.";
        public const string notInitiatedAssessmentEmployee = "Assessment is not initiated for this employee.";
        public const string unauthorizedForAssessment = "You can edit/delete the comments/attachment only given by you.";
        public const string AssessmentCommentUpdated = "Comment updated successfully.";
        public const string AssessmentCommentDelete = "Comment deleted successfully.";
        public const string AttachmentDeleted = "Attachment deleted successfully.";
        public const string unauthorizedSelfAssessment = "Self assessment not permissible.";
        public const string AssessmentScoreFeedbackUpdated = "Assessment score feedback updated successfully.";
        public const string AssessmentDetailsNotFound = "Assessment details not found.";

        public const string dataNotFound = "No data found.";
        public const string mobileAppsForManagerAndExecutiveLogin = "Mobile apps are for manager, executive and company admin login only. For 'Trigger Admin' menus, Please login to the website.";
        public const string userLogIn = "User logged in successfully!";
        public const string deleteDeviceInfo = "DeviceInfo Deleted.";
        public const string noDataFound = "Data not found.";
        public const string importData = "Data Imported Successfully.";
        public const string invalidPassword = "Invalid Current Password.";
        public const string changePassword = "Your Password has been changed successfully!";
        public const string userNotExist = "User Does Not Exist.";
        public const string passwordSentToEmail = "Password successfully sent to your email.";
        public const string noNotifications = "No new notifications.";
        public const string userGuideSentToYourEmail = "User guide has been sent to your email address.";
        public const string alreadySubscribedTrigger = "You have already subscribed Trigger.";
        public const string positionsaved = "Position saved successfully.";
        public const string configureWidget = "Widget configured successfully.";
        public const string badRequest = "Bad Request.";
        public const string unauthorizedAcces = "Unauthorized Access.";
        public const string notFound = "Not Found.";
        public const string mailSent = "Registration mail sent successfully.";
        public const string updateEmpForIsMailSent = "usp_UpdateEmpForIsMailSent";
        public const string verificationCodeTimeExpired = "Your verification code for SMS notification expired.";
        public const string invalidVerificationCode = "Invalid verification code for SMS notification.";
        public const string smsCodeVerified = "Your phone number verified successfully.";
        public const string verificationCodeSent = "Phone number verification code successfully sent.";
        public const string phoneNumberNotExists = "Phone number not registered for user.";
        public const string phoneNumberNotVerified = "Phone number not verified for SMS notification.";
        public const string phoneNumberAlreadyVerified = "Phone number verification already done.";
        public const string companyLogo = "companylogo";
        public const string profilePic = "profilepic";
        public const string AssessmentDocPath = "assessmentdoc";
        public const string DollarSign = "$";
        public const string SmsBodySplitter = "&";
        public const string companyName = "Trigger Transformation";
        public const string enquiry = "Enquiry";
        public const string prefixValues = "Select Prefix,Capt.,Dr.,Mayor,Mr.,Ms.,Mrs.,III,Jr.,Sr.";
        public const string gender = "Select Gender, Male,Female";
        public const string status = "Select Status, Active,InActive";
        public const string tenantConnectionString = "TenantConnectionString";
        public const string startExecuteDashbord = "Start Executing Dashboard.";
        public const string endExecuteDashbord = "End Executing Dashboard.";
        public const string triggerAPI = "TriggerAPI";
        public const string triggerClientAPI = "TriggerClientApi";
        public const string triggerClient = "TriggerClient";
        public const string triggerClientSecret = "TriggerClientSecret";
        public const string emailSubject = "Trigger - Your password has been changed.";
        public const string username = "username";
        public const string usernameRequired = "Username is required.";
        public const string validEmail = "Enter valid email.";
        public const string helloWorld = "Helloworld";
        public const string cryptoKey = "TTDRB01";
        public const string dateFormat = "MM-dd-yyyy";
        public const string DateTimeFormat = "MM-dd-yyyy HH:mm:ss";
        public const string dateHeaderFormat = "(MM-DD-YYYY)";
        public const string companyLogoUser = "{CompLogo}";
        public const string headingtext = "{Headingtext}";
        public const string FirstName = "{FirstName}";
        public const string LastName = "{LastName}";
        public const string PhoneNumber = "{PhoneNumber}";

        public const string inActivityDays = "{InActivityDays}";
        public const string userId = "{UserId}";
        public const string landingURL = "{LandingURL}";
        public const string triggerLogo = "{TriggerLogo}";
        public const string resetPasswordURL = "{ResetPasswordURL}";
        public const string registration = "Trigger Transformation | User Registration";
        public const string inActivityReminder = "Trigger Transformation | InActivity Reminder";
        public const string failToUploadExcel = "Fail to Add Company Admin[Excel Upload].";
        public const string apiManager = "/api/Manage/";
        public const string contantType = "Content-Type";
        public const string jsonFilePath = "application/json";
        public const string userAgent = "User-Agent";
        public const string authorization = "Authorization";
        public const string triggerSecretAPI = "Triggersecretapi";
        public const string tokenValid = "Token Valid: ";
        public const string tokenValidError = "Token Valid error: ";
        public const string tokenEmpty = "Token Empty: ";
        public const string accessToken = "access_token";
        public const string helloWorldMsg = "Helloworld";
        public const string theAdmin = "The Admin";
        public const string jsonPost = "jsonpost";
        public const string appUpgradeMessage = "Please update your app for a better viewing experience!";
        public const string TeamConfiguration = "Trigger Transformation | Welcome to the Team - {0}";
        public const string TeamConfigurationUpdate = "Trigger Transformation | Team configuration update - {0}";
      

        //Create Database Messages
        public const string createTable = "CreateSchema_Tables.sql";
        public const string createExternalTable = "CreateSchema_ExternalTables.sql";
        public const string tableType = "CreateSchema_TablesTypes.sql";
        public const string tableFunctions = "CreateSchema_Functions.sql";
        public const string createTrigger = "CreateSchema_Trigger.sql";
        public const string tableView = "CreateSchema_Views.sql";
        public const string sps = "CreateSchema_SPs.sql";
        public const string insertMasterData = "InsertMasterData.sql";
        public const string insertDepartmentMaster = "InsertDepartmentMaster.sql";
        public const string failToCreateNewTenant = "Fail to create New Tenant.";
        public const string alterDBError = "Added to Pool.";
        public const string dBPassword = "DBPassword";
        public const string dBUserName = "DBUserName";
        public const string dBServerName = "DBServerName";
        public const string dbDrop = "DB Dropped";
        public const string dropDatabase = "DROP DATABASE[{0}]";
        public const string indexDB = "IndexDB";
        public const string created = "Created";
        public const string dbName = "DbName";
        public const string disconnectWithDB = "Unable to connect to the database.";
        public const string createDBError = "DB Created.";
        public const string email = "Email  : ";
        public const string phone = "Phone  : ";
        public const string subject = "Subject  : ";
        public const string description = "Description  :";
        public const string supportMsg = "Kindly get in touch." + "\n\n" + "Thank You,\n";
        public const string confirmEmailPath = Messages.slash + "Account" + Messages.slash + "ConfirmEmail?UserId=";
        public const string queryFolderPath = "QueryFolderPath";
        public const string failToAddCompanyAdmin = "Fail to Add Company Admin.";
        public const string failToDeleteCompanyAdmin = "Fail to Delete Company Admin.";
        public const string code = "&code=";
        public const string roleid = "roleid=2";

        public const string dbMessage = "roleid NOT IN(1,5) AND email <> ''";
        public const string questionsMsg = " and questions <> ''";
        public const string promptForCountry = "Select Country.";
        public const string errorTitleForCountry = "Invalid Country.";
        public const string errorForCountry = "Please select country from the given list.";
        public const string excelFormulaForCountry = "MasterData!$A$1:$A$";
        public const string promptForRegion = "Select Region.";
        public const string errorTitleForRegion = "Invalid Region.";
        public const string errorForRegion = "Please select region from the given list.";
        public const string excelFormulaForRegion = "MasterData!$B$1:$B$";
        public const string promptForDepartment = "Select Department.";
        public const string errorTitleForDepartment = "Invalid Department.";
        public const string errorForDepartment = "Please select department from the given list.";
        public const string excelFormulaForDepartment = "MasterData!$C$1:$C$";
        public const string promptForRole = "Select Role.";
        public const string errorTitleForRole = "Invalid Role.";
        public const string errorForRole = "Please select role from the given list.";
        public const string excelFormulaForRole = "MasterData!$D$1:$D$";
        public const string promptForManager = "Select Manager.";
        public const string errorTitleForManager = "Invalid Manager.";
        public const string errorForManager = "In case the manager's name is not in the list, write the manager's EmployeeID in the ExcelManagerID column.";
        public const string excelFormulaForManager = "MasterData!$E$1:$E$";
        public const string promptForEthanicity = "Select Ethanicity.";
        public const string errorTitleForEthanicity = "Invalid Ethanicity.";
        public const string errorForEthanicity = "Please select Ethanicity from the given list.";
        public const string excelFormulaForEthanicity = "MasterData!$F$1:$F$";
        public const string promptForCountryCallingCode = "Select Country Calling Code.";
        public const string promptForCountryCode = "Select Country Calling Code.";
        public const string errorForCountryCode = "Please select Country Calling Code from the given list.";
        public const string excelFormulaForCountryCode = "MasterData!$G$1:$G$";
        public const string errorTitleForCountryCode = "Invalid Country Calling Code.";
        public const string promptForEmloyeeStatus = "Select EmloyeeStatus.";
        public const string errorTitleForEmloyeeStatus = "Invalid Emloyee Status.";
        public const string errorForEmloyeeStatus = "Please select Emloyee Status from the given list.";
        public const string promptForGender = "Select Gender.";
        public const string errorTitleForGender = "Invalid Gender.";
        public const string errorForGender = "Please select Gender from the given list.";
        public const string promptForPrefix = "Select Prefix.";
        public const string errorTitleForPrefix = "Invalid Prefix.";
        public const string errorForPrefix = "Please select Prefix from the given list.";
        public const string supportMsgForContactUs = "Dear Support Team," + "\n\nYou have received enquiry from  {0} {1} \n\n" + "Kindly take note of below contact info :" + "\n\n" + "Email  :  {2}\n\n";
        public const string newLine = "\n\n";
        public const string doubleCollen = "://";
        public const string doubleSlash = @"\\";
        public const string slash = "/";
        public const string id = "0";
        public const string comma = ",";
        public const string emailpattern = @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
        public const string doubleSlashWithoutCollon = "\\";
        public const string deviceId = "DeviceId";
        public const string deviceType = "DeviceType";
        public const string deviceTypeiOS = "deviceType = 'iOS'";
        public const string jobId = " - Job Id :[{0}]\n";
        public const string bodyMsgForContactUs = "Dear {0} {1} , \n\n\nThank you for contacting us. We have received your email and will reply shortly. " + " \n\n\nThank You,\n {2}";
        public const string alterDBForCompany = "ALTER DATABASE [{0}] MODIFY ( SERVICE_OBJECTIVE = ELASTIC_POOL ( name ={1}) )";
        public const string createDBForCompany = " CREATE DATABASE [{0}]";
        public const string conntectionstring = "Server={0};Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0;";
        public const string notifyForEmployee = "{ \"data\" : {\"message\":\"[0]\"},\"type\":\"[1]\",\"id\":\"[2]\"}";
        public const string alertForEmployee = "{\"aps\":{\"alert\":\"[0]!\",\"badge\":1},\"type\":\"[1]\",\"id\":\"[2]\"}";
        public const string bodyMsgForLogIn = "Hi," + "\n\n\nWe have received a request to reset password for your" + " Trigger account.\nHere is your new password : {0}\n\n\nThanks,\nThe Admin";
        public const string messageForSendEmailToUser = "Hi {0}," + "\n\n\nWe have received a request to reset password for your " + "Trigger account.\nHere is your new password :{1} \n\n\nThanks,\nThe {2}";
        public const string data = "{ \"subscribers\" : [{\"email\":\"{0}\",\"double_optin\": false }]}";
        public const string GetCompanyAdminById = "GetCompanyAdminById";
        public const string invokeGetEmployeeById = "GetEmployeeById";
        public const string invokeGetAssessmentScore = "GetAssessmentScore";
        public const string invokeGetEmpLastAssessmentDetail = "GetEmpLastAssessmentDetail";
        public const string invokeAddAssessment = "AddAssessment";
        public const string invokeAddPartialAssessment = "AddPartialAssessment";
        public const string invokeAddAssessmentDetails = "AddAssessmentDetails";
        public const string invokeUpdateAssessmentScore = "UpdateAssessmentScore";
        public const string invokeDeleteCompany = "DeleteCompany";
        public const string invokeGetAllCompany = "GetAllCompany";
        public const string invokeGetAllIndustryType = "GetAllIndustryType";
        public const string invokeAddCompany = "AddCompany";
        public const string invokeUpdateCompany = "UpdateCompany";
        public const string invokeAddCompanyConfig = "AddCompanyConfig";
        public const string invokeGetCompanyById = "GetCompanyById";
        public const string invokeAddContactUs = "AddContactUs";
        public const string invokeAddSubscriber = "AddSubscriber";
        public const string invokeGetExcelDataWithCount = "GetExcelDataWithCount";
        public const string invokeDeleteTempEmployee = "DeleteTempEmployee";
        public const string invokeGetAllMastersForExcel = "GetAllMastersForExcel";
        public const string invokeAddEmpolyeeFromExcel = "AddEmpolyeeFromExcel";
        public const string invokeGetExcelNewEmployees = "GetExcelNewEmployees";
        public const string invokeAddUserLoginFromExcel = "AddUserLoginFromExcel";
        public const string invokeAddAuthLoginFromExcel = "AddAuthLoginFromExcel";
        public const string invokeGetExistingAuthUsers = "GetExistingAuthUsers";
        public const string invokeAddAuthClaimsFromExcel = "AddAuthClaimsFromExcel";
        public const string invokeGetConnectionString = "GetConnectionString";
        public const string invokeGetTenantName = "GetTenantName";
        public const string invokeGetCompanyName = "GetCompanyName";
        public const string invokeGetAssessmentYear = "GetAssessmentYear";
        public const string invokeGetEmployeeDashboard = "GetEmployeeDashboard";
        public const string invokeAddWidgetPosition = "AddWidgetPosition";
        public const string invokeDeleteCompanyAndRoleWiseWidget = "DeleteCompanyAndRoleWiseWidget";
        public const string invokeAddCompanyAndRoleWiseWidget = "AddCompanyAndRoleWiseWidget";
        public const string invokeGetAllWidgetWithType = "GetAllWidgetWithType";
        public const string invokeGetUserwiseWidget = "GetUserwiseWidget";
        public const string invokeGetEmployeeByCompId = "GetEmployeeByCompId";
        public const string invokeAddRegisterUser = "AddRegisterUser";
        public const string invokeGetAllQuestionnariesCategory = "GetAllQuestionnariesCategory";
        public const string invokeUpdateNotificationMarkAsRead = "UpdateNotificationMarkAsRead";
        public const string invokeGetAllNotification = "GetAllNotification";
        public const string invokeLogin = "Login";
        public const string invokeGetUserByUserName = "GetUserByUserName";
        public const string invokeGetSubIdByUserName = "GetSubIdByUserName";
        public const string invokeResetPassword = "ResetPassword";
        public const string invokeResetAuthPassword = "ResetAuthPassword";
        public const string invokeGetUserByUserId = "GetUserByUserId";
        public const string invokeGetPasswordHashByUserName = "GetPasswordHashByUserName";
        public const string invokeChangeAuthPassword = "ChangeAuthPassword";
        public const string invokeRegisterDevice = "RegisterDevice";
        public const string invokeDeleteDeviceInfo = "DeleteDeviceInfo";
        public const string invokeGetUserDetails = "GetUserDetails";
        public const string invokeGetSubIdByEmail = "GetSubIdByEmail";
        public const string invokeDeleteEmployee = "DeleteEmployee";
        public const string invokeDeleteUser = "DeleteUser";
        public const string invokeAddDepartment = "AddDepartment";
        public const string invokeUpdateDepartment = "UpdateDepartment";
        public const string invokeDeleteDepartment = "DeleteDepartment";
        public const string invokeDeleteCompanyWiseDepartment = "DeleteCompanyWiseDepartment";
        public const string invokeAddCompanyWiseDepartment = "AddCompanyWiseDepartment";
        public const string invokeAddEmployee = "AddEmployee";
        public const string invokeAddUser = "AddUser";
        public const string invokeAddUserClaims = "AddUserClaims";
        public const string invokeUpdateEmployee = "UpdateEmployee";
        public const string invokeUpdateEmpProfile = "UpdateEmpProfile";
        public const string invokeUpdateUser = "UpdateUser";
        public const string invokeUpdateAuthUser = "UpdateAuthUser";
        public const string invokeUpdateAuthUserClaim = "UpdateAuthUserClaim";
        public const string invokeUpdateNotificationFlagIsSent = "UpdateNotificationFlagIsSent";
        public const string invokeGetDeviceInfoById = "GetDeviceInfoById";
        public const string invokeGetNotificationById = "GetNotificationById";
        public const string invokeGetAllCountry = "GetAllCountry";
        public const string invokeGetAllDepartment = "GetAllDepartment";
        public const string invokeGetAllEmployeeByManager = "GetAllEmployeeByManager";
        public const string invokeGetAllEmployee = "GetAllEmployee";
        public const string invokeGetAllEmployeeForTriggerAdminWithPagination = "GetAllEmployeeForTriggerAdminWithPagination";
        public const string invokeGetYearWiseAllEmployeeByManager = "GetYearWiseAllEmployeeByManager";
        public const string invokeGetAllEmployeeByManagerWithPagination = "GetAllEmployeeByManagerWithPagination";
        public const string invokeGetYearWiseAllEmployees = "GetYearWiseAllEmployees";
        public const string invokeGetAllEmployeeWithPagination = "GetAllEmployeeWithPagination";
        public const string invokeGetAllJobCategory = "GetAllJobCategory";
        public const string invokeGetAllJobCode = "GetAllJobCode";
        public const string invokeGetAllJobGroup = "GetAllJobGroup";
        public const string invokeGetAllRaceOrEthnicity = "GetAllRaceOrEthnicity";
        public const string invokeGetAllRegion = "GetAllRegion";
        public const string invokeGetAllRole = "GetAllRole";
        public const string invokeGetCompanyAndYearWiseDepartments = "GetCompanyAndYearWiseDepartments";
        public const string invokeGetCompanyWiseDepartments = "GetCompanyWiseDepartments";
        public const string invokeGetCompanyWiseEmployee = "GetCompanyWiseEmployee";
        public const string invokeGetCompanyWiseEmployeeWithPagination = "GetCompanyWiseEmployeeWithPagination";
        public const string invokeGetEmployeeByGradeByManager = "GetEmployeeByGradeByManager";
        public const string invokeGetEmployeeByGrade = "GetEmployeeByGrade";
        public const string invokeGetEmployeeByGradeAndMonthByManager = "GetEmployeeByGradeAndMonthByManager";
        public const string invokeGetEmployeeByGradeAndMonth = "GetEmployeeByGradeAndMonth";
        public const string invokeGetYearWiseEmployeeByGradeAndMonthByManager = "GetYearWiseEmployeeByGradeAndMonthByManager";
        public const string invokeGetEmployeeByGradeAndMonthByManagerWithPagination = "GetEmployeeByGradeAndMonthByManagerWithPagination";
        public const string invokeGetYearWiseEmployeeByGradeAndMonth = "GetYearWiseEmployeeByGradeAndMonth";
        public const string invokeGetEmployeeByGradeAndMonthWithPagination = "GetEmployeeByGradeAndMonthWithPagination";
        public const string invokeGetYearWiseEmployeeByGradeByManager = "GetYearWiseEmployeeByGradeByManager";
        public const string invokeGetEmployeeByGradeByManagerWithPagination = "GetEmployeeByGradeByManagerWithPagination";
        public const string invokeGetYearWiseEmployeeByGrade = "GetYearWiseEmployeeByGrade";
        public const string invokeGetEmployeeByGradeWithPagination = "GetEmployeeByGradeWithPagination";
        public const string invokeGetAllEmployeeForTriggerAdmin = "GetAllEmployeeForTriggerAdmin";
        public const string getDepartmentWiseManagerDashboard = "usp_GetDepartmentWiseManagerDashboard";
        public const string getYearlyDepartmentWiseManagerDashboard = "usp_GetYearlyDepartmentWiseManagerDashboard";
        public const string getManagerDashboard = "usp_GetManagerDashboard";
        public const string uspUserLogin = "usp_UserLogin";
        public const string uspAddCompanyAdminDetails = "usp_AddCompanyAdminDetailsNEW";
        public const string usp_GetAllQuestAnswers = "usp_GetAllQuestAnswers";
        public const string usp_DeleteCompanyAdminDetails = "usp_DeleteCompanyAdminDetailsNEW";
        public const string empIdFilter = "isnull(empid,0)>0";
        public const string usp_GetInactivityManagers = "usp_GetInactivityManagers";
        public const string usp_AddInactivityManagers = "usp_AddInActivityLog";
        public const string getCompanyDetails = "select compid,contractstartdate,contractenddate,graceperiod,inactivitydays,reminderdays from companydetails \n" +
                                                     "where bactive = 1 AND compid <> 1 ";

        // Template Name
        public const string GetTemplateByName = "usp_GetTemplateByName";
        public const string userRegEmailTemplate = "UserRegEmailTemplate";
        public const string userUpdateEmailTemplate = "UserUpdateEmailTemplate";
        public const string userInactivityEmailTemplate = "InActivityEmailTemplate";
        public const string userInactivitySmsTemplate = "InActivitySmsTemplate";
       

        public const string SparkApprovalEmailTemplate = "SparkApprovalEmailTemplate";

        // contact us email template
        public const string contactUsEmailTemplate = "ContactUsEmailTemplate";
        public const string ContactUsFeedBackEmailTemplate = "ContactUsFeedBackEmailTemplate";
        public const string contactUsFullName = "{fullName}";
        public const string contactUsEmail = "{email}";
        public const string contactUsSubject = "{subject}";
        public const string contactUsComments = "{comments}";

        // Email Subject
        public const string SparkApprovalEmailSubject = "Trigger Transformation | Spark Approval Request";

        //Spark Approval EmailTemplate
        public const string ManagerName = "{ManagerName}";
        public const string EmployeeName = "{EmployeeName}";
        public const string SparkBy = "{SparkBy}";
        public const string CurrentYear = "{CurrentYear}";

        public const string getPhoneNumberConfirmed = "SELECT phonenumberconfirmed FROM aspnetusers WHERE email = '{0}' ";

        //Dimension Matrix
        public const string addDimension = "Dimension added successfully.";
        public const string updateDimension = "Dimension updated successfully.";
        public const string dimensionExists = "Dimension already exists.";

        public const string addDimensionValues = "New Attribute added successfully.";
        public const string updateDimensionValues = "Attribute updated successfully.";
        public const string dimensionValueExists = "Attribute already exists.";
        public const string dimensionValueAssign = "Attribute cannot be deleted because It has been used.";
        public const string deleteDimensionElement = "Attribute deleted successfully.";
        public const string invalidDimensionElementId = "Attribute does not exists.";
        public const string DimensionElementAlreadyDeleted = "Attribute already deleted.";

        public const string getDimensionWiseValues = "GetValueByDimensionId";
        public const string addActionwisePermission = "Permissions configured successfully.";
        public const string validatePermission = "View permission is mandatory prior to edit permission.";

        public const string InvokeGetAllActions = "GetAllActions";
        public const string InvalidAssessmentDetails = "Invalid document details";

        public const string TeamSuccessMessage = "Team Configuration completed successfully.";
        public const string TeamUpdateSuccessMessage = "Team Configuration updated successfully.";
        public const string TeamIsInactiveMessage = "Team is inactivated successfully.";
        public const string TeamNameAlreadyExists = "Team already exists.";
        public const string TeamIsAlreadyInactivated = "Team is already inactivated.";

        public const string DefaultApiVersion = "1.0";
        public const string ApiVersionV2 = "2.0";
        public const string ApiVersionV2_1 = "2.1";

        //Team Notiifcation
        public const string GetCompanyDetailsForTeamNotification = "usp_GetCompanyDetailsForTeamNotification";
        public const string GetTeamDetailsForTeamNotification = "usp_GetTeamDetailsForTeamNotification";
        public const string GetTeamManagersEmailIds = "usp_GetTeamManagersEmailIds";
        public const string DeleteTeamConfiguration = "usp_DeleteTeamConfiguration";
        public const string GetTeamEmployeesDetailsByTeamId = "usp_GetTeamEmployeesDetailsByTeamId";
        public const string GetTeamManagersDetailsByTeamId = "usp_GetTeamManagersDetailsByTeamId";
        public const string AddTeamInActivityLog = "usp_AddTeamInActivityLog";
        public const string GetTeamInactivityEmployee = "usp_GetTeamInactivityEmployee";



        public const string InvalidDocumentDetails = "Invalid document details.";
        
        //Spark
        public const string AddEmployeeSpark = "Employee sparked successfully.";
        public const string UpdateEmployeeSpark = "Spark updated successfully.";
        public const string DeleteEmployeeSpark = "Spark deleted successfully.";
        public const string SparkApproved = "Employee spark approved.";
        public const string SparkRejected = "Employee spark rejected.";
        public const string UnauthorizedForSpark = "You are not authorized to edit or delete spark done by others.";
        public const string UnauthorizedForSparkSms = "You are not authorized to spark an employee.";
        public const string InvalidSparkDetails = "Invalid spark details";
        public const string SparkByUnknownUser = "SparkByUnknownUser";
        public const string SparkByUnauthorizedUser = "SparkByUnauthorizedUser";
        public const string UnauthorizedSparkEmail = "Unauthorized Spark via SMS";
        public const string InvalidSMSFormat = "Invalid SMS format.";
        public const string UnknownUser = "Your mobile number is not attached to a Trigger Manager profile. Please log on to Trigger and add/verify your mobile number.";
        public const string SparkRejectionMessage = "Your spark for an employee {0} has been rejected by {1} for following reason - {2}";
        public const string SparkForSelf = "User can not create a spark for himself.";
    }
}
