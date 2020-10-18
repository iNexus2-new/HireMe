namespace HireMe.Services.Core.Interfaces
{
    using System.Threading.Tasks;

    public interface IEmailSenderGrid
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}