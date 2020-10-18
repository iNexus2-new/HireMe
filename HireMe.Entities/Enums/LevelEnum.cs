using System.ComponentModel.DataAnnotations;

namespace HireMe.Entities.Enums
{
    public enum Level
    {
        [Display(Name = "Служители/Работници")]
        Employee,
        [Display(Name = "Мениджмънт")]
        Management,
        [Display(Name = "Експерти/Специалисти")]
        Specialist
    }
}