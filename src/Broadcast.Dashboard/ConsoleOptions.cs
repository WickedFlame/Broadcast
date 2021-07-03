
namespace Broadcast.Dashboard
{
	/// <summary>
	/// Enumeration that defines the position of the console in the UI
	/// </summary>
	public enum ConsolePosition
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	/// <summary>
	/// Options for the console
	/// </summary>
	public class ConsoleOptions
	{
		/// <summary>
		/// Gets the position of the console in the UI
		/// </summary>
		public ConsolePosition Position { get; set; } = ConsolePosition.TopRight;
	}
}
