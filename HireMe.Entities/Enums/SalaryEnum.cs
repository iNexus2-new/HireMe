using System.ComponentModel.DataAnnotations;

namespace HireMe.Entities.Enums
{
    public enum SalaryType
    {
        [Display(Name="час")]
        hour,
        [Display(Name = "ден")]
        day,
        [Display(Name = "месец")]
        month,
        [Display(Name = "година")]
        year
    }
}