using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Broadcast.Dashboard.Dispatchers
{
	/// <summary>
	/// Dispatcher for embedded resources
	/// </summary>
	public class EmbeddedResourceDispatcher : IDashboardDispatcher
	{
		private readonly Assembly _assembly;
		private readonly string _resourceName;
		private readonly string _contentType;

		/// <summary>
		/// Create a new instance of the EmbeddedResourceDispatcher
		/// </summary>
		/// <param name="contentType"></param>
		/// <param name="assembly"></param>
		/// <param name="resourceName"></param>
		public EmbeddedResourceDispatcher(string contentType, Assembly assembly, string resourceName)
		{
			_resourceName = resourceName;
			_contentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
			_assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
		}

		/// <summary>
		/// Execute the dispatcher
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Dispatch(IDashboardContext context)
		{
			context.Response.ContentType = _contentType;
			//context.Response.SetExpire(DateTimeOffset.Now.AddYears(1));

			await WriteResponse(context.Response).ConfigureAwait(false);
		}

		protected virtual Task WriteResponse(DashboardResponse response)
		{
			return WriteResource(response, _assembly, _resourceName);
		}

		protected async Task WriteResource(DashboardResponse response, Assembly assembly, string resourceName)
		{
			using (var inputStream = assembly.GetManifestResourceStream(resourceName))
			{
				if (inputStream == null)
				{
					throw new ArgumentException($@"Resource with name {resourceName} not found in assembly {assembly}.");
				}

				await inputStream.CopyToAsync(response.Body).ConfigureAwait(false);
			}
		}
	}
}
