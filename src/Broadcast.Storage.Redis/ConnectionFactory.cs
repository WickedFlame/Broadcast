using StackExchange.Redis;

namespace Broadcast.Storage.Redis
{
	/// <summary>
	/// Factory delegate that for creating the <see cref="IConnectionMultiplexer"/>
	/// </summary>
	/// <param name="connectionString"></param>
	/// <returns></returns>
	public delegate IConnectionMultiplexer Factory(string connectionString);

	/// <summary>
	/// Factory that returns the <see cref="IConnectionMultiplexer"/>
	/// The default implementation simply delegates to <see cref="ConnectionMultiplexer.Connect(string, System.IO.TextWriter)"/>
	/// This is only used when connecting with a connectionString to the redis database
	/// </summary>
	public static class ConnectionFactory
	{
		/// <summary>
		/// Connect to the redis server
		/// </summary>
		public static Factory Connect { get; set; } = s => ConnectionMultiplexer.Connect(s);
	}
}
