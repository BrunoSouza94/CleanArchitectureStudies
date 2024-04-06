using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Entities.Bookings.Events
{
    public sealed record BookingCompletedDomainEvent(Guid BookingId) : IDomainEvent;
}