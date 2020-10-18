namespace HireMe.Services
{
    using HireMe.Data.Repository.Interfaces;
    using HireMe.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Entities.Models;
    using HireMe.Core.Helpers;
    using HireMe.Mapping.Utility;
    using HireMe.ViewModels.Company;
    using Microsoft.EntityFrameworkCore;
    using HireMe.Entities.Input;
    using HireMe.Data;
    using System;

    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> companyRepository;
        public CompanyService(IRepository<Company> companyRepository, FeaturesDbContext _context)
        {
            this.companyRepository = companyRepository;
         //   this.SeedTest(_context);
        }

        public async Task<OperationResult> Create(CreateCompanyInputModel viewModel, bool authenticEIK, User user)
        {
            Company company = new Company();
            company.Update(viewModel, 0, authenticEIK, user);

            var entity = await this.companyRepository.AddAsync(company);
            return entity;
        }

        public async Task<OperationResult> Delete(int id)
        {
            Company entityId = await companyRepository.GetByIdAsync(id);

            var entity = await companyRepository.DeleteAsync(entityId);
            return entity;
        }

        public async Task<OperationResult> Update(CreateCompanyInputModel viewModel, bool authenticEIK, User user)
        {
            var existEntity = companyRepository.GetById(viewModel.Id);

            existEntity.Update(viewModel, existEntity.isApproved, authenticEIK, user);

            var entity = await this.companyRepository.UpdateAsync(existEntity);
            return entity;
        }

        public IAsyncEnumerable<CompanyViewModel> GetAll()
        {
            var ent = GetAllAsNoTrackingMapped().ToAsyncEnumerable();
            return ent;
        }
        public async ValueTask<int> GetAllCount()
        {
            var ent = await GetAllAsNoTracking().CountAsync().ConfigureAwait(false);
            return ent;
        }
        public IQueryable<CompanyViewModel> GetAllAsNoTrackingMapped()
        {
            return companyRepository.Set().AsNoTracking().To<CompanyViewModel>();
        }
        public IQueryable<Company> GetAllAsNoTracking()
        {
            return companyRepository.Set().AsNoTracking();
        }

        public IAsyncEnumerable<CompanyViewModel> GetAllUnapproved()
        {
            var ent = GetAllAsNoTrackingMapped().Where(x => x.isApproved != 2).AsQueryable().ToAsyncEnumerable();
            return ent;
        }
        public IAsyncEnumerable<CompanyViewModel> GetTop(int entitiesToShow)
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

        public IAsyncEnumerable<CompanyViewModel> GetLast(int entitiesToShow)
        {
            return GetAllAsNoTrackingMapped()
                   .Where(x => x.isApproved == 2)
                   .OrderByDescending(x => x.Id)
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await companyRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }
        public async ValueTask<bool> IsValid(int Id)
        {
            var ent = await this.companyRepository.Set().AnyAsync(x => x.Id == Id).ConfigureAwait(false);
            return ent;
        }
        public T GetById<T>(int id)
        {
            var ent = companyRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefault();

            return ent;
        }

        public void SeedTest(FeaturesDbContext dbContext)        {            var test = new List<Company>();            for (int i = 0; i < 100000; i++)            {
                test = new List<Company>                {
                new Company
                {                Title = "test",                isApproved = 2                }               };
                dbContext.Company.AddRange(test);            }

            dbContext.SaveChanges();        }        public int RandomNumber(int min, int max)        {            Random random = new Random();            return random.Next(min, max);        }

        public async ValueTask<bool> AddRatingToCompany(int Id, int rating)
        {
            var job = await this.companyRepository.Set().FirstOrDefaultAsync(j => j.Id == Id).ConfigureAwait(false);

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

                await companyRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}