namespace HireMe.Entities.Models
{
    using Ardalis.GuardClauses;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Jobs : BaseModel
    {
        public string Name { get; set; }
        public int LocationId { get; set; }
        public string WorkType { get; set; }
        public ExprienceLevels ExprienceLevels { get; set; }
        public JobTypeEnum JobType { get; set; }
        public Visiblity Visiblity { get; set; }
        public string Adress { get; set; }
        public string Description { get; set; }

        public uint MinSalary { get; set; }
        public uint MaxSalary { get; set; }
        public SalaryType SalaryType { get; set; }

        public PromotionEnum Promotion { get; set; }

        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int VotedUsers { get; set; }
        public uint Views { get; set; }

        public string resumeFilesId { get; set; } = "0";
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public string LanguageId { get; set; }
        public string TagsId { get; set; }

        public string PosterID { get; set; }
        public DateTime Date { get; set; }
        public int isApproved { get; set; }

        public void Update(CreateJobInputModel viewModel, int approved, User user)
        {
            Id = viewModel.Id;

            Guard.Against.NullOrEmpty(viewModel.Name, nameof(viewModel.Name));
            Name = viewModel.Name;

            Guard.Against.NullOrEmpty(viewModel.Description, nameof(viewModel.Description));
            Description = viewModel.Description;

            Guard.Against.NullOrEmpty(viewModel.Adress, nameof(viewModel.Adress));
            Adress = viewModel.Adress;

            WorkType = viewModel.WorkType;
            ExprienceLevels = viewModel.ExprienceLevels;
            Visiblity = viewModel.Visiblity;
            LocationId = viewModel.LocationId;

            MinSalary = viewModel.MinSalary;
            MaxSalary = viewModel.MaxSalary;
            SalaryType = viewModel.SalaryType;

            CompanyId = viewModel.CompanyId;
            CategoryId = viewModel.CategoryId;
            LanguageId = viewModel.LanguageId;
            TagsId = viewModel.TagsId;

            PosterID = user.Id;
            Date = DateTime.Now;
            isApproved = approved;
        }
    }
   
}