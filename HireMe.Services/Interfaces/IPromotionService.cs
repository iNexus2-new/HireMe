namespace HireMe.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;

    public interface IPromotionService
    {
        Task<OperationResult> Create(int id, string userid, PromotionEnum type, DateTime start, DateTime end);

        Task<OperationResult> Delete(int iD, string userId);
    }
}
