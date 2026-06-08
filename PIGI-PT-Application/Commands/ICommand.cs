using MediatR;

namespace PIGI_PT_Application.Commands
{
    public interface ICommand : IRequest { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
