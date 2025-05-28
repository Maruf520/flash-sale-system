using FlashSale.Workers.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlashSale.Workers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorkerServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, SmtpEmailService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderPlacedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });

                    cfg.ReceiveEndpoint("order-placed-queue", e =>
                    {
                        e.ConfigureConsumer<OrderPlacedConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
