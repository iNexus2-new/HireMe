namespace HireMe.Entities.Input
{
    using HireMe.Entities.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateCompanyInputModel : BaseModel
    {
        [Required(ErrorMessage = "Моля въведете име на фирма.")]
        [Display(Name = "Име на фирма:")]
        [StringLength(50, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 10)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Моля въведете емайл адрес.")]
        [Display(Name = "Емайл:")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Моля, въведете кратко описание за вашата компания.")]
        [StringLength(300, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 10)]
        [Display(Name = "Описание")]
        public string About { get; set; }

        [Display(Name = "Частна компания")]
        public bool Private { get; set; }

        [Display(Name = "Снимка")]
        public string Logo { get; set; }

        [Display(Name = "Местоположение")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Моля въведете точен адрес на фирмата.")]
        [StringLength(60, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 5)]
        [Display(Name = "Адрес")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Моля въведете телефонен номер за връзка.")]
        [Display(Name = "Телефонен номер:")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Url]
        [Display(Name = "Уеб сайт:")]
        public string Website { get; set; }

        public string PosterId { get; set; }

        [Display(Name = "ЕИК номер:")]
        public bool isAuthentic_EIK { get; set; }

        [Display(Name = "Работодател:")]
        public string Admin1_Id { get; set; }

        [Display(Name = "Работодател:")]
        public string Admin2_Id { get; set; }

        [Display(Name = "Работодател:")]
        public string Admin3_Id { get; set; }

        public PromotionEnum Promotion { get; set; }

        public DateTime Date { get; set; }
    }
}
