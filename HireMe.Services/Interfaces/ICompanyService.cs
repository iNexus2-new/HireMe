namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Company;

    public interface ICompanyService
    {
        Task<OperationResult> Create(CreateCompanyInputModel viewModel, bool authenticEIK, User user);
        Task<OperationResult> Delete(int id);
        Task<OperationResult> Update(CreateCompanyInputModel viewModel, bool authenticEIK, User user);

        IQueryable<Company> GetAllAsNoTracking();
        IQueryable<CompanyViewModel> GetAllAsNoTrackingMapped();

        IAsyncEnumerable<CompanyViewModel> GetAll();
        IAsyncEnumerable<CompanyViewModel> GetTop(int entitiesToShow);
        IAsyncEnumerable<CompanyViewModel> GetLast(int entitiesToShow);
        IAsyncEnumerable<CompanyViewModel> GetAllUnapproved();

        ValueTask<int> GetAllCount();
        ValueTask<bool> AddRatingToCompany(int Id, int rating);
        ValueTask<T> GetByIdAsync<T>(int id);
        ValueTask<bool> IsValid(int Id);

    }
}
