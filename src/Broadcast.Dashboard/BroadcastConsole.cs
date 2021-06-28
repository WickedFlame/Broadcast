using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace Broadcast.Dashboard
{
	public static class BroadcastConsole
	{
		public static HtmlString AppendConsoleIncludes()
		{
			var path = DashboardOptions.Default.RouteBasePath.EnsureTrailingSlash();
			var sb = new StringBuilder();

			sb.AppendLine($"<link rel=\"stylesheet\" href=\"{path}css/broadcast-console.min.css\"/>");
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.AppendLine("  var consoleConfig = {");
			sb.AppendLine($"    pollUrl: \"{path}dashboard/metrics\",");
			sb.AppendLine("    pollInterval: 2000");
			sb.AppendLine("  };");
			sb.AppendLine("</script>");
			sb.AppendLine($"<script type='module' async src=\"{path}js/broadcast-console.js\"></script>");

			return new HtmlString(sb.ToString());
		}
	}
}
