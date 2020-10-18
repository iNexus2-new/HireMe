using System;
using System.ComponentModel.DataAnnotations;

namespace HireMe.Core.Filters
{
    public class ValidValuesFromEnumAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidValuesFromEnumAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!Enum.IsDefined(_enumType, (int)value))
                {
                    return new ValidationResult($"Invalid {_enumType.Name} value.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
