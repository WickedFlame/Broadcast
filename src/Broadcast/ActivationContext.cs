using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Broadcast
{
	/// <summary>
	/// The ActivationContext is used to resolve objects and create instances
	/// </summary>
	public class ActivationContext : IActivationContext
	{
		private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

		/// <summary>
		/// Gets a dictionary containing all registrations
		/// </summary>
		public IReadOnlyDictionary<Type, Func<object>> Registrations => _registrations;

		/// <summary>
		/// Register a type to be resolved to a key type. Usualy the key type is a interface that represents the object to be used
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		public void Register<TService, TImpl>() where TImpl : TService
		{
			_registrations.Add(typeof(TService), () => Resolve(typeof(TImpl)));
		}

		/// <summary>
		/// Register a instance to be resolved to a key type
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="instanceCreator"></param>
		public void Register<TService>(Func<TService> instanceCreator)
		{
			_registrations.Add(typeof(TService), () => instanceCreator());
		}

		/// <summary>
		/// Register a singleton to the context. The instance is at all times the same
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="instance"></param>
		public void RegisterSingleton<TService>(TService instance)
		{
			_registrations.Add(typeof(TService), () => instance);
		}

		/// <summary>
		/// Register a singleton to the context. The instance is at all times the same
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="instanceCreator"></param>
		public void RegisterSingleton<TService>(Func<TService> instanceCreator)
		{
			var lazy = new Lazy<TService>(instanceCreator);
			Register<TService>(() => lazy.Value);
		}

		/// <summary>
		/// Resolve a object and return it as the key type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		/// <summary>
		/// Resolve a object and return it as the key type
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public object Resolve(Type serviceType)
		{
			if (_registrations.TryGetValue(serviceType, out var creator))
			{
				return creator();
			}

			if (!serviceType.IsAbstract)
			{
				return CreateInstance(serviceType);
			}

			throw new InvalidOperationException($"Could not resolve {serviceType} because there is no registration for the Type.{Environment.NewLine}");
		}

		/// <summary>
		/// Get a copy of the current context. All registrations are the same
		/// </summary>
		/// <returns></returns>
		public IActivationContext ChildContext()
		{
			var ctx = new ActivationContext();
			foreach (var registration in _registrations)
			{
				ctx._registrations.Add(registration.Key, registration.Value);
			}

			return ctx;
		}

		private object CreateInstance(Type implementationType)
		{
			try
			{
				var ctors = implementationType.GetConstructors().OrderBy(c => c.GetParameters().Length);
				if (!ctors.Any())
				{
					return Activator.CreateInstance(implementationType);
				}

				foreach(var ctor in ctors)
                {
                    var inst = CreateInstance(implementationType, ctor);
                    if (inst != null)
                    {
						return inst;
                    }
                }
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Could not create an instance of {implementationType.FullName}. See inner exceptions for reasons", e);
			}

            throw new InvalidOperationException($"Could not create an instance of {implementationType.FullName}.");
        }

        private object CreateInstance(Type implementationType, ConstructorInfo ctor)
        {
            try
            {
                var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
                var dependencies = parameterTypes.Select(t => Resolve(t)).ToArray();

                return Activator.CreateInstance(implementationType, dependencies);
            }
            catch (Exception)
            {
				return null;
            }
		}
	}
}
