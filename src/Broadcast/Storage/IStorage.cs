using System.Collections.Generic;

namespace Broadcast.Storage
{
	/// <summary>
	/// Interface for the storage
	/// </summary>
	public interface IStorage
	{
		/// <summary>
		/// Add a value to a list in the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void AddToList<T>(StorageKey key, T value);

		/// <summary>
		/// Get a list of values from the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		IEnumerable<T> GetList<T>(StorageKey key);

		/// <summary>
		/// Removes a range of items from the list
		/// </summary>
		/// <param name="key"></param>
		/// <param name="count"></param>
		void RemoveRangeFromList(StorageKey key, int count);

		/// <summary>
		/// Set a value to the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Set<T>(StorageKey key, T value);

		/// <summary>
		/// Gets a value from the storage
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get<T>(StorageKey key);

		/// <summary>
		/// Gets all keys that start with the given key value
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IEnumerable<string> GetKeys(StorageKey key);

		/// <summary>
		/// Delete the storage entry
		/// </summary>
		/// <param name="key"></param>
		void Delete(StorageKey key);
	}
}
