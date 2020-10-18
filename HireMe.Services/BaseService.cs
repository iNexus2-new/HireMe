using HireMe.Data.Repository.Interfaces;
using HireMe.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using HireMe.Entities.Enums;
using HireMe.Core.Helpers;
using HireMe.Entities.Models;
using HireMe.Mapping.Utility;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using HireMe.Firewall.Checker.Core.Interface;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace HireMe.Services
{
    public class BaseService : IBaseService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Jobs> _jobsRepository;
        private readonly IRepository<Contestant> _contestantRepository;
        private readonly IRepository<Company> _companiesRepository;

        private readonly IScanner _scanner;
        private readonly ILogger<BaseService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IToastNotification _toastNotification;

        private readonly long _fileSizeLimit;
        private readonly string _FilePath;
        private readonly string[] _permittedFiles = { ".pdf", ".ps", ".doc", ".docx", ".odt", ".sxw", ".txt", ".rtf" };

        private readonly long _imageSizeLimit;
        private readonly string _ImagePath;
        private readonly string[] _permittedImages = { ".png", ".jpg", ".gif", ".jpeg", ".JPG", ".JPEG", ".PNG", ".GIF" };


        public BaseService(
              IConfiguration config,
              IScanner scanner,
              ILogger<BaseService> loggerFactory,
              IRepository<User> userRepository,
              IRepository<Jobs> jobsRepository,
              IRepository<Contestant> contestantRepository,
              IRepository<Company> companiesRepository,
              INotificationService notificationService,
              IToastNotification toastNotification)
        {
             _userRepository = userRepository;
             _jobsRepository = jobsRepository;
             _contestantRepository = contestantRepository;
             _companiesRepository = companiesRepository;

            _logger = loggerFactory;
            _scanner = scanner;
            _notificationService = notificationService;
            _toastNotification = toastNotification;

            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _FilePath = config.GetValue<string>("StoredFilesPath");

            _imageSizeLimit = config.GetValue<long>("ImageSizeLimit");
            _ImagePath = config.GetValue<string>("StoredImagesPath");
        }

        public void ToastNotify(ToastMessageState state, string title, string message, int duration)
        {
            switch (state)
            {
                case ToastMessageState.Success:_toastNotification.AddSuccessToastMessage(message, new ToastrOptions() { Title = title, TimeOut = duration });
                    break;
                case ToastMessageState.Info:_toastNotification.AddInfoToastMessage(message, new ToastrOptions() { Title = title, TimeOut = duration });          
                    break;
                case ToastMessageState.Alert: _toastNotification.AddAlertToastMessage(message, new ToastrOptions() { Title = title, TimeOut = duration });          
                    break;
                case ToastMessageState.Warning:_toastNotification.AddWarningToastMessage(message, new ToastrOptions() { Title = title, TimeOut = duration });
                    break;
                case ToastMessageState.Error:_toastNotification.AddErrorToastMessage(message, new ToastrOptions() { Title = title, TimeOut = duration });
                    break;
            } 
        }

         public IEnumerable<TViewModel> GetUserBy_FirstName<TViewModel>(string name)
        {
            var users = _userRepository.Set()
                .Where(p => p.FirstName.Contains(name))
                .Select(p => p.FirstName)
                .To<TViewModel>()
                .ToList();

            return users;
        }

        public async ValueTask<OperationResult> Approve(int Id, PostType postType, int type)
        {
            switch (postType)
            {
                case PostType.Company:
                    {

                        var item = await _companiesRepository.Set().FirstOrDefaultAsync(j => j.Id == Id);
                        item.isApproved = type;

                        var success = await _companiesRepository.SaveChangesAsync();
                        return success;
                    }
                case PostType.Contestant:
                    {

                        var item = await _contestantRepository.Set().FirstOrDefaultAsync(j => j.Id == Id);
                        item.isApproved = type;

                        var success = await _contestantRepository.SaveChangesAsync();
                        return success;
                    }

                case PostType.Job:
                    {

                        var item = await _jobsRepository.Set().FirstOrDefaultAsync(j => j.Id == Id);
                        item.isApproved = type;

                        var success = await _jobsRepository.SaveChangesAsync();
                        return success;
                    }
            }
            return null;
        }

        [RequestSizeLimit(5000000)]
        public async Task<string> UploadFileAsync(IFormFile file, User user)
        {
            if (file == null)
            {
                return null;
            }
            string extension = Path.GetExtension(file.FileName);

            if (String.IsNullOrEmpty(extension) || !_permittedFiles.Contains(extension))
            {
                return null;
            }
            string fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
            string filePath = Path.Combine(_FilePath, fileName);

            using (FileStream fileStream = File.Create(filePath))
            {
                await file.CopyToAsync(fileStream);
            }

            var result = await fileScanner(fileName, filePath);

            return result;
        }

        [RequestSizeLimit(2000000)]
        public async Task<string> UploadImageAsync(IFormFile file, User user)
        {
            if (file == null)
            {
                return null;
            }
            string extension = Path.GetExtension(file.FileName);

            if (String.IsNullOrEmpty(extension) || !_permittedImages.Contains(extension))
            {
                return null;
            }
            string fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
            string filePath = Path.Combine(_ImagePath, fileName);

            using (FileStream fileStream = File.Create(filePath))
            {
                await file.CopyToAsync(fileStream);
            }

            var result = await fileScanner(fileName, filePath);

            return result;
        }
        public async Task<string> fileScanner(string fileName, string filePath)
        {
            //      var scan = await _scanner.ScanAsync(filePath);

            //   var result = _scanner.completedProcess(scan);

            /*
            switch (_scanner.Scan(filePath))
            {
                case ScanResult.ThreatFound:
                    {
                        Delete(filePath);
                        ToastNotify(ToastMessageState.Error, "Грешка", "Вашият файл не може да бъде качен. Опитайте по-късно.", 5000)
            ToastNotify(ToastMessageState.Error, "Грешка", "Вашият файл не може да бъде качен. Опитайте по-късно.", 5000)
                        this._logger.LogError($"\n\n\n\n\nUser with name: , is trying to upload virus file.");
                        return null; // success = false;
                    }
                case ScanResult.NoThreatFound:
                    {
                        this._logger.LogInformation($"\n\n\n\n\n's, file: '{fileName}' is clean.");
                        return fileName;// success = true;
                    }
                case ScanResult.FileNotFound:
                    {
                        this._logger.LogError($"\n\n\n\n\nNo file found to scan.");
                        return null; // success = false;
                    }

                case ScanResult.Timeout:
                    {
                        this._logger.LogError($"\n\n\n\n\nScan Timeout.");
                        return null; // success = false;
                    }

                case ScanResult.Error:
                    {
                        this._logger.LogError($"\n\n\n\n\nScan Error.");
                        return null; // success = false;
                    }
                default:
                    {
                        this._logger.LogError($"\n\n\n\n\nScan Error.");
                        return null; // success = false;
                    }
            }*/
            return fileName;
        }

        public bool Delete(string fullpath)
        {
            if (!File.Exists(fullpath))
            {
                return false;
            }

            bool isDeleted = false;
            while (!isDeleted)
            {
                try
                {
                    File.Delete(fullpath);
                    isDeleted = true;
                    return true;
                }
                catch (Exception e)
                {
                }
                Thread.Sleep(50);
            }
            return false;
        }

    }
}
