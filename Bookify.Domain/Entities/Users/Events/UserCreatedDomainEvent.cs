using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Entities.Users.Events
{
    public record UserCreatedDomainEvent(Guid UserId) : IDomainEvent; 
}