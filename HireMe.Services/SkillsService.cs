using HireMe.Core.Helpers;
using HireMe.Data.Repository.Interfaces;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Contestants;
using HireMe.ViewModels.Jobs;
using HireMe.ViewModels.Skills;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Services
{
    public class SkillsService : ISkillsService
    {
        private readonly IRepository<Skills> skillsRepository;
        private readonly IContestantsService _contestantService;
        private readonly IJobsService _jobsService;

        public SkillsService(
            IRepository<Skills> skillsRepository,
            IContestantsService contestantService,
            IJobsService jobsService)
            
        {
            this.skillsRepository = skillsRepository;
            this._contestantService = contestantService;
            this._jobsService = jobsService;
        }

        public async Task<OperationResult> Create(int id, string title)
        {
            var Skills = new Skills
            {
                Id = id,
                Title = title
            };

            return await this.skillsRepository.AddAsync(Skills);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Skills entity = await skillsRepository.GetByIdAsync(id);
            return await skillsRepository.DeleteAsync(entity);
        }


        public IAsyncEnumerable<SkillsViewModel> GetAll()
        {
            var ent = GetAllAsNoTrackingMapped().ToAsyncEnumerable();
            return ent;
        }
        public IQueryable<SkillsViewModel> GetAllAsNoTrackingMapped()
        {
            return skillsRepository.Set().AsNoTracking().To<SkillsViewModel>();
        }
        public IAsyncEnumerable<SkillsViewModel> GetAllByContestant(int id)
        {
            var contestant = _contestantService.GetById<ContestantViewModel>(id);
            if (contestant == null)
            {
                return null;
            }

            string[] words = contestant.userSkillsId?.Split(',');
            if (words == null)
                return null;

            var entity = GetAllAsNoTrackingMapped()
                .Where(x => ((IList)words).Contains(x.Id.ToString()))
                .AsQueryable()
                .AsAsyncEnumerable();

            return entity;
        }
        public IAsyncEnumerable<SkillsViewModel> GetAllByJob(int id)
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