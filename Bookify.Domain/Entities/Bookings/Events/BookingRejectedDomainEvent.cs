using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Entities.Bookings.Events
{
    public sealed record BookingRejectedDomainEvent(Guid BookingId) : IDomainEvent;
}