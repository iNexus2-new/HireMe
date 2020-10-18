namespace HireMe.Services.Validations
{
    using System.ComponentModel.DataAnnotations;
    using HireMe.Services.Interfaces;

    public class ValidCompanyIdAttribute : ValidationAttribute
    {
        /*protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = (ICompanyService)validationContext
                .GetService(typeof(ICompanyService));

            if (service.IsValid((int)value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Invalid company id!");
            }
        }*/
    }
}