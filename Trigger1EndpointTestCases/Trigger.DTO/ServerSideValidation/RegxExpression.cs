namespace Trigger.DTO.ServerSideValidation
{
    public static class RegxExpression
    {
        public const string AlphabaticWithSpace = @"^[a-zA-Z][a-zA-Z\s]*$";
        public const string DateFormat = "{0:MM/dd/yyyy}";

        public const string CompanyName = @"^[a-zA-Z0-9][A-Za-z0-9\-\s]*$";
        public const string Address = @"$|^[A-Za-z0-9\'\s]+";
        public const string CityStateCountry = @"^[a-zA-Z ]*$";
        public const string ZipCode = @"^[0-9\-]+$";
        public const string Website = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";

        public const string EmployeeName = @"^[a-zA-Z][a-zA-Z\'\s]*$";
        public const string EmployeeFName = @"^[a-zA-Z][a-zA-Z\'\-\s]*$";
        public const string EmployeeLName = @"^[a-zA-Z][a-zA-Z\'\-\s]*$";

        public const string EmployeeId = @"^[a-zA-Z0-9]+$";
        public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3}){1,2})$";
        public const string Position = @"^$|^[A-Za-z0-9\s]+";
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{10,15}$";
        public const string Salary = "^[0-9]{1,7}$";
        public const string PhoneNumber = @"^(\+\d{1,4}[ ])((?!0)\d{10})$";
        public const string PhoneNumberClient = @"^((?!0)\d{10})$";

        //For Dimensions & Values
        public const string DimensionRegularExpression = @"^[a-zA-Z][a-zA-Z\s-]*$";

        //For Team Configuration
        public const string TeamName = @"$|^[A-Za-z0-9\'\s]+";


    }
}
