namespace HireMe.Services
{
    using HireMe.Data.Repository.Interfaces;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Utility;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Contestants;
    using HireMe.ViewModels.Jobs;
    using HireMe.ViewModels.Language;
    using Microsoft.EntityFrameworkCore;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language> LanguageRepository;
        private readonly IContestantsService _contestantService;
        private readonly IJobsService _jobsService;
        public LanguageService(
            IRepository<Language> LanguageRepository,
            IContestantsService contestantService,
            IJobsService jobsService)
        {
            this.LanguageRepository = LanguageRepository;
            this._contestantService = contestantService;
            this._jobsService = jobsService;
        }

        public IAsyncEnumerable<LanguageViewModel> GetAll()
        {
            var ent = GetAllAsNoTrackingMapped().ToAsyncEnumerable();
            return ent;
        }
        public IQueryable<LanguageViewModel> GetAllAsNoTrackingMapped()
        {
            return LanguageRepository.Set().AsNoTracking().To<LanguageViewModel>();
        }
        public IAsyncEnumerable<LanguageViewModel> GetAllByContestant(int id)
        {
            var contestant = _contestantService.GetById<ContestantViewModel>(id);
            if (contestant == null)
                return null;
            
            string[] words = contestant.userSkillsId?.Split(',');
            if (words == null)
                return null;
            
            var entity = GetAllAsNoTrackingMapped()
                .Where(x => ((IList)words).Contains(x.Id.ToString()))
                .AsQueryable()
                .AsAsyncEnumerable();

            return entity;
        }
        public IAsyncEnumerable<LanguageViewModel> GetAllByJob(int id)
        {
            var job = _jobsService.GetById<JobsViewModel>(id);
            if (job == null)
            {
                return null;
            }

            string[] words = job.TagsId?.Split(',');
            if (words == null)
                return null;

            var entity = GetAllAsNoTrackingMapped()
                .Where(x => ((IList)words).Contains(x.Id.ToString()))
                .AsQueryable()
                .AsAsyncEnumerable();

            return entity;
        }
    }
}