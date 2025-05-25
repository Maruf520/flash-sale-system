namespace FlashSale.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
