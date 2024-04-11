using Bookify.Domain.Entities.Apartments;

namespace Bookify.Domain.Entities.Bookings
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> IsOverlappingAsync(
            Apartment apartment,
            DateRange duration,
            CancellationToken cancellationToken = default);

        void Add(Booking booking);
    }
}