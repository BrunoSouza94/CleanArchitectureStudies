using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Entities.Bookings
{
    public static class BookingErrors
    {
        public static Error NotFound = new(
            "Booking.NotFound",
            "The specified booking was not found.");

        public static Error Overlap = new(
            "Booking.Overlap",
            "The period is overlapping a existing booking.");

        public static Error NotReserved = new(
            "Booking.NotReserved",
            "There's no reservation to this apartment.");

        public static Error NotConfirmed = new(
            "Booking.NotConfirmed",
            "This booking was not confirmed yet.");

        public static Error AlreadyStarted = new(
            "Booking.AlreadyStarted",
            "This booking has already started");
    }
}