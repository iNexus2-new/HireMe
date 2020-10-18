using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HireMe.Data.Repository.Interfaces;
using HireMe.Services.Interfaces;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using HireMe.Entities.Input;
using HireMe.ViewModels.Jobs;
using HireMe.Data;
using HireMe.Entities.Enums;

namespace HireMe.Services
{

    public class JobsService : IJobsService
    {
        private readonly IRepository<Jobs> jobsRepository;
        private readonly ICompanyService _companyService;
        private readonly INotificationService _notifyService;

        public JobsService(
            IRepository<Jobs> jobsRepository,
            ICompanyService companyService,
            INotificationService notifyService,
            FeaturesDbContext _context)
        {
            this.jobsRepository = jobsRepository;
            _companyService = companyService;
            _notifyService = notifyService;


         //   this.SeedTest(_context);
        }
        public async Task<OperationResult> Create(CreateJobInputModel viewModel, User user)
        {
            Jobs jobs = new Jobs();
            jobs.Update(viewModel, 0, user);

            var entity = await jobsRepository.AddAsync(jobs);
            return entity;
        }

        public async Task<bool> AddResumeFile(int jobId, string resumeId)
        {
            Jobs entityId = await jobsRepository.GetByIdAsync(jobId);
            if (entityId == null)
            {
                return false;
            }
            string postIdComplate;
            
            postIdComplate = ',' + resumeId;

            entityId.resumeFilesId += postIdComplate;

            await jobsRepository.SaveChangesAsync();
            return true;
        }

        public async Task<OperationResult> Delete(int id)
        {
            Jobs entityId = await jobsRepository.GetByIdAsync(id);

            var entity = await jobsRepository.DeleteAsync(entityId);
            return entity;
        }

        public async Task<OperationResult> Update(CreateJobInputModel viewModel, User user)
        {
           Jobs existEntity = await jobsRepository.GetByIdAsync(viewModel.Id);

            existEntity.Update(viewModel, existEntity.isApproved, user);

            var entity = await jobsRepository.UpdateAsync(existEntity);
            return entity;
        }
        public async Task<OperationResult> DeleteAllByCompany(int companyId)
        {
            var valid = await _companyService.IsValid(companyId);
            if (valid)
            {
                return OperationResult.FailureResult("Invalid company id!");
            }
            
            var jobs = this.jobsRepository.Set()
              .Where(j => j.CompanyId == companyId)
              .ToList();

            if (jobs.Count > 0)
            {
                foreach (var item in jobs)
                {
                    await jobsRepository.DeleteAsync(item);
                }
            }
            return null;
           // var success = this.jobsRepository.SaveChanges() > 0;
            //return success ? OperationResult.SuccessResult() : OperationResult.FailureResult("Job post can't create, try again later or contact with support !");
        }
        public async ValueTask<int> GetAllCount()
        {
            var ent = await GetAllAsNoTracking().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IAsyncEnumerable<JobsViewModel> GetAllUnapproved()
        {
            var ent = GetAllAsNoTrackingMapped().Where(x => x.isApproved != 2).AsQueryable().ToAsyncEnumerable();
            return ent;
        }
        public async ValueTask<int> GetAllCountByCategory(int id)
        {
            var ent = await GetAllAsNoTracking().Where(x => x.CategoryId == id).AsQueryable().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public async ValueTask<int> GetAllCountByCompany(int id)
        {
            var ent = await GetAllAsNoTracking().Where(x => x.CompanyId == id).AsQueryable().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IQueryable<JobsViewModel> GetAllTrackingMapped()
        {
            return jobsRepository.Set().To<JobsViewModel>();
        }
        public IQueryable<JobsViewModel> GetAllAsNoTrackingMapped()
        {
            return jobsRepository.Set().AsNoTracking().To<JobsViewModel>(); 
        }
        public IQueryable<Jobs> GetAllAsNoTracking()
        {
            return jobsRepository.Set().AsNoTracking();
        }
        public IAsyncEnumerable<JobsViewModel> GetByCompanyOrCategory(int id, bool isCompany, int entitiesToShow)
        {
            return GetAllAsNoTrackingMapped()
                   .Where(x => x.isApproved == 2 && (isCompany ? x.CompanyId == id : x.CategoryId == id))
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<JobsViewModel> GetTop(int entitiesToShow)
        {
            var ent = GetAllAsNoTrackingMapped()
                   .Where(x => x.isApproved == 2);

            if (ent.Any(x => (x.Promotion > 0)))
            {
                ent.OrderByDescending(x => x.Promotion);
            }
            else if (ent.Any(x => x.Rating > 0))
            {
                ent.OrderByDescending(x => x.Rating);
            }
            else ent.OrderByDescending(x => x.Id);

            var data = ent.Take(entitiesToShow).AsQueryable().AsAsyncEnumerable();
            return data;
        }

        public IAsyncEnumerable<JobsViewModel> GetLast(int entitiesToShow)
        {
            return jobsRepository.Set()
                   .To<JobsViewModel>()
                   .Where(x => x.isApproved == 2)
                   .OrderByDescending(x => x.Id)
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await jobsRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }
        public T GetById<T>(int id)
        {
            var ent = jobsRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefault();

            return ent;
        }
        public async ValueTask<bool> IsValid(int Id)
        {
            var ent = await this.jobsRepository.Set().AnyAsync(x => x.Id == Id).ConfigureAwait(false);
            return ent;
        }
        
        public void SeedTest(FeaturesDbContext dbContext)        {            var test = new List<Jobs>();            for (int i = 0; i < 100000; i++)            {
                test = new List<Jobs>                {
                new Jobs
                {                //Id = i,                CategoryId = 1,                CompanyId = 1,                Name = "Test2",                //WorkType = Entities.Enums.WorkType.Full,                ExprienceLevels = Entities.Enums.ExprienceLevels.Beginner,                Description = "testt",                LocationId = 1,                Visiblity = Entities.Enums.Visiblity.Public,                LanguageId = null,                MinSalary = 11,                MaxSalary = 111,                SalaryType = Entities.Enums.SalaryType.day,                Adress = "test",                TagsId = null,                PosterID = "77701575-2fcc-4896-98ff-8f80a15efd35",                Date = DateTime.Now,                Promotion = Entities.Enums.PromotionEnum.Default,                isApproved = 2                }               };               dbContext.Jobs.AddRange(test);            }         dbContext.SaveChanges();        }        public int RandomNumber(int min, int max)        {            Random random = new Random();            return random.Next(min, max);        }

        public async ValueTask<bool> AddRatingToJobs(int jobId, int rating)
        {
            var job = await this.jobsRepository.Set().FirstOrDefaultAsync(j => j.Id == jobId).ConfigureAwait(false);

            if (job == null)
                return false;

            int maxRating = 5;

            if (rating < maxRating)
            {
                job.RatingVotes += rating;     
            }

                if (job.Rating < (double)maxRating)
                {
                    job.Rating += ((job.Rating * job.RatingVotes) + rating) / (job.RatingVotes + 1);

                    await jobsRepository.SaveChangesAsync();
                    return true;
                }

                return false;
        }

        // RESUME
        public async ValueTask RemoveResumeFromReceived(string id, User user)
        {
            string postIdComplate = ',' + id;

            var job = this.jobsRepository.Set()
                .Where(x => x.PosterID == user.Id)
                .AsQueryable()
                .FirstOrDefault();

            if (job != null)
            {
                string[] items = job.resumeFilesId?.Split(',');

                if (job.resumeFilesId?.IndexOf(id) > -1)
                {
                    job.resumeFilesId = job.resumeFilesId.Remove(job.resumeFilesId.Contains(',') ? job.resumeFilesId.IndexOf(postIdComplate) : job.resumeFilesId.IndexOf(id));

                    await jobsRepository.UpdateAsync(job);
                    await jobsRepository.SaveChangesAsync();
                    await _notifyService.Create("Your resume was seen...", $"jobs/details/{job.Id}", DateTime.Now, NotifyType.Information, user);

                }
            }
            
        }
    }
}
