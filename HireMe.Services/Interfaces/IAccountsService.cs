namespace HireMe.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HireMe.Entities.Models;

    public interface IAccountsService
    {
        Task<bool> Register(string username, string password, string confirmPassword, string email, string firstName, string lastName, bool isemployer);

        bool Login(string username, string password, bool rememberMe);

        void Logout();

        ICollection<User> GetAllUsers();

        string LatestUsernames(string orederBy);

        void PromoteUser(string userId);

        void DemoteUser(string userId);

        ValueTask<User> GetByIdAsync(string id);

        User GetById(string id);
    }
}