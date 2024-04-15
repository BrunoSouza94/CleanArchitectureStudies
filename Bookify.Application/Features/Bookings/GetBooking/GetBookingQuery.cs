using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Features.Bookings.GetBooking
{
    public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;
}