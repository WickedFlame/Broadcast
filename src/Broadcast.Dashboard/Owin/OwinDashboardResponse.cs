using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Owin;

namespace Broadcast.Dashboard.Owin
{
	public class OwinDashboardResponse : IDashboardResponse
	{
		private readonly IOwinContext _context;

		/// <summary>
		/// Creates a new instance 
		/// </summary>
		/// <param name="context"></param>
		public OwinDashboardResponse(OwinContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Gets or sets the ContentType of the <see cref="HttpResponse"/>
		/// </summary>
		public string ContentType
		{
			get => _context.Response.ContentType;
			set => _context.Response.ContentType = value;
		}

		/// <summary>
		/// Gets or sets the statuscode of the <see cref="HttpResponse"/>
		/// </summary>
		public int StatusCode
		{
			get => _context.Response.StatusCode;
			set => _context.Response.StatusCode = value;
		}

		/// <summary>
		/// Gets the Body of the <see cref="HttpResponse"/>
		/// </summary>
		public Stream Body => _context.Response.Body;

		/// <summary>
		/// Writes a string to the <see cref="HttpResponse"/>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public Task WriteAsync(string text)
		{
			return _context.Response.WriteAsync(text);
		}
	}
}
