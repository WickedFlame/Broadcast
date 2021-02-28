using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Broadcast
{
	public interface IActivationContext
	{
		void Register<TService, TImpl>() where TImpl : TService;

		void Register<TService>(Func<TService> instanceCreator);

		T Resolve<T>();

		T Resolve<T>(Type serviceType);

		object Resolve(Type serviceType);

		IActivationContext ChildContext();
	}

	public class ActivationContext : IActivationContext
	{
		private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

		public IReadOnlyDictionary<Type, Func<object>> Registrations => _registrations;

		public void Register<TService, TImpl>() where TImpl : TService
		{
			_registrations.Add(typeof(TService), () => Resolve(typeof(TImpl)));
		}

		public void Register<TService>(Func<TService> instanceCreator)
		{
			_registrations.Add(typeof(TService), () => instanceCreator());
		}

		public void RegisterSingleton<TService>(TService instance)
		{
			_registrations.Add(typeof(TService), () => instance);
		}

		public void RegisterSingleton<TService>(Func<TService> instanceCreator)
		{
			var lazy = new Lazy<TService>(instanceCreator);
			Register<TService>(() => lazy.Value);
		}

		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		public T Resolve<T>(Type serviceType)
		{
			return (T)Resolve(serviceType);
		}

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

			throw new InvalidOperationException("No registration for " + serviceType);
		}

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
			var ctor = implementationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
			if (ctor == null)
			{
				return Activator.CreateInstance(implementationType);
			}

			var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
			var dependencies = parameterTypes.Select(t => Resolve(t)).ToArray();

			return Activator.CreateInstance(implementationType, dependencies);
		}
	}
}
