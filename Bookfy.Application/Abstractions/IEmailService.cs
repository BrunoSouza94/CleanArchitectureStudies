using Bookify.Domain.Entities.Users;

namespace Bookify.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(Email recipient, string subject, string body);
    }
}
