using System.Text;
using Microsoft.AspNetCore.Html;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Helper methods for adding the console to the UI
	/// </summary>
	public static class BroadcastConsole
	{
		/// <summary>
		/// Append the HTML that loads the JS to add a console overlay to the UI
		/// </summary>
		/// <returns></returns>
		public static HtmlString AppendConsoleIncludes()
		{
			var path = DashboardOptions.Default.RouteBasePath.EnsureTrailingSlash();
			path = path.EnsureLeadingSlash()
				.EnsureTrailingSlash();

			var sb = new StringBuilder();

			sb.AppendLine($"<link rel=\"stylesheet\" href=\"{path}css/broadcast-console\"/>");
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.AppendLine("  var consoleConfig = {");
			sb.AppendLine($"    pollUrl: \"{path}dashboard/metrics\",");
			sb.AppendLine("    pollInterval: 2000");
			sb.AppendLine("  };");
			sb.AppendLine("</script>");
			sb.AppendLine($"<script type='module' async src=\"{path}js/broadcast-console\"></script>");

			return new HtmlString(sb.ToString());
		}
	}
}
