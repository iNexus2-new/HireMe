namespace HireMe.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Notifications;

    public interface INotificationService
    {
        Task<OperationResult> Create(string title, string url, DateTime start, NotifyType type, User user);

        Task<OperationResult> Delete(int iD, string userId);

        IAsyncEnumerable<NotificationsViewModel> GetAllBy(User user);

        ValueTask RemoveAllBy(User user);

    }
}
