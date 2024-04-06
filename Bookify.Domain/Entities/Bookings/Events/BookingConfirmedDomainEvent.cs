using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Entities.Bookings.Events
{
    public sealed record BookingConfirmedDomainEvent(Guid BookingId) : IDomainEvent;
}