namespace Broadcast.Storage.Inmemory
{
	/// <summary>
	/// Interface for a storageitem for the inmemory storage
	/// </summary>
	public interface IStorageItem
	{
		/// <summary>
		/// Set the value
		/// </summary>
		/// <param name="value"></param>
		void SetValue(object value);

		/// <summary>
		/// Get the value
		/// </summary>
		/// <returns></returns>
		object GetValue();
	}
}
