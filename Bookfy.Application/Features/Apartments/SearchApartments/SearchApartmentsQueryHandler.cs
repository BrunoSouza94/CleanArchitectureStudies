using Bookfy.Application.Abstractions.Data;
using Bookfy.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Entities.Bookings;
using Dapper;

namespace Bookfy.Application.Features.Apartments.SearchApartments
{
    internal sealed class SearchApartmentsQueryHandler 
        : IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
    {
        private ISqlConnectionFactory _sqlConnectionFactory;

        private static readonly int[] ActiveBookingStatuses =
        {
            (int)BookingStatusEnum.Reserved,
            (int)BookingStatusEnum.Confirmed,
            (int)BookingStatusEnum.Completed
        };

        public SearchApartmentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
                return new List<ApartmentResponse>();

            using var connection = _sqlConnectionFactory.CreateConnection();

            const string sql = """
                select
                    a.id as Id,
                    a.name as Name,
                    a.description as Description,
                    a.price_amount as Price,
                    a.price_currency as Currency,
                    a.address_country as Country,
                    a.address_state as State,
                    a.address_zip_code as ZipCode,
                    a.address_city as City,
                    a.address_street as Street
                from apartments as a
                where not exists
                (
                    select 1
                    from bookings as b
                    where
                        b.apartment_id = a.id and
                        b.duration_start <= @EndDate and
                        b.duration_end >= @StartDate and
                        b.status = Any(@ActiveBookingStatuses)
                )
            """;

            var apartments = await connection.QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
                sql,
                (apartment, address) =>
                {
                    apartment.Address = address;

                    return apartment;
                },
                new
                {
                    request.StartDate,
                    request.EndDate,
                    ActiveBookingStatuses
                },
                splitOn: "Country");

            return apartments.ToList();
        }
    }
}
