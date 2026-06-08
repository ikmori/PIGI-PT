using MediatR;

namespace PIGI_PT_Application.Commands
{
    /// <summary>
    /// Define un comando que representa una acción de escritura o modificación de estado en el sistema.
    /// Implementa <see cref="IRequest"/> de MediatR y no devuelve una respuesta.
    /// </summary>
    public interface ICommand : IRequest { }

    /// <summary>
    /// Define un comando que representa una acción de escritura o modificación de estado en el sistema.
    /// Implementa <see cref="IRequest{TResponse}"/> de MediatR y devuelve un resultado de tipo <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">Tipo del objeto devuelto como resultado de la ejecución del comando.</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
