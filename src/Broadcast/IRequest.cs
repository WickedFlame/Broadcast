
namespace Broadcast
{
    /// <summary>
    /// A request that can be published with the help of a <see cref="IRequestHandler{T}"/>
    /// </summary>
    public interface IRequest
    {
    }

    /// <summary>
    /// A request that can be published with the help of a <see cref="IRequestHandler{T}"/>
    /// </summary>
    public interface IRequest<TResult>
    {
    }
}
