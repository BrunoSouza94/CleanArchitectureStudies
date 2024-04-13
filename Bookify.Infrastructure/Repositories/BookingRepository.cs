using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories
{
    internal sealed class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private static readonly BookingStatusEnum[] ActiveBookingStatusesEnum =
        {
            BookingStatusEnum.Reserved,
            BookingStatusEnum.Confirmed,
            BookingStatusEnum.Completed
        };

        public BookingRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<bool> IsOverlappingAsync(Apartment apartment, DateRange duration, CancellationToken cancellationToken = default)
        {
            return await DbContext
                .Set<Booking>()
                .AnyAsync(
                    booking =>
                        booking.ApartmentId == apartment.Id &&
                        booking.Duration.Start <= duration.End &&
                        booking.Duration.End >= duration.Start &&
                        ActiveBookingStatusesEnum.Contains(booking.Status),
                    cancellationToken);
        }
    }
}