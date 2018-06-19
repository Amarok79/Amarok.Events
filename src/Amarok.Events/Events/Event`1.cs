/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Event<T> : 
		IEquatable<Event<T>>
	{
		// data
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly EventSource<T> mEventSource;


		#region ++ Public Interface ++

		/// <summary>
		/// </summary>
		public EventSource<T> Source => mEventSource;

		// TODO: HasSource, IsSourceConnected, ?!?!



		public Event(EventSource<T> eventSource)
		{
			mEventSource = eventSource;
		}



		public IDisposable Subscribe(Action<T> action)
		{
			if (mEventSource == null)
				return null;

			return mEventSource.Add(action);
		}

		public IDisposable Subscribe(Func<T, Task> func)
		{
			if (mEventSource == null)
				return null;

			return mEventSource.Add(func);
		}

		public IDisposable SubscribeWeak(Action<T> action)
		{
			if (mEventSource == null)
				return null;

			return mEventSource.AddWeak(action);
		}

		//public IDisposable SubscribeWeak(Func<T, Task> func)
		//{
		//	return null;
		//}



		/// <summary>
		/// Returns a string that represents the current instance.
		/// </summary>
		/// 
		/// <returns>
		/// A string that represents the current instance.</returns>
		public override String ToString()
		{
			return null;
		}



		// TODO: Subscribe -> IProgress<T>, ...

		#endregion

		#region ++ Public Interface (Equality) ++

		/// <summary>
		/// Returns the hash code for the current instance. 
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.</returns>
		public override Int32 GetHashCode()
		{
			return 0;
		}


		/// <summary>
		/// Determines whether the specified instance is equal to the current instance.
		/// </summary>
		/// 
		/// <param name="other">
		/// The instance to compare with the current instance.</param>
		/// 
		/// <returns>
		/// True, if the specified instance is equal to the current instance; otherwise, False.</returns>
		public override Boolean Equals(Object other)
		{
			return true;
		}

		/// <summary>
		/// Determines whether the specified instance is equal to the current instance.
		/// </summary>
		/// 
		/// <param name="other">
		/// The instance to compare with the current instance.</param>
		/// 
		/// <returns>
		/// True, if the specified instance is equal to the current instance; otherwise, False.</returns>
		public Boolean Equals(Event<T> other)
		{
			return true;
		}


		/// <summary>
		/// Determines whether the specified instances are equal.
		/// </summary>
		/// 
		/// <param name="left">
		/// The first instance to compare.</param>
		/// <param name="right">
		/// The second instance to compare.</param>
		/// <returns>
		/// True, if the specified instances are equal; otherwise, False.</returns>
		public static Boolean operator ==(Event<T> left, Event<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Determines whether the specified instances are unequal.
		/// </summary>
		/// 
		/// <param name="left">
		/// The first instance to compare.</param>
		/// <param name="right">
		/// The second instance to compare.</param>
		/// <returns>
		/// True, if the specified instances are unequal; otherwise, False.</returns>
		public static Boolean operator !=(Event<T> left, Event<T> right)
		{
			return !left.Equals(right);
		}

		#endregion







	}
}
