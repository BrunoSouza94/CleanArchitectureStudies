using Bookfy.Application.Abstractions.Messaging;

namespace Bookfy.Application.Features.Bookings.ReserveBooking
{
    public record ReserveBookingCommand(
        Guid ApartmentId,
        Guid UserId,
        DateOnly StartDate,
        DateOnly EndDate) : ICommand<Guid>;
}