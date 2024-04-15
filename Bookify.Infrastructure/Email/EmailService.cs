using Bookify.Application.Abstractions;
using Bookify.Domain.Entities.Users;

namespace Bookify.Infrastructure.NovaPasta
{
    internal sealed class EmailService : IEmailService
    {
        public Task SendAsync(Email recipient, string subject, string body)
        {
            return Task.CompletedTask;
        }
    }
}