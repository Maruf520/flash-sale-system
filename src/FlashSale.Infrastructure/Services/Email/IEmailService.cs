namespace FlashSale.Infrastructure.Services.Email
{
    public interface IEmailService
    {
        Task SendAsync(EmailDto dto);
    }
}
