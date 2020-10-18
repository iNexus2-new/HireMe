namespace HireMe.Services.Interfaces
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Jobs;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IJobsService
    {
        Task<OperationResult> Create(CreateJobInputModel viewModel, User user);
        Task<OperationResult> Update(CreateJobInputModel viewModel, User user);
        Task<OperationResult> Delete(int id);
        Task<OperationResult> DeleteAllByCompany(int companyId);
        Task<bool> AddResumeFile(int jobId, string resumeId);

        IQueryable<JobsViewModel> GetAllTrackingMapped();
        IQueryable<JobsViewModel> GetAllAsNoTrackingMapped();
        IQueryable<Jobs> GetAllAsNoTracking();

        IAsyncEnumerable<JobsViewModel> GetTop(int entitiesToShow);
        IAsyncEnumerable<JobsViewModel> GetLast(int entitiesToShow);
        IAsyncEnumerable<JobsViewModel> GetAllUnapproved();
        IAsyncEnumerable<JobsViewModel> GetByCompanyOrCategory(int id, bool isCompany, int entitiesToShow);

        ValueTask RemoveResumeFromReceived(string id, User user);
        ValueTask<bool> AddRatingToJobs(int jobId, int rating);
        ValueTask<int> GetAllCount();
        ValueTask<int> GetAllCountByCategory(int id);
        ValueTask<int> GetAllCountByCompany(int id);
        ValueTask<T> GetByIdAsync<T>(int id);
        ValueTask<bool> IsValid(int Id);

        T GetById<T>(int id);
    }
}