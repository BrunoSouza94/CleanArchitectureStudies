using Bookfy.Application.Abstractions.Behaviors;
using Bookify.Domain.Entities.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookfy.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddTransient<PrincingService>();

            return services;
        }
    }
}