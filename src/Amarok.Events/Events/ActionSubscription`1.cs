/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// Implementation class that represents a subscription to a sync handler method.
	/// </summary>
	internal sealed class ActionSubscription<T> : Subscription<T>
	{
		/// <summary>
		/// a reference to the event source; necessary for disposal
		/// </summary>
		private readonly EventSource<T> mSource;

		/// <summary>
		/// a delegate to the handler method
		/// </summary>
		private readonly Action<T> mAction;

		/// <summary>
		/// an optional weak reference back to another subscription holding this subscription
		/// also via weak reference; necessary for automatic removal magic of weak subscriptions
		/// </summary>
		private WeakReference<Subscription<T>> mBackReference;


		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ActionSubscription(EventSource<T> source, Action<T> action)
		{
			mSource = source;
			mAction = action;
		}


		/// <summary>
		/// Invoked to establish a weak reference back to another subscription. Only called
		/// for weak subscriptions.
		/// </summary>
		public void SetBackReference(Subscription<T> subscription)
		{
			mBackReference = new WeakReference<Subscription<T>>(subscription);
		}

		/// <summary>
		/// Invokes the subscription's handler in a synchronous way.
		/// </summary>
		public override void Invoke(T value)
		{
			mAction(value);
		}

		/// <summary>
		/// Invokes the subscription's handler in an asynchronous way.
		/// </summary>
		public override ValueTask InvokeAsync(T value)
		{
			mAction(value);

			return new ValueTask(Task.CompletedTask);
		}

		/// <summary>
		/// Disposes the subscription; removes it from the event source.
		/// </summary>
		public override void Dispose()
		{
			if (mBackReference != null)
			{
				// dispose the previous subscription, if still reachable
				if (mBackReference.TryGetTarget(out var target))
					target.Dispose();
			}
			else
			{
				// remove ourself from event source
				mSource.Remove(this);
			}
		}

		/// <summary>
		/// Returns a string that represents the current instance.
		/// </summary>
		public override String ToString()
		{
			return $"=> {mAction.Method.DeclaringType.FullName}.{mAction.Method.Name}()";
		}


		internal Subscription<T> TestingGetBackReference()
		{
			if (mBackReference == null)
			{
				return null;
			}
			else
			{
				if (mBackReference.TryGetTarget(out var target))
					return target;
				else
					return null;
			}
		}
	}
}
