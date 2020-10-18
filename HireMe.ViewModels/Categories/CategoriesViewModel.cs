namespace HireMe.ViewModels.Categories
{
    using AutoMapper;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Interface;
    using System.Collections.Generic;
    using System.Linq;


    public class CategoriesViewModel : IMapFrom<Category>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Title_BG { get; set; }

        public string Icon { get; set; }

        public IAsyncEnumerable<CategoriesViewModel> List { get; set; }

        public string TitleAndCount => $"{this.Title_BG} ({this.CountOfAllJobs})";

        public int CountOfAllJobs { get; set; }


        public string TitleAndCountContestant => $"{this.Title_BG} ({this.CountOfAllContestants})";

        public int CountOfAllContestants { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Category, CategoriesViewModel>()
             .ForMember(x => x.CountOfAllJobs, opt => opt.MapFrom(x => x.Jobs.Count()))
             .ForMember(x => x.CountOfAllContestants, opt => opt.MapFrom(x => x.Contestants.Count()));


        }
    }
}