using MediatR;

namespace PIGI_PT_Application.Queries
{
    /// <summary>
    /// Define una consulta de solo lectura en el sistema.
    /// Implementa <see cref="IRequest{TResponse}"/> de MediatR y devuelve un resultado de tipo <typeparamref name="TResponse"/>.
    /// Las consultas nunca deben modificar el estado de la base de datos.
    /// </summary>
    /// <typeparam name="TResponse">Tipo del objeto devuelto con la información consultada.</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
