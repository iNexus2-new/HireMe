namespace HireMe.Services.Interfaces
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Contestants;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IContestantsService
    {
        Task<OperationResult> Create(CreateContestantInputModel viewModel, User user);
        Task<OperationResult> Delete(int id);
        Task<OperationResult> Update(CreateContestantInputModel viewModel, User user);

        IQueryable<Contestant> GetAllAsNoTracking();
        IQueryable<ContestantViewModel> GetAllAsNoTrackingMapped();

        ValueTask<int> GetAllCount();
        IAsyncEnumerable<ContestantViewModel> GetTop(int entitiesToShow);
        IAsyncEnumerable<ContestantViewModel> GetLast(int entitiesToShow);
        IAsyncEnumerable<ContestantViewModel> GetAllUnapproved();

        ValueTask<int> GetAllCountByCategory(int id);
        ValueTask<T> GetByIdAsync<T>(int id);

        ValueTask<ContestantViewModel> GetByPosterId(string id);
        ValueTask<bool> IsValid(int Id);
        T GetById<T>(int id);
    }
}