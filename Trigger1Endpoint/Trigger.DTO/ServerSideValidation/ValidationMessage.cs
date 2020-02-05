namespace Trigger.DTO.ServerSideValidation
{
    public static class ValidationMessage
    {
        // Department
        public const string DepartmentNameRequired = "Please enter Department Name.";
        public const string DepartmentNameMaxLength = "Department Name should be maximum 50 characters long.";
        public const string DepartmentNameValid = "Please enter valid Department Name."; 
        public const string CompanyRequired = "Please select Company."; 

        // Comapny
            
        public const string CompanyNameValid = "Please enter valid Company Name.";  
        public const string CompanyNameRequired = "Please enter Company Name.";
        public const string CompanyNameMaxLength = "Company Name should be maximum 100 characters long.";
        public const string CompanyTypeRequired = "Please select Company Type.";
        public const string Address1Required = "Please enter Address1.";
        public const string Address1Valid = "Please enter valid Address1."; 
        public const string Address1MaxLength = "Address1 should be maximum 100 characters long.";
        public const string Address2MaxLength = "Address2 should be maximum 100 characters long.";
        public const string Address2Valid = "Please enter valid Address2.";
        public const string CityRequired = "Please enter City.";
        public const string CityMaxLength = "City should be maximum 25 characters long.";
        public const string EmployeeCityMaxLength = "City should be maximum 30 characters long.";
        public const string CityNameValid = "Please enter valid City Name."; 
        public const string StateRequired = "Please enter State.";
        public const string StateMaxLength = "State should be maximum 25 characters long.";
        public const string StateNameValid = "Please enter valid State Name."; 
        public const string ZipCodeRequired = "Please enter Zipcode.";
        public const string ZipCodeValid = "Please enter valid Zipcode.";
        public const string ZipCodeMaxLength = "Zipcode should be maximum 15 characters long.";
        public const string ZipCodeMinLength = "Zipcode should be minimum 5 characters require."; 
        public const string CountryRequired = "Please enter Country.";
        public const string CountryValid = "Please enter valid Country."; 
        public const string CountryMaxLength = "Country should be maximum 30 characters long.";
        public const string WebsiteValid = "Please enter valid Website."; 
        public const string WebsiteMaxLength = "Website should be maximum 50 character long."; 
        public const string PhoneRequired = "Please enter Phone Number.";
        public const string PhoneMaxLength = "Phone Number should be atleast 10 digits.";
        public const string PhoneNumberValid = "Please enter valid Phone Number.";
        public const string ContractStartDateRequired = "Please select Contract Start Date.";
        public const string ContractStartDateNotPastDate = "Start Date should be greater than or equal Current Date."; 
        public const string ContractEndDateRequired = " Please select Contract End Date.";
        public const string ContractStartDateLessThanEndDate = "Start Date should be less than End Date.";
        public const string GracePeriodRequired = "Please enter Grace Period.";
        public const string GracePeriodMaxValue = "Grace Period should not be more than 365.";
        public const string RemarkMaxLength = "Remark should be maximum 300 character long."; 
        public const string DealsDetailsMaxLength = "Deals Details should be maximum 300 character long."; 
        public const string CostPerEmpMaxValue = "Cost Per Employee should not be more than 99999."; 
        public const string FixedAmtPerMonMaxValue = "Fixed Amount Per Month should not be more than 99999."; 
        public const string AssessmentDaysMaxValue = "Assessment Days should not be more than 99999 days."; 

        //Employee
        public const string EmployeeIdValid = "Please enter valid Employee Id.";
        public const string FirstNameRequired = "Please enter First Name.";
        public const string FirstNameValid = "Please enter valid First Name."; 
        public const string FirstNameMaxLength = "First Name should be maximum 25 characters long.";
        public const string LastNameRequired = "Please enter Last Name.";
        public const string LastNamevalid = "Please enter valid Last Name."; 
        public const string LastNameMaxLength = "Last Name should be maximum 25 characters long.";
        public const string MiddleNameMaxLength = "Middle Name should be maximum 25 characters long."; 
        public const string MiddleNameValid = "Please enter valid Middle Name.";
        public const string EmailRequired = "Please enter Email Address.";
        public const string EmailValid = "Please enter valid Email Address.";
        public const string EmailMaxLength = "Email Address should be maximum 60 characters long."; 
        public const string PositionRequired = "Please enter Employee Position.";
        public const string PositionMaxLenght = "Employee Position should be maximum 50 characters long.";
        public const string PositionValid = " Please enter valid Employee Position."; 
        public const string DateOfHireRequired = "Please select Date Of Hire.";
        public const string DateOfHireValidation= "Date Of Hire should not be greater than Current Date.";
        public const string BirthDateHireDateValidation = "Date Of Hire should be greater than Date Of Birth.";
        public const string BirthDateIncDateValidation = "Date Of Last Salary Increase should be greater than Date Of Birth.";
        public const string BirthDatePromDateValidation = "Date In Position should be greater than Date Of Birth.";
        public const string HireDateIncDateValidation = "Date Of Last Salary Increase should be greater than Date Of Hire."; 
        public const string HireDatePromDateValidation = "Date In Position should be greater than Date Of Hire.";       
        public const string IncDatePromDateValidation = "Date Of Last Salary Increase should be greater than or equal to Date In Position."; 
        public const string DepartmentRequired = "Please select Department Name.";
        public const string RoleRequired = "Please select Trigger Role.";
        public const string ManagerRequired = "Please select Manager.";
        public const string EmployeeStateRequired = "Please select Employee Status."; 
        public const string JobcategoryMaxLength = "Job Category should be maximum 25 characters long."; 
        public const string JobGroupMaxLength = "Job Group should be maximum 25 characters long.";
        public const string JobCodeMaxLength = "Job Code should be maximum 25 characters long.";
        public const string CurrentSalaryMaxValue = "Current Salary should not be more than 9999999 or not allow decimal value.";
        public const string LocationNameMaxLength = "Location Name should be maximum 25 characters long.";
        public const string LocationNameValid = "Please enter valid Location Name.";
        public const string RegionRequired = "Please select Region.";
       
        //change Password
        public const string CurrentPasswordRequired = "Please enter your Current Password.";
        public const string NewPasswordRequired = "Please enter New Password.";
        public const string NewPasswordValid = "Password must contain uppercase, lowercase, number and special character.";
        public const string PasswordMinLength = "Password must be at least 10 characters.";
        public const string PasswordMaxLength = "Password should be maximum 15 characters long.";
        public const string UserIdRequire = "User Id not found.";

        //Edit Profile
        public const string EmpIdRequired = "Please enter emp id.";
        public const string EmployeeIdRequired = "Please enter employee id.";

        //Dimension Matrix
        public const string DimensionTypeRequired = "Please enter Dimension Type.";
        public const string DimensionIdRequired = "Please enter Dimension Id.";
        public const string DimensionTypeMaxLength = "Dimension Type should be maximum 50 characters long.";
        public const string DimensionValueRequired = "Please enter Dimension Value.";
        public const string DimensionValueIdRequired = "Please enter Dimension Value Id.";
        public const string DimensionValueMaxLength = "Dimension Value should be maximum 50 characters long.";
        public const string DimensionwiseActionRequired = "Please select Action.";
        public const string DimensionwiseActionIdRequired = "Please enter Action Id";
        public const string DimensionElementsRegExpression = "Special characters & numbers not allowed except '-'.";

        //Contact Us
        public const string SubjectRequired = "Please enter Subject.";
        public const string CommentsRequired = "Please enter Comments.";
        public const string FullNameRequired = "Please enter Full Name.";

        //Assessment Attachment
        public const string DocumentNameRequired = "Attached document name should not be blank.";
        public const string CloudFilePathRequired = "You can either upload a document or enter a cloud file url.";
        public const string CurrentDateRequired = "Assessment can be initiate for a current date only.";

        //Team Configuration
        public const string TeamNameRequired = "Team Name should not be blank.";
        public const string TeamStartDateRequired = "Please select Team Start Date.";
        public const string TeamEndDateRequired = "Please select Team End Date.";
        public const string TriggerActivityDays = "Please enter trigger activity days.";
        public const string InValidTriggerActivityDays = "Invalid trigger activity days.";

        public const string TeamStartDateLessThanEndDate = "Start Date should be less than End Date.";
        public const string TeamStartDateNotPastDate = "Start Date should be greater than or equal Current Date.";
    }
}
