using Bookify.Domain.Entities.Users;

namespace Bookfy.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(Email recipient, string subject, string body);
    }
}
