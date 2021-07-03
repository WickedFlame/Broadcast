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
		/// <param name="options"></param>
		/// <returns></returns>
		public static HtmlString AppendConsoleIncludes(ConsoleOptions options = null)
		{
			var path = DashboardOptions.Default.RouteBasePath.EnsureTrailingSlash();
			var sb = new StringBuilder();

			sb.AppendLine($"<link rel=\"stylesheet\" href=\"{path}css/broadcast-console\"/>");
			sb.AppendLine("<script type=\"text/javascript\">");
			sb.AppendLine("  var consoleConfig = {");
			sb.AppendLine($"    pollUrl: \"{path}dashboard/metrics\",");
			sb.AppendLine("    pollInterval: 2000,");
			if (options != null)
			{
				sb.AppendLine($"    position: \"{options.Position}\"");
			}
			sb.AppendLine("  };");
			sb.AppendLine("</script>");
			sb.AppendLine($"<script type='module' async src=\"{path}js/broadcast-console\"></script>");

			return new HtmlString(sb.ToString());
		}

		private static string FormatPositionStyle(ConsoleOptions options)
		{
			if (options == null)
			{
				return string.Empty;
			}

			switch (options.Position)
			{
				case ConsolePosition.TopLeft:
					return "top:0;right:auto;bottom:auto;left:0;";
				case ConsolePosition.TopRight:
					return "top:0;right:0;bottom:auto;left:auto;";
				case ConsolePosition.BottomRight:
					return "top:auto;right:0;bottom:0;left:auto;";
				case ConsolePosition.BottomLeft:
				default:
					return "top:auto;right:auto;bottom:0;left:0;";
			}
		}
	}
}
