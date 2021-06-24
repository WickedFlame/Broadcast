using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	public class DashboardResponse
	{
		private readonly HttpContext _context;

		public DashboardResponse(HttpContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public string ContentType
		{
			get => _context.Response.ContentType;
			set => _context.Response.ContentType = value;
		}

		public int StatusCode
		{
			get => _context.Response.StatusCode;
			set => _context.Response.StatusCode = value;
		}

		public Stream Body => _context.Response.Body;

		public Task WriteAsync(string text)
		{
			return _context.Response.WriteAsync(text);
		}
	}
}
