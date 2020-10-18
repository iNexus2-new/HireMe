using HireMe.Core.Helpers;
using HireMe.Data.Repository.Interfaces;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notifyRepository;
  
        public NotificationService(IRepository<Notification> notifyRepository)
        {
            this._notifyRepository = notifyRepository;
        }

        public async Task<OperationResult> Create(string title, string url, DateTime start, NotifyType type, User user)
        {
            var notification = new Notification
            {
                Title = title,
                Url = url,
                Date = start,
                Type = type,
                UserId = user.Id
            };

            return await _notifyRepository.AddAsync(notification);
        }

        public async Task<OperationResult> Delete(int id, string userId)
        {
            Notification entity = await _notifyRepository.GetByIdAsync(id);
            return await _notifyRepository.DeleteAsync(entity);
        }


        public IAsyncEnumerable<NotificationsViewModel> GetAllBy(User user)
        {
            var entity = GetAllAsNoTrackingMapped()
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Date)
                .AsQueryable()
                .AsAsyncEnumerable();


            return entity;
        }
        public async ValueTask RemoveAllBy(User user)
        {
            var entities = await _notifyRepository.Set()
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            if (entities != null && entities.Count() > 0)
            {
                foreach (var item in entities)
                {
                   await _notifyRepository.DeleteAsync(item);
                }
            }                
        }
        private IQueryable<NotificationsViewModel> GetAllAsNoTrackingMapped()
        {
            return _notifyRepository.Set().AsNoTracking().To<NotificationsViewModel>();
        }

    }
}