using System.Threading.Tasks;

namespace Broadcast
{
    /// <summary>
    /// Class that is used to publish a IRequest to a IRequestHandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestPublisher<T> : IRequestPublisher<T> where T : IRequest
    {
        private readonly IRequestHandler<T> _handler;

        public RequestPublisher(IRequestHandler<T> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Handle the IRequest
        /// </summary>
        /// <param name="request">The request to handle</param>
        public void Handle(T request)
        {
            _handler.Handle(request);
        }

        /// <summary>
        /// Handle the IRequest asynchronously
        /// </summary>
        /// <param name="request">The request to handle</param>
        /// <returns>Async Task</returns>
        public async Task HandleAsync(T request)
        {
            await Task.Run(() => _handler.Handle(request));
        }
    }

    /// <summary>
    /// Class that is used to publish a IRequest to a IRequestHandler
    /// </summary>
    /// <typeparam name="T">The type of request</typeparam>
    /// <typeparam name="TResult">The returned result</typeparam>
    public class RequestPublisher<T, TResult> : IRequestPublisher<T, TResult> where T : IRequest<TResult>
    {
        private readonly IRequestHandler<T, TResult> _handler;

        public RequestPublisher(IRequestHandler<T, TResult> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Handle the IRequest
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Result</returns>
        public TResult Handle(T request)
        {
            return _handler.Handle(request);
        }

        /// <summary>
        /// Handle the IRequest asynchronously
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>Async result</returns>
        public async Task<TResult> HandleAsync(T request)
        {
            return await Task.Run(() => _handler.Handle(request));
        }
    }
}
