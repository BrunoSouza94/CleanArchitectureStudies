using Bookfy.Application.Abstractions.Messaging;

namespace Bookfy.Application.Features.Bookings.GetBooking
{
    public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;
}