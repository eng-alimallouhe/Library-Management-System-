namespace LMS.Application.Abstractions.Communications
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
