namespace HireMe.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Models;
    using HireMe.ViewModels.Message;

    public interface IMessageService
    { 
        Task<OperationResult> Create(string title, string description, string senderid, string receiverid);

        Task<OperationResult> CreateReport(string title, string description, string senderid, string receiverid);

        Task<OperationResult> Delete(int id, string userId);

        IAsyncEnumerable<TViewModel> GetAllMessagesBy_Sender<TViewModel>(string Id, int count);

        IAsyncEnumerable<TViewModel> GetAllMessagesBy_Receiver<TViewModel>(string Id, int count);

        IQueryable<MessageViewModel> GetAllAsNoTrackingMapped();
        IQueryable<Message> GetAllAsNoTracking();

        T GetById<T>(int id);
        ValueTask<bool> IsValid(int Id);
        ValueTask<T> GetByIdAsync<T>(int id);

        ValueTask<OperationResult> Add_MessageState(int msgId, MessageStates msgState, bool type);

        ValueTask<int> GetMessagesCountBy_Sender(string Id);
        ValueTask<int> GetMessagesCountBy_Receiver(string Id);
        ValueTask<int> GetMessagesUnreadBy_Receiver(string Id);


    }
}
