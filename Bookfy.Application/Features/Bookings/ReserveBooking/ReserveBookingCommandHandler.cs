using Bookfy.Application.Abstractions.Clock;
using Bookfy.Application.Abstractions.Messaging;
using Bookfy.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Users;

namespace Bookfy.Application.Features.Bookings.ReserveBooking
{
    internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly PrincingService _princingService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBookingRepository _bookingRepository;
        private readonly IApartmentRepository _apartmentRepository;

        public ReserveBookingCommandHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            PrincingService princingService,
            IDateTimeProvider dateTimeProvider,
            IBookingRepository bookingRepository,
            IApartmentRepository apartmentRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _princingService = princingService;
            _dateTimeProvider = dateTimeProvider;
            _bookingRepository = bookingRepository;
            _apartmentRepository = apartmentRepository;
        }

        public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure<Guid>(UserErrors.NotFound);

            Apartment apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

            if (apartment is null)
                return Result.Failure<Guid>(ApartmentErrors.NotFound);

            DateRange duration = DateRange.Create(request.StartDate, request.EndDate);

            if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
                return Result.Failure<Guid>(BookingErrors.Overlap);

            try
            {
                Booking booking = Booking.Reserve(
                apartment,
                user.Id,
                duration,
                _dateTimeProvider.UtcNow,
                _princingService);

                _bookingRepository.Add(booking);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return booking.Id;
            }
            catch (ConcurrencyException)
            {
                return Result.Failure<Guid>(BookingErrors.Overlap);
            }
            
        }
    }
}