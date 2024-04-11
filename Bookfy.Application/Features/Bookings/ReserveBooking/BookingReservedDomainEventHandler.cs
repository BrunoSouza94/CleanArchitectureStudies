using Bookfy.Application.Abstractions;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Bookings.Events;
using Bookify.Domain.Entities.Users;
using MediatR;

namespace Bookfy.Application.Features.Bookings.ReserveBooking
{
    internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingReservedDomainEventHandler(IEmailService emailService, IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
        {
            Booking booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

            if (booking is null)
                return;

            User user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);

            if (user is null)
                return;

            await _emailService.SendAsync(
                user.Email,
                "Booking reserved!",
                "You have 10 minutes to confirm this booking.");
        }
    }
}
