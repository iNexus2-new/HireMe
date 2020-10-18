namespace HireMe.ViewModels.Company
{
    using AutoMapper;
    using System.Linq;
    using HireMe.Mapping.Interface;
    using HireMe.Entities.Models;
    using HireMe.Entities.Enums;
    using System;
    using HireMe.ViewModels.Message;
    using Microsoft.AspNetCore.Http;

    public class CompanyViewModel : IMapFrom<Company>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }

        public bool isAuthentic_EIK { get; set; }

        public string About { get; set; }

        public bool Private { get; set; }

        public string Logo { get; set; }

        public int LocationId { get; set; }

        public string Adress { get; set; }

        public string PhoneNumber { get; set; }

        public string Website { get; set; }

        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public int VotedUsers { get; set; }

        public string CompanyTags { get; set; }

        public string PosterId { get; set; }

        public string Admin1_Id { get; set; }

        public string Admin2_Id { get; set; }

        public string Admin3_Id { get; set; }

        public int isApproved { get; set; }

        public PromotionEnum Promotion { get; set; }

        public DateTime Date { get; set; }

        public virtual MessageViewModel Message { get; set; }

        public string TitleAndCount => $"{this.Title} ({this.CountOfAllJobs})";

        public int CountOfAllJobs { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Company, CompanyViewModel>().ForMember(
                x => x.CountOfAllJobs,
                opt => opt.MapFrom(x => x.Jobs.Where(x => x.isApproved == 2).Count()));
        }

    }
}