using MediatR;

namespace PIGI_PT_Application.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
