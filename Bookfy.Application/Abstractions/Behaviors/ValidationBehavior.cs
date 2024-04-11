﻿using Bookfy.Application.Abstractions.Messaging;
using Bookfy.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Bookfy.Application.Abstractions.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationErorrs = _validators.Select(validator => validator.Validate(context))
                                              .Where(validationResult => validationResult.Errors.Any())
                                              .SelectMany(validationResult => validationResult.Errors)
                                              .Select(validationFailure => new ValidationError(
                                                  validationFailure.PropertyName,
                                                  validationFailure.ErrorMessage))
                                              .ToList();

            if (validationErorrs.Any())
                throw new Exceptions.ValidationException(validationErorrs);

            return await next();
        }
    }
}