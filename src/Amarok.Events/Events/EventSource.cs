namespace Amarok.Events
{
	/// <summary>
	/// </summary>
	public static class EventSource
	{
		/// <summary>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static EventSource<T> Create<T>()
		{
			return new EventSource<T>();
		}
	}
}
