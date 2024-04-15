using Bookify.Domain.Abstractions;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings.Events;
using Bookify.Domain.Entities.Shared;

namespace Bookify.Domain.Entities.Bookings
{
    public sealed class Booking : Entity
    {
        private Booking(
            Guid id,
            Guid apartmentId,
            Guid userId,
            DateRange duration,
            Money price,
            Money cleaningFee,
            Money amenitiesUpCharge,
            Money total,
            BookingStatusEnum status,
            DateTime createdOnUtc)
            : base(id) 
        {
            ApartmentId = apartmentId;
            UserId = userId;
            Duration = duration;
            Price = price;
            CleaningFee = cleaningFee;
            AmenitiesUpCharge = amenitiesUpCharge;
            Total = total;
            Status = status;
            CreatedOnUtc = createdOnUtc;
        }

        private Booking()
        {
        }

        public Guid ApartmentId { get; private set; }

        public Guid UserId { get; private set; }

        public DateRange Duration { get; private set; }

        public Money Price { get; private set; }

        public Money CleaningFee { get; private set; }

        public Money AmenitiesUpCharge { get; private set; }

        public Money Total {  get; private set; }

        public BookingStatusEnum Status { get; private set; }

        public DateTime CreatedOnUtc { get; private set; }

        public DateTime? ConfirmedOnUtc { get; private set; }

        public DateTime? RejectedOnUtc { get; private set; }

        public DateTime? CompletedOnUtc { get; private set; }

        public DateTime? CancelledOnUtc { get; private set; }

        public static Booking Reserve(Apartment apartment, Guid userId, DateRange duration, DateTime utcNow, PrincingService princingService)
        {
            PrincingDetails princingDetails = princingService.CalculatePrice(apartment, duration);

            Booking booking = new(
                Guid.NewGuid(),
                apartment.Id,
                userId,
                duration,
                princingDetails.PriceForRange,
                princingDetails.CleaningFee,
                princingDetails.AmenitiesUpCharge,
                princingDetails.Total,
                BookingStatusEnum.Reserved,
                utcNow);

            booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

            apartment.LastBookedOnUtc = utcNow;

            return booking;
        }

        public Result Confirm(DateTime utcNow)
        {
            if (Status != BookingStatusEnum.Reserved)
                return Result.Failure(BookingErrors.NotReserved);

            Status = BookingStatusEnum.Confirmed;
            ConfirmedOnUtc = utcNow;

            RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));

            return Result.Success();
        }

        public Result Reject(DateTime utcNow)
        {
            if (Status != BookingStatusEnum.Reserved)
                return Result.Failure(BookingErrors.NotReserved);

            Status = BookingStatusEnum.Rejected;
            RejectedOnUtc = utcNow;

            RaiseDomainEvent(new BookingRejectedDomainEvent(Id));

            return Result.Success();
        }

        public Result Cancel(DateTime utcNow)
        {
            if (Status != BookingStatusEnum.Confirmed)
                return Result.Failure(BookingErrors.NotConfirmed);

            Status = BookingStatusEnum.Completed;
            CompletedOnUtc = utcNow;

            RaiseDomainEvent(new BookingCompletedDomainEvent(Id));

            return Result.Success();
        }

        public Result Complete(DateTime utcNow)
        {
            if (Status != BookingStatusEnum.Confirmed)
                return Result.Failure(BookingErrors.NotConfirmed);

            DateOnly currentDate = DateOnly.FromDateTime(utcNow);

            if (currentDate > Duration.Start)
                return Result.Failure(BookingErrors.AlreadyStarted);

            Status = BookingStatusEnum.Cancelled;
            CancelledOnUtc = utcNow;

            RaiseDomainEvent(new BookingCancelledDomainEvent(Id));

            return Result.Success();
        }
    }
}