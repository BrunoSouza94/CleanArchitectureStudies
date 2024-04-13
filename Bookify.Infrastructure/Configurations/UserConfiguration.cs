﻿using Bookify.Domain.Entities.Users;
using Bookify.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.FirstName)
                .HasMaxLength(200)
                .HasConversion(firstName => firstName.Value, value => new FirstName(value));

            builder.Property(user => user.LastName)
                .HasMaxLength(4000)
                .HasConversion(lastName => lastName.Value, value => new LastName(value));

            builder.Property(user => user.Email)
                .HasMaxLength(4000)
                .HasConversion(email => email.Value, value => new Email(value));

            builder.HasIndex(user => user.Email).IsUnique();
        }
    }
}