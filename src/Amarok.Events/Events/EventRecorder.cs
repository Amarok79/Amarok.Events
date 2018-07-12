/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;


namespace Amarok.Events
{
	/// <summary>
	/// This static type provides factory methods for creating <see cref="EventRecorder{T}"/> instances.
	/// 
	/// This type is thread-safe.
	/// </summary>
	public static class EventRecorder
	{
		/// <summary>
		/// Returns a newly created event recorder for the given event.
		/// </summary>
		public static EventRecorder<T> From<T>(Event<T> @event)
		{
			return new EventRecorder<T>(@event);
		}

		/// <summary>
		/// Returns a newly created event recorder for the given event source.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public static EventRecorder<T> From<T>(EventSource<T> eventSource)
		{
			if (eventSource == null)
				throw new ArgumentNullException(nameof(eventSource));

			return new EventRecorder<T>(eventSource.Event);
		}
	}
}
