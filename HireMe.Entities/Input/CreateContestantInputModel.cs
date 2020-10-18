namespace HireMe.Entities.Input
{
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateContestantInputModel : BaseModel
    {
        // Main
        [Required(ErrorMessage = "Моля напишете трите си имена.")]
        [StringLength(50, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 5)]
        [Display(Name = "Пълно име")]
        public string Name { get; set; }

        [Display(Name = "Пол")]
        public Gender Genders { get; set; }

        [Display(Name = "Одобрен")]
        public bool isApproved { get; set; }

        [Display(Name = "Населено място")]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Моля въведи рождената си дата.")]
        [Display(Name = "Рождена дата")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? Age { get; set; }

        // Details
        [Required(ErrorMessage = "Моля, напишете специалността си.")]
        [StringLength(100, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 20)]
        [Display(Name = "Специалност:")]
        public string About { get; set; }

        [Required(ErrorMessage = "Моля, опишете себе си в описанието.")]
        [StringLength(100000, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 100)]
        [Display(Name = "Описание:")]
        public string Description { get; set; }

        [Range(0, 100)]
        [Display(Name = "Опит")]
        public int Experience { get; set; }

        [Range(0, 9999)]
        [Display(Name = "Заплащане")]
        public int payRate { get; set; }

        [Display(Name = "Вид заплащане")]
        public SalaryType SalaryType { get; set; }

        [Display(Name = "Видимост на профила")]
        public int profileVisiblity { get; set; }

        [Display(Name = "Тип работа")]
        public string WorkType { get; set; }


        // Web presence

        [Display(Name = "Страница")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]
        public string WebsiteUrl { get; set; }

        [Display(Name = "Блог")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string PortfolioUrl { get; set; }

        [Display(Name = "Linkedin")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string Linkedin { get; set; }

        [Display(Name = "Facebook")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string Facebook { get; set; }

        [Display(Name = "Twitter")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string Twitter { get; set; }


        [Display(Name = "Github")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string Github { get; set; }


        [Display(Name = "Dribbble")]
        [StringLength(20, ErrorMessage = "{0} трябва да е между {2} и {1} символи.", MinimumLength = 3)]

        public string Dribbble { get; set; }

        public PromotionEnum Promotion { get; set; }

        // Resume
        [Display(Name = "Резюме")]
        public string ResumeFileId { get; set; }

        // Links
        // [ValidSkillsId]
        [Display(Name = "Умения")]
        public string userSkillsId { get; set; }

        [Display(Name = "Езици")]
        public string LanguagesId { get; set; }

        //  [ValidCategoryId]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Резюме")]
        public IFormFile FormFile { get; set; }

        public string PosterID { get; set; }

    }


}
