namespace HireMe.Services
{
    using EntityFrameworkCore.Cacheable;
    using HireMe.Core.Helpers;
    using HireMe.Data.Repository.Interfaces;
    using HireMe.Entities.Models;
    using HireMe.Mapping.Utility;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Categories;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IRepository<Jobs> jobsRepository;
        private readonly IRepository<Contestant> contestantRepository;

        public CategoriesService(
            IRepository<Category> categoriesRepository,
            IRepository<Jobs> jobsRepository,
            IRepository<Contestant> contestantRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.jobsRepository = jobsRepository;
            this.contestantRepository = contestantRepository;
        }

        public async Task<OperationResult> Create(CategoriesViewModel viewModel)
        {
            Category category = new Category()
            {
                Title = viewModel.Title,
                Title_BG = viewModel.Title_BG,
                Icon = viewModel.Icon
            };

           return await categoriesRepository.AddAsync(category);
        }

        public async Task<OperationResult> Update(CategoriesViewModel viewModel)
        {
            Category existingcategory = await categoriesRepository.GetByIdAsync(viewModel.Id);

            existingcategory.Update(viewModel.Title, viewModel.Title_BG, viewModel.Icon);
            return await categoriesRepository.UpdateAsync(existingcategory);
        }

        public async Task<OperationResult> Delete(int id)
        {
            Category category = await categoriesRepository.GetByIdAsync(id);
            return await categoriesRepository.DeleteAsync(category);

            /*
            var jobs = this.jobsRepository.All()
             .Where(j => j.CategoryId == iD)
             .ToList();

            if (jobs.Count > 0)
            {
                foreach (var item in jobs)
                {
                    this.jobsRepository.Delete(item);
                }
            }

            var contestants = this.contestantRepository.All()
             .Where(j => j.CategoryId == iD)
             .ToList();

            if (contestants.Count > 0)
            {
                foreach (var item in contestants)
                {
                    this.contestantRepository.Delete(item);
                }
            }*/
        }

        public IAsyncEnumerable<CategoriesViewModel> GetAll()
        {
            var ent = GetAllAsNoTrackingMapped().ToAsyncEnumerable();
            return ent;
        }
        public IQueryable<CategoriesViewModel> GetAllAsNoTrackingMapped()
        {
            return categoriesRepository.Set().AsNoTracking().To<CategoriesViewModel>();
        }
        public IQueryable<Category> GetAllAsNoTracking()
        {
            return categoriesRepository.Set().AsNoTracking();
        }

        public IAsyncEnumerable<CategoriesViewModel> GetTop(int entitiesToShow)
        {
            return GetAllAsNoTrackingMapped()
                   .OrderByDescending(x => x.CountOfAllJobs)
                   .OrderByDescending(x => x.CountOfAllContestants)
                   .Take(entitiesToShow)
                   .AsQueryable()
                   .AsAsyncEnumerable();
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await categoriesRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }
        public async ValueTask<bool> IsValid(int Id)
        {
            var ent = await this.categoriesRepository.Set().AnyAsync(x => x.Id == Id).ConfigureAwait(false);
            return ent;
        }

    }
}