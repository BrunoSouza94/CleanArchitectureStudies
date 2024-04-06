using Bookify.Domain.Entities.Shared;

namespace Bookify.Domain.Entities.Bookings
{
    public record PrincingDetails(Money PriceForRange, Money CleaningFee, Money AmenitiesUpCharge, Money Total);
}