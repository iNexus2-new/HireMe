namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Skills;

    public interface ISkillsService
    {
        Task<OperationResult> Create(int id, string title);

        Task<OperationResult> Delete(int id);

        IAsyncEnumerable<SkillsViewModel> GetAll();

        IQueryable<SkillsViewModel> GetAllAsNoTrackingMapped();

        IAsyncEnumerable<SkillsViewModel> GetAllByContestant(int id);
        IAsyncEnumerable<SkillsViewModel> GetAllByJob(int id);

    }
}
