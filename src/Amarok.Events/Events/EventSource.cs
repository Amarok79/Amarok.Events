using System;
using System.Linq;


namespace Amarok.Events
{
	/// <summary>
	/// </summary>
	public static class EventSource
	{
		private readonly static EventSource<Exception> sUnobservedExceptionEventSource = new EventSource<Exception>();


		internal static EventSource<Exception> UnobservedExceptionEventSource => sUnobservedExceptionEventSource;

		/// <summary>
		/// </summary>
		public static Event<Exception> UnobservedException => sUnobservedExceptionEventSource.Event;


	}
}
