using HireMe.Core.Helpers;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HireMe.Services.Interfaces
{
    public interface IBaseService
    {
        IEnumerable<TViewModel> GetUserBy_FirstName<TViewModel>(string name);

        ValueTask<OperationResult> Approve(int Id, PostType postType, int type);

        Task<string> UploadFileAsync(IFormFile file, User user);

        Task<string> UploadImageAsync(IFormFile file, User user);

        Task<string> fileScanner(string fileName, string filePath);

        public bool Delete(string fullpath);

        void ToastNotify(ToastMessageState state, string title, string message, int duration);
    }
}