using Bookfy.Application.Abstractions.Messaging;

namespace Bookfy.Application.Features.Apartments.SearchApartments
{
    public sealed record SearchApartmentsQuery(DateOnly StartDate, DateOnly EndDate)
        : IQuery<IReadOnlyList<ApartmentResponse>>;
}