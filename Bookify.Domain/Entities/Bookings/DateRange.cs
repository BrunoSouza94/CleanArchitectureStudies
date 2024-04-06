﻿namespace Bookify.Domain.Entities.Bookings
{
    public record DateRange
    {
        private DateRange()
        {

        }

        public DateOnly Start { get; init; }

        public DateOnly End { get; init; }

        public int LengthInDays => End.DayNumber - Start.DayNumber;

        public static DateRange Create(DateOnly start, DateOnly end)
        {
            if (start > end)
                throw new ApplicationException("End date must be grater than start date");

            return new DateRange
            {
                Start = start,
                End = end
            };
        }
    }
}