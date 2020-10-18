using HireMe.Core.Helpers;
using HireMe.Data.Repository.Interfaces;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Jobs;
using HireMe.ViewModels.Resume;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IRepository<Resume> _resumeRepository;
        private readonly IRepository<Jobs> _jobsRepository;
        private readonly IJobsService _jobsService;

        public ResumeService(
            IRepository<Resume> resumeRepository,
            IRepository<Jobs> jobsRepository,
            IJobsService jobsService)
        {
            _resumeRepository = resumeRepository;
            _jobsRepository = jobsRepository;
            _jobsService = jobsService;
        }

        public async Task<OperationResult> Create(string title, string fileid, User user)
        {
            if (await IsResumeExists(user.Id, title))
            {
                return OperationResult.FailureResult("Resume uploading failure contact with support !");
            }

            var resume = new Resume
            {
                Title = title,
                FileId = fileid,
                Date = DateTime.Now,
                UserId = user.Id
            };

                var ent = await _resumeRepository.AddAsync(resume);
            return ent;
        }

        public async Task<OperationResult> Delete(int id, string userId)
        {
            Resume entity = await _resumeRepository.GetByIdAsync(id);
            return await _resumeRepository.DeleteAsync(entity);
        }


        public IAsyncEnumerable<ResumeViewModel> GetAllBy(User user)
        {
            var entity = GetAllAsNoTrackingMapped()
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .AsQueryable()
                .AsAsyncEnumerable();


            return entity;
        }
        public IAsyncEnumerable<ResumeViewModel> GetAllReceived(User user)
        {
            var job = _jobsService.GetAllAsNoTrackingMapped()
                .Where(x => x.isApproved == 2 && (x.PosterID == user.Id))
                .AsQueryable()
                .FirstOrDefault();

            if (job != null)
            {
                string[] items = job.resumeFilesId?.Split(',');
                if (items == null)
                    return null;

                var entity = GetAllAsNoTrackingMapped()
                    .Where(x => ((IList)items).Contains(x.Id.ToString()))
                    .OrderByDescending(x => x.Id)
                    .AsQueryable()
                    .AsAsyncEnumerable();
                
                return entity;
            }
            return null;
        }

        public IQueryable<ResumeViewModel> GetAllByQueryable(User user)
        {
            var entity = GetAllAsNoTrackingMapped()
                .Where(x => x.UserId == user.Id)
                .AsQueryable();

            return entity;
        }
        public IQueryable<ResumeViewModel> GetAllAsNoTrackingMapped()
        {
            return _resumeRepository.Set().AsNoTracking().To<ResumeViewModel>();
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await _resumeRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }
        private async Task<bool> IsResumeExists(string userId, string title)
        {
            return await _resumeRepository.Set().AsNoTracking().AnyAsync(x => (x.UserId == userId) && (x.Title == title));
        }
    }
}