namespace HireMe.ViewModels.Contestants
{
    using AutoMapper;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Interface;
    using HireMe.ViewModels.Categories;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;

    public class ContestantViewModel : IMapFrom<Contestant>
    {
        public int Id { get; set; }

        // Main
        public string Name { get; set; }
        public Gender Genders { get; set; }
        public DateTime Age { get; set; }
        public int LocationId { get; set; }

        public DateTime Date { get; set; }
        public int isApproved { get; set; }
        // Details
        public string About { get; set; }
        public string Description { get; set; }
        public int Experience { get; set; }
        public int payRate { get; set; }
        public SalaryType SalaryType { get; set; }
        public int profileVisiblity { get; set; }
        public string WorkType { get; set; }

        public uint profileViews { get; set; }
        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int VotedUsers { get; set; }

        public uint Views { get; set; }

        public string ResumeFileId { get; set; }

        // Web presence
        public string WebsiteUrl { get; set; }
        public string PortfolioUrl { get; set; }
        public string Linkedin { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Github { get; set; }
        public string Dribbble { get; set; }

        public PromotionEnum Promotion { get; set; }

        public IFormFile FormFile { get; set; }

        // Links
        public string name { get; set; }
        public string userSkillsId { get; set; }
        public string LanguagesId { get; set; }
        public int CategoryId { get; set; }
        public string PosterID { get; set; }

        public virtual CreateMessageInputModel MessageInputModel { get; set; }

    }

}