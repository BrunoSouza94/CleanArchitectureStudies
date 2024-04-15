using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Entities.Apartments;
using Bookify.Domain.Entities.Bookings;
using Bookify.Domain.Entities.Shared;
using Bookify.Domain.Entities.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IPublisher _publisher;

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            // * bookings * //
            modelBuilder.Entity<Booking>().ToTable("bookings");

            modelBuilder.Entity<Booking>().HasKey(booking => booking.Id);

            modelBuilder.Entity<Booking>().OwnsOne(booking => booking.Price, priceBuilder =>
            {
                priceBuilder.Property(money => money.Currency)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

            modelBuilder.Entity<Booking>().OwnsOne(booking => booking.CleaningFee, priceBuilder =>
            {
                priceBuilder.Property(money => money.Currency)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

            modelBuilder.Entity<Booking>().OwnsOne(booking => booking.AmenitiesUpCharge, priceBuilder =>
            {
                priceBuilder.Property(money => money.Currency)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

            modelBuilder.Entity<Booking>().OwnsOne(booking => booking.Total, priceBuilder =>
            {
                priceBuilder.Property(money => money.Currency)
                    .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
            });

            modelBuilder.Entity<Booking>().OwnsOne(booking => booking.Duration);

            modelBuilder.Entity<Booking>().HasOne<Apartment>()
                .WithMany()
                .HasForeignKey(booking => booking.ApartmentId);

            modelBuilder.Entity<Booking>().HasOne<User>()
                .WithMany()
                .HasForeignKey(booking => booking.UserId);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                await PublishDomainEventAsync();

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("Concurrency exception occurred.", ex);
            }
        }

        private async Task PublishDomainEventAsync()
        {
            var domainEvents = ChangeTracker.Entries<Entity>()
                                            .Select(entry => entry.Entity)
                                            .SelectMany(entity =>
                                            {
                                                var domainEvents = entity.GetDomainEvents();

                                                entity.ClearDomainEvents();

                                                return domainEvents;
                                            })
                                            .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}