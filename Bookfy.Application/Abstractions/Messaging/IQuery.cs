using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookfy.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}