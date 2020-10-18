namespace HireMe.ViewModels.Jobs
{
    using System.Collections.Generic;
    using System;
    using HireMe.Mapping.Interface;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.Entities.Input;
    using HireMe.ViewModels.Message;
    using System.Linq;

    public class JobsViewModel : IMapFrom<Jobs>
    {
        // Main
        public int Id { get; set; }
        public string Name { get; set; }
        public int isApproved { get; set; }
        public int LocationId { get; set; }
        public DateTime Date { get; set; }

        // Details
        public ExprienceLevels ExprienceLevels { get; set; }

        public JobTypeEnum JobType { get; set; }

        public Visiblity Visiblity { get; set; }
        public string Adress { get; set; }
        public string About { get; set; }
        public string Description { get; set; }
        public uint MinSalary { get; set; }
        public uint MaxSalary { get; set; }
        public SalaryType SalaryType { get; set; }

        public PromotionEnum Promotion { get; set; }
        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int VotedUsers { get; set; }
        public uint Views { get; set; }
        public string PosterID { get; set; }

        // Links

        public string resumeFilesId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public string LanguageId { get; set; }
        public string TagsId { get; set; }
        public string SearchString { get; set; }

        public virtual MessageViewModel Message { get; set; }

        public string WorkType { get; set; }

    }

  
}