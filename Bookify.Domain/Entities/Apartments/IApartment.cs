namespace Bookify.Domain.Entities.Apartments
{
    public interface IApartment
    {
        Task<Apartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
