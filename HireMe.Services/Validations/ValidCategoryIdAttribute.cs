namespace HireMe.Services.Validations
{
    using System.ComponentModel.DataAnnotations;
    using HireMe.Services.Interfaces;

    public class ValidCategoryIdAttribute : ValidationAttribute
    {
        /*protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = (ICategoriesService)validationContext
                .GetService(typeof(ICategoriesService));

            if (service.IsValid((int)value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Invalid category id!");
            }
        }*/
    }
}