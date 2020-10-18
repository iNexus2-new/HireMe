using HireMe.Core.Helpers;
using HireMe.Data.Repository.Interfaces;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<Promotion> _promotionRepository;

        public PromotionService(IRepository<Promotion> promotionRepository)
        {
            this._promotionRepository = promotionRepository;
        }
        
        public async Task<OperationResult> Create(int id, string userid, PromotionEnum type, DateTime start, DateTime end)
        {
            if (await IsPromotionExists(userid, type))
            {
                return OperationResult.FailureResult("Promotion failure contact with support !");
            }

            var promo = new Promotion
            {
                Id = id,
                UserId = userid,
                Type = type,
                StartTime = start,
                EndTime = end
            };

             return await _promotionRepository.AddAsync(promo);
        }

        public async Task<OperationResult> Delete(int id, string userId)
        {
            Promotion entity = await _promotionRepository.GetByIdAsync(id);
            return await _promotionRepository.DeleteAsync(entity);
        }

        private async Task<bool> IsPromotionExists(string userId, PromotionEnum promotion)
        {
            return await _promotionRepository.Set().AsNoTracking().AnyAsync(x => (x.UserId == userId) && (x.Type == promotion));
        }

    }
}