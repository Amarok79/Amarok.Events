/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// This type represents an Event that allows consumers to subscribe on.
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Event<T> :
		IEquatable<Event<T>>
	{
		// a reference to the owning event source; can be null
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly EventSource<T> mSource;


		#region ++ Public Interface ++

		/// <summary>
		/// Gets a reference to the owning <see cref="EventSource{T}"/>, or null if this <see cref="Event{T}"/>
		/// isn't associated with an <see cref="EventSource{T}"/>. See also <see cref="HasSource"/>.
		/// </summary>
		public EventSource<T> Source => mSource;

		/// <summary>
		/// Gets a boolean value indicating whether this <see cref="Event{T}"/> is associated with an 
		/// <see cref="EventSource{T}"/>. See also <see cref="Source"/>.
		/// </summary>
		public Boolean HasSource => mSource != null;


		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		internal Event(EventSource<T> eventSource)
		{
			mSource = eventSource;
		}


		/// <summary>
		/// Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
		/// 
		/// This method establishes a strong reference between the event source and the object holding the supplied
		/// callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep 
		/// the subscriber in memory. To break this strong reference, you can dispose the returned subscription.
		/// </summary>
		/// 
		/// <param name="action">
		/// The callback to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the callback from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable Subscribe(Action<T> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if (mSource == null)
				return NullSubscription.Instance;

			return mSource.Add(action);
		}

		/// <summary>
		/// Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
		/// 
		/// This method establishes a weak reference between the event source and the object holding the supplied
		/// callback, aka subscriber. That means that the subscription is kept alive only as long as both event source 
		/// and subscriber are kept in memory via strong references from other objects. The event source alone doesn't 
		/// keep the subscriber in memory. You have to keep a strong reference to the returned subscription object to
		/// achieve this.
		/// 
		/// The subscription can be canceled at any time by disposing the returned subscription object. Otherwise, 
		/// the subscription is automatically canceled if the subscriber is being garbage collected. For this to 
		/// happen no other strong reference to the returned subscription must exist.
		/// </summary>
		/// 
		/// <param name="action">
		/// The callback to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the callback from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable SubscribeWeak(Action<T> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));
			if (mSource == null)
				return NullSubscription.Instance;

			return mSource.AddWeak(action);
		}

		/// <summary>
		/// Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
		/// 
		/// This method establishes a strong reference between the event source and the object holding the supplied
		/// callback, aka subscriber. That means as long as the event source is kept in memory, it will also keep 
		/// the subscriber in memory. To break this strong reference, you can dispose the returned subscription.
		/// </summary>
		/// 
		/// <param name="func">
		/// The callback to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the callback from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable Subscribe(Func<T, Task> func)
		{
			if (func == null)
				throw new ArgumentNullException(nameof(func));
			if (mSource == null)
				return NullSubscription.Instance;

			return mSource.Add(func);
		}

		/// <summary>
		/// Subscribes the given callback on the event. The callback will be invoked every time the event is raised.
		/// 
		/// This method establishes a weak reference between the event source and the object holding the supplied
		/// callback, aka subscriber. That means that the subscription is kept alive only as long as both event source 
		/// and subscriber are kept in memory via strong references from other objects. The event source alone doesn't 
		/// keep the subscriber in memory. You have to keep a strong reference to the returned subscription object to
		/// achieve this.
		/// 
		/// The subscription can be canceled at any time by disposing the returned subscription object. Otherwise, 
		/// the subscription is automatically canceled if the subscriber is being garbage collected. For this to 
		/// happen no other strong reference to the returned subscription must exist.
		/// </summary>
		/// 
		/// <param name="func">
		/// The callback to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the callback from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable SubscribeWeak(Func<T, Task> func)
		{
			if (func == null)
				throw new ArgumentNullException(nameof(func));
			if (mSource == null)
				return NullSubscription.Instance;

			return mSource.AddWeak(func);
		}

		/// <summary>
		/// Subscribes the given progress object on the event. The progress object will be invoked every time the 
		/// event is raised.
		/// 
		/// This method establishes a strong reference between the event source and the progress object. That means 
		/// as long as the event source is kept in memory, it will also keep the progress object in memory. To break 
		/// this strong reference, you can dispose the returned subscription.
		/// </summary>
		/// 
		/// <param name="progress">
		/// The progress object to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the progress object from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable Subscribe(IProgress<T> progress)
		{
			return this.Subscribe(x => progress.Report(x));
		}

		/// <summary>
		/// Subscribes the given progress object on the event. The progress object will be invoked every time the 
		/// event is raised.
		/// 
		/// This method establishes a strong reference between the event source and the progress object. That means 
		/// as long as the event source is kept in memory, it will also keep the progress object in memory. To break 
		/// this strong reference, you can dispose the returned subscription.
		/// </summary>
		/// 
		/// <param name="progress">
		/// The progress object to subscribe on the event.</param>
		/// 
		/// <returns>
		/// An object that represents the newly created subscription. Disposing this object will cancel the 
		/// subscription and remove the progress object from the event source's subscription list.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public IDisposable SubscribeWeak(IProgress<T> progress)
		{
			return this.SubscribeWeak(x => progress.Report(x));
		}


		/// <summary>
		/// Returns a string that represents the current instance.
		/// </summary>
		public override String ToString()
		{
			if (this.HasSource)
				return $"Event<{typeof(T).Name}> :=: {mSource}";
			else
				return $"Event<{typeof(T).Name}> :=: <null>";
		}

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
			return mSource?.GetHashCode() ?? 0;
		}


		/// <summary>
		/// Determines whether the specified instance is equal to the current instance.
		/// </summary>
		/// 
		/// <param name="obj">
		/// The instance to compare with the current instance.</param>
		/// 
		/// <returns>
		/// True, if the specified instance is equal to the current instance; otherwise, False.</returns>
		public override Boolean Equals(Object obj)
		{
			return obj is Event<T> && Equals((Event<T>)obj);
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
			return Object.ReferenceEquals(mSource, other.mSource);
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
