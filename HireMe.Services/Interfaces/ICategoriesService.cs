namespace HireMe.Services.Interfaces
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Categories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICategoriesService
    {
        Task<OperationResult> Create(CategoriesViewModel viewModel);
        Task<OperationResult> Delete(int id);
        Task<OperationResult> Update(CategoriesViewModel viewModel);

        IAsyncEnumerable<CategoriesViewModel> GetAll();
        IQueryable<Category> GetAllAsNoTracking();
        IQueryable<CategoriesViewModel> GetAllAsNoTrackingMapped();

        IAsyncEnumerable<CategoriesViewModel> GetTop(int entitiesToShow);

        ValueTask<T> GetByIdAsync<T>(int id);
        ValueTask<bool> IsValid(int Id);
    }
}
