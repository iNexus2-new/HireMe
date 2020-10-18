namespace HireMe.Entities.Input
{
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CreateJobInputModel : BaseModel
    {
        [Required(ErrorMessage = "Please enter your job post title.")]
        [Display(Name = "Title:")]
        [StringLength(50, ErrorMessage = "{0} The title must be between {2} and {1} characters.", MinimumLength = 10)]
        public string Name { get; set; }

        [Display(Name = "Location:")]
        public int LocationId { get; set; }

        [Display(Name = "Exprience levels:")]
        public ExprienceLevels ExprienceLevels { get; set; }

        public JobTypeEnum JobType { get; set; }

        [Display(Name = "Visiblity:")]
        public Visiblity Visiblity { get; set; }


        [Required(ErrorMessage = "Please enter job adress.")]
        [StringLength(40, ErrorMessage = "{0} The adress must be between {2} and {1} characters.", MinimumLength = 5)]
        [Display(Name = "Adress")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Please enter something about you in description.")]
        [StringLength(100000, ErrorMessage = "{0} The description must be between {2} and {1} characters.", MinimumLength = 100)]
        [Display(Name = "About")]
        public string Description { get; set; }

        [Display(Name = "Minimum Salary:")]
        public uint MinSalary { get; set; } = 0;

        [Display(Name = "Maximum Salary:")]
        public uint MaxSalary { get; set; } = 0;

        public SalaryType SalaryType { get; set; }

        public PromotionEnum Promotion { get; set; }

        // [ValidCategoryId]
        [Display(Name = "Select Category")]
        public int CategoryId { get; set; }

        //  [ValidCompanyId]
        [Display(Name = "Select Company")]
        public int CompanyId { get; set; }

        public string CompanyLogo { get; set; }

        [Display(Name = "Select Languages")]
        public string LanguageId { get; set; }

        [Display(Name = "Select Tags")]
        public string TagsId { get; set; }

        public string PosterId { get; set; }

        public bool isApproved { get; set; }

        [Display(Name = "Work Type:")]
        public string WorkType { get; set; }
    }
}
