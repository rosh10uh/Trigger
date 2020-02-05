using System;
using System.ComponentModel.DataAnnotations;

namespace Trigger.DTO.ServerSideValidation
{
    /// <summary>
    /// Compare two date based on condition 
    /// </summary>
    /// <param name="compareing field"></param>
    /// <param name="condition"></param>
    /// <param name="validation message"></param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateCompareValidationAttribute : ValidationAttribute
    {
        private string _dateToCompareToFieldName { get; set; }
        private enumOprator _condition { get; set; }
        private string _validationMessage { get; set; }

        public DateCompareValidationAttribute(string dateToCompareToFieldName, enumOprator condition, string validationMessage)
        {
            _dateToCompareToFieldName = dateToCompareToFieldName;
            _condition = condition;
            _validationMessage = validationMessage;
        }

        /// <summary>
        /// Compare dates & check validations
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime currentDateField = (DateTime)value;
            DateTime comparedDateField = (DateTime)validationContext.ObjectType.GetProperty(_dateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);
            if (comparedDateField == DateTime.MinValue
                || currentDateField == DateTime.MinValue
                || comparedDateField <= Convert.ToDateTime("01-01-1900 12:00:00")
                || currentDateField <= Convert.ToDateTime("01-01-1900 12:00:00"))
                return ValidationResult.Success;

            Boolean isSuccess = false;
            switch (_condition)
            {
                case enumOprator.lessThan:
                    if (currentDateField < comparedDateField)
                        isSuccess = true;
                    break;
                case enumOprator.greaterThan:
                    if (currentDateField > comparedDateField)
                        isSuccess = true;
                    break;
                case enumOprator.lessThanOrEqual:
                    if (currentDateField <= comparedDateField)
                        isSuccess = true;
                    break;
                case enumOprator.greaterThanOrEqual:
                    if (currentDateField >= comparedDateField)
                        isSuccess = true;
                    break;
            }

            if (isSuccess)
                return ValidationResult.Success;
            else
                return new ValidationResult(_validationMessage);
        }
    }

    /// <summary>
    /// Compare date on current date based on condition 
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="primary field"></param>
    /// <param name="validation message"></param>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateValidationOnCurentDateAttribute : ValidationAttribute
    {
        private enumOprator _condition { get; set; }
        private string _primaryField { get; set; }
        private string _validationMessage { get; set; }

        public DateValidationOnCurentDateAttribute(enumOprator condition, string primaryField, string validationMessage)
        {
            _condition = condition;
            _primaryField = primaryField;
            _validationMessage = validationMessage;
        }

        /// <summary>
        /// common method to validate date against current date
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime currentDateField = (DateTime)value;

            int primaryFieldValue = (int)validationContext.ObjectType.GetProperty(_primaryField).GetValue(validationContext.ObjectInstance, null);

            if (primaryFieldValue != 0)
                return ValidationResult.Success;

            Boolean isSuccess = false;
            switch (_condition)
            {
                case enumOprator.lessThan:
                    if (currentDateField < DateTime.Now.Date)
                        isSuccess = true;
                    break;
                case enumOprator.greaterThan:
                    if (currentDateField > DateTime.Now.Date)
                        isSuccess = true;
                    break;
                case enumOprator.lessThanOrEqual:
                    if (currentDateField <= DateTime.Now.Date)
                        isSuccess = true;
                    break;
                case enumOprator.greaterThanOrEqual:
                    if (currentDateField >= DateTime.Now.Date)
                        isSuccess = true;
                    break;
            }
            if (isSuccess)
                return ValidationResult.Success;
            else
                return new ValidationResult(_validationMessage);
        }
    }

    /// <summary>
    /// Check region for canada and us country 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegionValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int region = (int)value;

            int contryId = (int)validationContext.ObjectType.GetProperty(ValidationResource.countryId).GetValue(validationContext.ObjectInstance, null);

            if ((contryId == ValidationResource.US || contryId == ValidationResource.Canada) && region == 0)
            {
                return new ValidationResult(ValidationMessage.RegionRequired);
            }
            else
                return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Require date
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredDateAttribute : ValidationAttribute
    {
        public RequiredDateAttribute(string validationMessage)
        {
            _validationMessage = validationMessage;
        }
        private string _validationMessage { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateValue = (DateTime)value;
            if (dateValue == DateTime.MinValue)
            {
                return new ValidationResult(_validationMessage);
            }
            else
                return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Check validation for date as current UTC date
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredCurrentDateAttribute : ValidationAttribute
    {
        public RequiredCurrentDateAttribute(string validationMessage)
        {
            _validationMessage = validationMessage;
        }
        private string _validationMessage { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateValue = ((DateTime)value).ToUniversalTime();
            if (dateValue.Date != DateTime.UtcNow.Date)
            {
                return new ValidationResult(_validationMessage);
            }
            else
                return ValidationResult.Success;
        }
    }


    /// <summary>
    /// Require Int
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIntAttribute : ValidationAttribute
    {
        public RequiredIntAttribute(string validationMessage)
        {
            _validationMessage = validationMessage;
        }
        private string _validationMessage { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int intValue = (int)value;

            if (intValue == 0)
            {
                return new ValidationResult(_validationMessage);
            }
            else
                return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Allow Only Positive numbers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowPositiveNumbersAttribute : ValidationAttribute
    {
        public AllowPositiveNumbersAttribute(string validationMessage)
        {
            _validationMessage = validationMessage;
        }
        private string _validationMessage { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int intValue = (int)value;

            if (intValue == 0)
            {
                return new ValidationResult(_validationMessage);
            }
            else if(intValue < 0)
            {
                return new ValidationResult(_validationMessage);
            }
                return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Check for document name & content if content is available & name is blank then throw validation message
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredDocumentNameAttribute : ValidationAttribute
    {
        public RequiredDocumentNameAttribute(string validationMessage, string fieldName)
        {
            _validationMessage = validationMessage;
            _fieldName = fieldName;
        }

        private string _validationMessage { get; set; }
        private string _fieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            string documentName = (string)value;
            string documentContent = Convert.ToString(validationContext.ObjectType.GetProperty(_fieldName).GetValue(validationContext.ObjectInstance, null));

            if (documentName.Length == 0 && documentContent.Length > 0)
            {
                return new ValidationResult(_validationMessage);
            }
            else
                return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Check for document name & cloud file path if both are available then throw validation message
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredCloudFilePathAttribute : ValidationAttribute
    {
        public RequiredCloudFilePathAttribute(string validationMessage, string fieldName)
        {
            _validationMessage = validationMessage;
            _fieldName = fieldName;
        }

        private string _validationMessage { get; set; }
        private string _fieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            string cloudFilePath = (string)value;
            string documentName = Convert.ToString(validationContext.ObjectType.GetProperty(_fieldName).GetValue(validationContext.ObjectInstance, null));

            if (documentName.Length > 0 && cloudFilePath.Length > 0)
            {
                return new ValidationResult(_validationMessage);
            }
            else
                return ValidationResult.Success;
        }
    }

    public enum enumOprator
    {
        lessThan,
        greaterThan,
        lessThanOrEqual,
        greaterThanOrEqual
    }
}
