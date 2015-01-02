using System.Threading.Tasks;

namespace Broadcast
{
    /// <summary>
    /// Publishes a <see cref="IRequest"/> to a specific <see cref="IRequestHandler"/>
    /// </summary>
    /// <typeparam name="T">The request to publish</typeparam>
    public interface IRequestPublisher<T> where T : IRequest
    {
        void Handle(T request);

        Task HandleAsync(T request);
    }

    /// <summary>
    /// Publishes a <see cref="IRequest"/> to a specific <see cref="IRequestHandler"/>
    /// </summary>
    /// <typeparam name="T">The request to publish</typeparam>
    /// <typeparam name="TResult">The expected type of result</typeparam>
    public interface IRequestPublisher<T, TResult> where T : IRequest<TResult>
    {
        TResult Handle(T request);

        Task<TResult> HandleAsync(T request);
    }
}
