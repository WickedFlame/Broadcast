using System;

namespace Broadcast
{
	/// <summary>
    /// The ActivationContext is used to resolve objects and create instances
    /// </summary>
    public interface IActivationContext
    {
        /// <summary>
        /// Register a type to be resolved to a key type. Usualy the key type is a interface that represents the object to be used
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        void Register<TService, TImpl>() where TImpl : TService;

        /// <summary>
        /// Register a instance to be resolved to a key type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instanceCreator"></param>
        void Register<TService>(Func<TService> instanceCreator);

        /// <summary>
        /// Register a singleton to the context. The instance is at all times the same
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        void RegisterSingleton<TService>(TService instance);

        /// <summary>
        /// Register a singleton to the context. The instance is at all times the same
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instanceCreator"></param>
        void RegisterSingleton<TService>(Func<TService> instanceCreator);

        /// <summary>
        /// Resolve a object and return it as the key type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        /// Resolve a object and return it as the key type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object Resolve(Type serviceType);

        /// <summary>
        /// Get a copy of the current context. All registrations are the same
        /// </summary>
        /// <returns></returns>
        IActivationContext ChildContext();
    }
}
