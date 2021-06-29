using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Response object for the dashboard and console
	/// </summary>
	public interface IDashboardResponse
	{
		/// <summary>
		/// Gets or sets the ContentType of the <see cref="HttpResponse"/>
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// Gets or sets the statuscode of the <see cref="HttpResponse"/>
		/// </summary>
		int StatusCode { get; set; }

		/// <summary>
		/// Gets the Body of the <see cref="HttpResponse"/>
		/// </summary>
		Stream Body { get; }

		/// <summary>
		/// Writes a string to the <see cref="HttpResponse"/>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		Task WriteAsync(string text);
	}
}
