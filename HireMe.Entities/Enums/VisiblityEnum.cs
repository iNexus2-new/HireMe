using System.ComponentModel.DataAnnotations;

namespace HireMe.Entities.Enums
{
    public enum Visiblity : int
    {
        [Display(Name = "Видимо за всички")]
        Public = 0,

        [Display(Name = "Частно")]
        Private = 1
    }
}