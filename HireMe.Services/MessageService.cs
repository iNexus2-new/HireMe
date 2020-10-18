using HireMe.Data.Repository.Interfaces;
using HireMe.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using HireMe.Core.Helpers;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using HireMe.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using HireMe.ViewModels.Message;

namespace HireMe.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> messageRepository;
        public MessageService(IRepository<Message> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        public async Task<OperationResult> Create(string title, string description, string senderid, string receiverid)
        {
            var message = new Message
            {
                Title = title,
                Description = description,
                dateTime = DateTime.Now,
                SenderId = senderid,
                ReceiverId = receiverid,
                isRead = false,
                isImportant = false,
                isStared = false
               
            };

           return await this.messageRepository.AddAsync(message);
        }
        public async Task<OperationResult> CreateReport(string title, string description, string senderid, string receiverid)
        {
            var message = new Message
            {
                Title = title,
                Description = description,
                dateTime = DateTime.Now,
                SenderId = senderid,
                ReceiverId = receiverid,
                isReport = true
            };

            return await this.messageRepository.AddAsync(message);
        }
        public async Task<OperationResult> Delete(int iD, string userid)
        {
            Message msg = await this.messageRepository.GetByIdAsync(iD);
            
            if (msg.ReceiverId == userid && !msg.deletedFromReceiver)
            {
                msg.deletedFromReceiver = true;
            }

            if (msg.SenderId == userid && !msg.deletedFromSender)
            {
                msg.deletedFromSender = true;
            }


            var success = await PostdeleteAsync(msg);
            return success ? OperationResult.SuccessResult() : OperationResult.FailureResult("Job post can't create, try again later or contact with support !");
        }

        private async Task<bool> PostdeleteAsync(Message msg)
        {
            if (msg.deletedFromReceiver && msg.deletedFromSender)
            {
                await messageRepository.DeleteAsync(msg);
                return true;
            }

            if (msg.isReport && msg.deletedFromReceiver)
            {
                await messageRepository.DeleteAsync(msg);
                return true;
            }

            return false;
        }

        public IAsyncEnumerable<TViewModel> GetAllMessagesBy_Sender<TViewModel>(string Id, int count)
        {
            var entity = GetAllAsNoTrackingMapped()
                .Where(j => j.SenderId == Id)
                .OrderByDescending(x => x.dateTime)
                .To<TViewModel>()
                .Take(count)
                .AsQueryable()
                .AsAsyncEnumerable();


            return entity;
        }
        public IAsyncEnumerable<TViewModel> GetAllMessagesBy_Receiver<TViewModel>(string Id, int count)
        {
            var entity = GetAllAsNoTracking()
                .Where(j => j.ReceiverId == Id)
                .OrderByDescending(x => x.dateTime)
                .To<TViewModel>()
                .Take(count)
                .AsQueryable()
                .AsAsyncEnumerable();


            return entity;
        }
        
        /* public IEnumerable<MessageViewModel> GetAllMessagesBy_Receiver2(string receiverId, int currentPage, int Skip, int Take)
         {
             CurrentPage = currentPage == 0 ? 1 : currentPage;

             Count = this.messageRepository.All()
               .Where(j => j.ReceiverId == receiverId)
               .Count();

             if (CurrentPage > TotalPages)
                 CurrentPage = TotalPages;

                 var messageList = this.messageRepository.All()
                 .Where(j => j.ReceiverId == receiverId)
                 .Skip((CurrentPage - 1) * PageSize)
                 .Take(PageSize)
                 .To<MessageViewModel>()
                 .ToList();


             return messageList;
         }*/

        public IQueryable<MessageViewModel> GetAllAsNoTrackingMapped()
        {
            return messageRepository.Set().AsNoTracking().To<MessageViewModel>();
        }
        public IQueryable<Message> GetAllAsNoTracking()
        {
            return messageRepository.Set().AsNoTracking();
        }

        public async ValueTask<bool> IsValid(int Id)
        {
            var ent = await this.messageRepository.Set().AnyAsync(x => x.Id == Id).ConfigureAwait(false);
            return ent;
        }

        public async ValueTask<T> GetByIdAsync<T>(int id)
        {
            var ent = await messageRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefaultAsync().ConfigureAwait(false);

            return ent;
        }

        public T GetById<T>(int id)
        {
            var ent = messageRepository.Set().Where(p => p.Id == id).To<T>().FirstOrDefault();

            return ent;
        }

        public async ValueTask<OperationResult> Add_MessageState(int msgId, MessageStates msgState, bool type)
        {
            var msg = await this.messageRepository.Set().FirstOrDefaultAsync(j => j.Id == msgId).ConfigureAwait(false);

                switch (msgState)
                {
                    case MessageStates.Read:
                        msg.isRead = type;
                        break;
                    case MessageStates.Stared:
                        msg.isStared = type;
                        break;
                    case MessageStates.Important:
                        msg.isImportant = type;
                        break;
                }
            return await messageRepository.UpdateAsync(msg);
        }

        public async ValueTask<int> GetMessagesCountBy_Sender(string Id)
        {
            int count = await GetAllAsNoTracking()
                .Where(j => j.SenderId == Id)
                .CountAsync()
                .ConfigureAwait(false);

            return count;
        }

        public async ValueTask<int> GetMessagesCountBy_Receiver(string Id)
        {
            int count = await GetAllAsNoTracking()
                .Where(j => j.ReceiverId == Id)
                .CountAsync()
                .ConfigureAwait(false);

            return count;
        }

        public async ValueTask<int> GetMessagesUnreadBy_Receiver(string Id)
        {
            int count = await GetAllAsNoTracking()
                .Where(j => j.ReceiverId == Id && !j.isRead)
                .CountAsync()
                .ConfigureAwait(false);

            return count;
        }
    }
}