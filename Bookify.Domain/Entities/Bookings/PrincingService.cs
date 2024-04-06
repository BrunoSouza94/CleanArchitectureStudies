using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Shared;
using System.Numerics;

namespace Bookify.Domain.Entities.Bookings
{
    public class PrincingService
    {
        public PrincingDetails CalculatePrice(Apartment apartment, DateRange dateRange)
        {
            Currency currency = apartment.Price.Currency;

            Money priceForRange = new(apartment.Price.Amount * dateRange.LengthInDays, currency);

            decimal percentageUpCharge = 0;
            foreach(var amenity in apartment.Amenities)
            {
                percentageUpCharge += amenity switch
                {
                    AmenityEnum.GardenView or AmenityEnum.MountainView => 0.05m,
                    AmenityEnum.AirConditioning => 0.01m,
                    AmenityEnum.Parking => 0.01m,
                    _ => 0
                };
            }

            Money total = Money.Zero(currency);
            total += priceForRange;

            Money amenitiesUpCharge = Money.Zero(currency);
            if (percentageUpCharge > 0)
            {
                amenitiesUpCharge = new(priceForRange.Amount * percentageUpCharge, currency);
                total += amenitiesUpCharge;
            }

            if(!apartment.CleaningFee.IsZero())
                total += apartment.CleaningFee;

            return new PrincingDetails(priceForRange, apartment.CleaningFee, amenitiesUpCharge, total);
        }
    }
}