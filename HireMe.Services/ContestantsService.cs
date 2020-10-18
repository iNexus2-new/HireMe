using System.Threading.Tasks;
using HireMe.Data.Repository.Interfaces;
using HireMe.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Core.Helpers;
using HireMe.Entities.Input;
using HireMe.ViewModels.Contestants;
using Microsoft.EntityFrameworkCore;
using HireMe.Data;

namespace HireMe.Services
{
    public class ContestantsService : IContestantsService
    {
        private readonly IRepository<Contestant> contestantRepository; 
       
        public ContestantsService(IRepository<Contestant> contestantRepository, FeaturesDbContext _context)
        {
            this.contestantRepository = contestantRepository;
            //this.SeedTest(_context);
        }

        public async Task<OperationResult> Create(CreateContestantInputModel viewModel, User user)
        {
            var contestant = new Contestant();
            contestant.Update(viewModel, 0, user);

            var entity = await this.contestantRepository.AddAsync(contestant);
            return entity;
        }

        public async Task<OperationResult> Delete(int id)
        {
            Contestant entityId = await contestantRepository.GetByIdAsync(id);
            
            var entity = await contestantRepository.DeleteAsync(entityId);
            return entity;
        }

        public async Task<OperationResult> Update(CreateContestantInputModel viewModel, User user)
        {
            Contestant existEntity = await contestantRepository.GetByIdAsync(viewModel.Id);

            existEntity.Update(viewModel, existEntity.isApproved, user);

            var entity = await this.contestantRepository.UpdateAsync(existEntity);
            return entity;
        }
        public async ValueTask<int> GetAllCount()
        {
            var ent = await GetAllAsNoTracking().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IQueryable<ContestantViewModel> GetAllAsNoTrackingMapped()
        {
            return contestantRepository.Set().AsNoTracking().To<ContestantViewModel>();
        }
        public IQueryable<Contestant> GetAllAsNoTracking()
        {
            return contestantRepository.Set().AsNoTracking();
        }

        public IAsyncEnumerable<ContestantViewModel> GetAllUnapproved()
        {
            var ent = GetAllAsNoTrackingMapped().Where(x => x.isApproved != 2).AsQueryable().ToAsyncEnumerable();
            return ent;
        }
        public async ValueTask<int> GetAllCountByCategory(int id)
        {
            var ent = await GetAllAsNoTracking().Where(x => x.CategoryId == id).AsQueryable().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IAsyncEnumerable<ContestantViewModel> GetTop(int entitiesToShow)
        {
            return GetAllAsNoTrackingMapped()
              //     .Where(x => x.isApproved && (x.profileVisiblity == 0) && ((x.Promotion > 0) || (x.Rating > 0)))
                   .OrderBy(x => x.Id)
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<ContestantViewModel> GetLast(int entitiesToShow)
        {
            return GetAllAsNoTrackingMapped()
                   .Where(x => x.isApproved == 2 && (x.profileVisiblity == 0))
                   .OrderByDescending(x => x.Id)
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await contestantRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }

        public T GetById<T>(int id)
        {
            var ent = contestantRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefault();

            return ent;
        }
        public async ValueTask<ContestantViewModel> GetByPosterId(string id)
        {
            var ent = await contestantRepository.Set().Where(p => p.PosterID == id).To<ContestantViewModel>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }
        public async ValueTask<bool> IsValid(int Id)
        {
            var ent = await this.contestantRepository.Set().AnyAsync(x => x.Id == Id).ConfigureAwait(false);
            return ent;
        }


        public void SeedTest(FeaturesDbContext dbContext)        {            var test = new List<Contestant>();            for (int i = 0; i < 100000; i++)            {
                test = new List<Contestant>                {
                new Contestant
                {                Name = "test",                CategoryId = 3,                isApproved = 2                }               };
                dbContext.Contestant.AddRange(test);            }

            dbContext.SaveChanges();        }        public int RandomNumber(int min, int max)        {            Random random = new Random();            return random.Next(min, max);        }
    }
}