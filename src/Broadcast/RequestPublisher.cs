using System.Threading.Tasks;

namespace Broadcast
{
    public class RequestPublisher<T> : IRequestPublisher<T> where T : IRequest
    {
        private readonly IRequestHandler<T> _handler;

        public RequestPublisher(IRequestHandler<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T request)
        {
            _handler.Handle(request);
        }

        public async Task HandleAsync(T request)
        {
            await Task.Run(() =>_handler.Handle(request));
        }
    }

    public class RequestPublisher<T, TResult> : IRequestPublisher<T, TResult> where T : IRequest<TResult>
    {
        private readonly IRequestHandler<T, TResult> _handler;

        public RequestPublisher(IRequestHandler<T, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Handle(T request)
        {
            return _handler.Handle(request);
        }

        public async Task<TResult> HandleAsync(T request)
        {
            return await Task.Run(() => _handler.Handle(request));
        }
    }
}
