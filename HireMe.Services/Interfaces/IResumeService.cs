namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Resume;

    public interface IResumeService
    {
        Task<OperationResult> Create(string title, string fileid, User user);

        Task<OperationResult> Delete(int iD, string userId);

        IAsyncEnumerable<ResumeViewModel> GetAllBy(User user);
        IAsyncEnumerable<ResumeViewModel> GetAllReceived(User user);

        IQueryable<ResumeViewModel> GetAllByQueryable(User user);
        IQueryable<ResumeViewModel> GetAllAsNoTrackingMapped();


        ValueTask<T> GetByIdAsync<T>(int id);

    }
}
