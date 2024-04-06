using Bookify.Domain.Abstractions;
using Bookify.Domain.Entities.Shared;

namespace Bookify.Domain.Entities.Apartments
{
    public sealed class Apartment : Entity
    {
        public Apartment(
            Guid id,
            Name name,
            Description description,
            Address address,
            Money price,
            Money cleaningFee,
            List<AmenityEnum> amenities) 
                : base(id) 
        {
            Name = name;
            Description = description;
            Address = address;
            Price = price;
            CleaningFee = cleaningFee;
            Amenities = amenities;
        }

        public Name Name { get; private set; }

        public Description Description { get; private set; }

        public Address Address { get; private set; }

        public Money Price { get; private set; }

        public Money CleaningFee { get; private set; }

        public DateTime? LastBookedOnUtc { get; internal set; }

        public List<AmenityEnum> Amenities { get; private set; } = new();
    }
}