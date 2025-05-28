namespace FlashSale.Infrastructure.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(EmailDto dto)
        {
            var smtp = new SmtpClient
            {
                Host = _config["Smtp:Host"],
                Port = int.Parse(_config["Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"])
            };

            var message = new MailMessage("", dto.To)
            {
                Subject = dto.Subject,
                Body = dto.Body
            };

            await smtp.SendMailAsync(message);
        }
    }
}
