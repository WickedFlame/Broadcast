
namespace Broadcast
{
    /// <summary>
    /// Handler for a <see cref="IRequest"/> object that is published through the <see cref="RequestPublisher"/>
    /// </summary>
    /// <typeparam name="T">The request that can be handled</typeparam>
    public interface IRequestHandler<T> where T : IRequest
    {
        void Handle(T request);
    }

    /// <summary>
    /// Handler for a <see cref="IRequest"/> object that is published through the <see cref="RequestPublisher"/>
    /// </summary>
    /// <typeparam name="T">The request that can be handled</typeparam>
    /// <typeparam name="TResult">The expected result</typeparam>
    public interface IRequestHandler<T, TResult> where T : IRequest<TResult>
    {
        TResult Handle(T request);
    }
}
