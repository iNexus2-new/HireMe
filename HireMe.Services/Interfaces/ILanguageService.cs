namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using HireMe.Entities.Enums;
    using HireMe.ViewModels.Language;

    public interface ILanguageService
    {
        IAsyncEnumerable<LanguageViewModel> GetAll();

        IQueryable<LanguageViewModel> GetAllAsNoTrackingMapped();

        IAsyncEnumerable<LanguageViewModel> GetAllByContestant(int id);
        IAsyncEnumerable<LanguageViewModel> GetAllByJob(int id);

    }
}
