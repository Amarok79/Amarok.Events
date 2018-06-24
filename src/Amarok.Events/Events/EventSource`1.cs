﻿/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

#pragma warning disable S1659 // Multiple variables should not be declared on the same line
#pragma warning disable S2221 // "Exception" should not be caught when not required by called methods
#pragma warning disable S4017 // Method signatures should not contain nested generic types

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// This type represents an Event that allows publishers to raise it.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class EventSource<T> :
		IDisposable
	{
		/// <summary>
		/// the associated public-facing event
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Event<T> mEvent;

		/// <summary>
		/// the list of subscriptions
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ImmutableArray<Subscription<T>> mSubscriptions = ImmutableArray<Subscription<T>>.Empty;

		/// <summary>
		/// sync object for disposal
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Object mSyncDispose = new Object();

		/// <summary>
		/// state of disposal
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private volatile Boolean mIsDisposed;

		/// <summary>
		/// number of threads still executing
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Int32 mNumberOfThreadsExecuting;


		#region ++ IDisposable Interface ++

		/// <summary>
		/// Disposes this <see cref="EventSource{T}"/>.
		/// 
		/// After disposal raising the event has no effect anymore.
		/// 
		/// The list of active subscriptions is canceled, releasing all references to subscribers; new 
		/// subscriptions are not accepted anymore.
		/// </summary>
		public void Dispose()
		{
			// ensure that we enter the subsequent code block only once
			lock (mSyncDispose)
			{
				if (mIsDisposed)
					return;

				mIsDisposed = true;
			}

			// wait for other threads to complete execution of local code
			SpinWait.SpinUntil(() =>
			{
				Int32 result = -1;
				Interlocked.Exchange(ref result, mNumberOfThreadsExecuting);
				return result == 0;
			});

			// cancel subscriptions
			var subscriptions = mSubscriptions;
			for (Int32 i = 0; i < subscriptions.Length; i++)
				subscriptions[i].Dispose();

			// clear subscription list
			ImmutableArray<Subscription<T>> initial, computed;
			do
			{
				initial = mSubscriptions;
				computed = initial.Clear();
			}
			while (initial != ImmutableInterlocked.InterlockedCompareExchange(
				ref mSubscriptions, computed, initial));
		}

		#endregion

		#region ++ Public Interface ++

		/// <summary>
		/// Gets the public-facing Event that allows consumers to subscribe to.
		/// </summary>
		public Event<T> Event => mEvent;

		/// <summary>
		/// Gets the current number of subscriptions. This information can be slightly out-of-date in
		/// multi-threading scenarios and is intended for diagnosis purposes only.
		/// </summary>
		public Int32 NumberOfSubscriptions => mSubscriptions.Length;

		/// <summary>
		/// Gets a boolean value indicating whether this object has been disposed.
		/// </summary>
		public Boolean IsDisposed => mIsDisposed;


		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public EventSource()
		{
			mEvent = new Event<T>(this);
		}


		/// <summary>
		/// Raises the event with the supplied event value. All subscribers are being invoked and the supplied event 
		/// value is forwarded to them. If no subscribers are registered or the event source has already been disposed, 
		/// then the event value is ignored and False is returned immediately.
		/// 
		/// Exceptions thrown by subscribers are caught and forwarded to <see cref="EventSource.UnobservedException"/>.
		/// Regardless of thrown exceptions all subscribers are being invoked.
		/// 
		/// This method invokes all subscribers synchronously in a blocking fashion, meaning the calling thread raising
		/// the event invokes all subscribers directly. The calling thread returns after all subscribers have been 
		/// executed, except for asynchronous subscribers that run only to their first await statement. That means this 
		/// method potentially returns before asynchronous subscribers have been completed. To await their completion 
		/// use <see cref="InvokeAsync(T)"/> instead.
		/// </summary>
		/// 
		/// <param name="value">
		/// The event value to forward to the subscribers.</param>
		/// 
		/// <returns>
		/// A boolean value indicating whether at least a single subscriber has been called.
		/// </returns>
		public Boolean Invoke(T value)
		{
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			_InvokeCore(subscriptions, value);

			return true;
		}

		/// <summary>
		/// Raises the event with the supplied event value. All subscribers are being invoked and the supplied event 
		/// value is forwarded to them. If no subscribers are registered or the event source has already been disposed, 
		/// then the event value is ignored and False is returned immediately.
		/// 
		/// Exceptions thrown by subscribers are caught and forwarded to <see cref="EventSource.UnobservedException"/>.
		/// Regardless of thrown exceptions all subscribers are being invoked.
		/// 
		/// This method invokes all subscribers synchronously in a blocking fashion, meaning the calling thread raising
		/// the event invokes all subscribers directly. The calling thread returns after all subscribers have been 
		/// executed, except for asynchronous subscribers that run only to their first await statement. That means this 
		/// method potentially returns before asynchronous subscribers have been completed. To await their completion 
		/// use <see cref="InvokeAsync(T)"/> instead.
		/// </summary>
		/// 
		/// <param name="valueFactory">
		/// A value factory to determine the event value to forward to the subscribers. The factory is only
		/// called if at least a single subscriber is registered, preventing potentially costly processing.</param>
		/// 
		/// <returns>
		/// A boolean value indicating whether at least a single subscriber has been called.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public Boolean Invoke(Func<T> valueFactory)
		{
			if (valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			var value = valueFactory();
			_InvokeCore(subscriptions, value);

			return true;
		}

		/// <summary>
		/// Raises the event with the supplied event value. All subscribers are being invoked and the supplied event 
		/// value is forwarded to them. If no subscribers are registered or the event source has already been disposed, 
		/// then the event value is ignored and False is returned immediately.
		/// 
		/// Exceptions thrown by subscribers are caught and forwarded to <see cref="EventSource.UnobservedException"/>.
		/// Regardless of thrown exceptions all subscribers are being invoked.
		/// 
		/// This method invokes all subscribers synchronously in a blocking fashion, meaning the calling thread raising
		/// the event invokes all subscribers directly. The calling thread returns after all subscribers have been 
		/// executed, except for asynchronous subscribers that run only to their first await statement. That means this 
		/// method potentially returns before asynchronous subscribers have been completed. To await their completion 
		/// use <see cref="InvokeAsync(T)"/> instead.
		/// </summary>
		/// 
		/// <param name="valueFactory">
		/// A value factory to determine the event value to forward to the subscribers. The factory is only
		/// called if at least a single subscriber is registered, preventing potentially costly processing.</param>
		/// <param name="arg">
		/// An argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// 
		/// <returns>
		/// A boolean value indicating whether at least a single subscriber has been called.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public Boolean Invoke<TArg>(Func<TArg, T> valueFactory, TArg arg)
		{
			if (valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			var value = valueFactory(arg);
			_InvokeCore(subscriptions, value);

			return true;
		}

		/// <summary>
		/// Raises the event with the supplied event value. All subscribers are being invoked and the supplied event 
		/// value is forwarded to them. If no subscribers are registered or the event source has already been disposed, 
		/// then the event value is ignored and False is returned immediately.
		/// 
		/// Exceptions thrown by subscribers are caught and forwarded to <see cref="EventSource.UnobservedException"/>.
		/// Regardless of thrown exceptions all subscribers are being invoked.
		/// 
		/// This method invokes all subscribers synchronously in a blocking fashion, meaning the calling thread raising
		/// the event invokes all subscribers directly. The calling thread returns after all subscribers have been 
		/// executed, except for asynchronous subscribers that run only to their first await statement. That means this 
		/// method potentially returns before asynchronous subscribers have been completed. To await their completion 
		/// use <see cref="InvokeAsync(T)"/> instead.
		/// </summary>
		/// 
		/// <param name="valueFactory">
		/// A value factory to determine the event value to forward to the subscribers. The factory is only
		/// called if at least a single subscriber is registered, preventing potentially costly processing.</param>
		/// <param name="arg1">
		/// An argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// <param name="arg2">
		/// A second argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// 
		/// <returns>
		/// A boolean value indicating whether at least a single subscriber has been called.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public Boolean Invoke<TArg1, TArg2>(Func<TArg1, TArg2, T> valueFactory, TArg1 arg1, TArg2 arg2)
		{
			if (valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			var value = valueFactory(arg1, arg2);
			_InvokeCore(subscriptions, value);

			return true;
		}

		/// <summary>
		/// Raises the event with the supplied event value. All subscribers are being invoked and the supplied event 
		/// value is forwarded to them. If no subscribers are registered or the event source has already been disposed, 
		/// then the event value is ignored and False is returned immediately.
		/// 
		/// Exceptions thrown by subscribers are caught and forwarded to <see cref="EventSource.UnobservedException"/>.
		/// Regardless of thrown exceptions all subscribers are being invoked.
		/// 
		/// This method invokes all subscribers synchronously in a blocking fashion, meaning the calling thread raising
		/// the event invokes all subscribers directly. The calling thread returns after all subscribers have been 
		/// executed, except for asynchronous subscribers that run only to their first await statement. That means this 
		/// method potentially returns before asynchronous subscribers have been completed. To await their completion 
		/// use <see cref="InvokeAsync(T)"/> instead.
		/// </summary>
		/// 
		/// <param name="valueFactory">
		/// A value factory to determine the event value to forward to the subscribers. The factory is only
		/// called if at least a single subscriber is registered, preventing potentially costly processing.</param>
		/// <param name="arg1">
		/// An argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// <param name="arg2">
		/// A second argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// <param name="arg3">
		/// A third argument that is supplied to the given value factory. Useful to prevent closure allocations.</param>
		/// 
		/// <returns>
		/// A boolean value indicating whether at least a single subscriber has been called.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// A null reference was passed to a method that did not accept it as a valid argument.</exception>
		public Boolean Invoke<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, T> valueFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			if (valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			var value = valueFactory(arg1, arg2, arg3);
			_InvokeCore(subscriptions, value);

			return true;
		}

		private static void _InvokeCore(ImmutableArray<Subscription<T>> subscriptions, T value)
		{
			for (Int32 i = 0; i < subscriptions.Length; i++)
			{
				try
				{
					subscriptions[i].Invoke(value);
				}
				catch (Exception exception)
				{
					EventSource.UnobservedExceptionEventSource.Invoke(exception);
				}
			}
		}







		public ValueTask<Boolean> InvokeAsync(T value)
		{
			if (mIsDisposed)
				return TaskUtils.FalseValueTask;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return TaskUtils.FalseValueTask;

			return _InvokeAsyncCore(subscriptions, value);
		}

		private static ValueTask<Boolean> _InvokeAsyncCore(ImmutableArray<Subscription<T>> subscriptions, T value)
		{
			List<Task> tasks = null;

			for (Int32 i = 0; i < subscriptions.Length; i++)
			{
				try
				{
					var valueTask = subscriptions[i].InvokeAsync(value);

					if (valueTask.IsCompletedSuccessfully)
						continue;
					if (valueTask.IsCompleted)
						EventSource.UnobservedExceptionEventSource.Invoke(valueTask.AsTask().Exception);

					if (tasks == null)
						tasks = new List<Task>(subscriptions.Length);

					tasks.Add(valueTask.AsTask());
				}
				catch (Exception exception)
				{
					EventSource.UnobservedExceptionEventSource.Invoke(exception);
				}
			}

			var taskCompletionSource = new TaskCompletionSource<Boolean>();

			Task.WhenAll(tasks)
				.ContinueWith(x =>
				{
					if (x.IsFaulted)
					{
						for (Int32 i = 0; i < x.Exception.InnerExceptions.Count; i++)
							EventSource.UnobservedExceptionEventSource.Invoke(x.Exception.InnerExceptions[i]);
					}

					taskCompletionSource.SetResult(true);
				},
				TaskContinuationOptions.ExecuteSynchronously);

			return new ValueTask<Boolean>(taskCompletionSource.Task);
		}

		public ValueTask<Boolean> InvokeAsync(Func<T> valueFactory)
		{
			return new ValueTask<Boolean>();
		}

		public ValueTask<Boolean> InvokeAsync<TArg>(Func<TArg, T> valueFactory, TArg arg)
		{
			return new ValueTask<Boolean>();
		}

		public ValueTask<Boolean> InvokeAsync<TArg1, TArg2>(Func<TArg1, TArg2, T> valueFactory, TArg1 arg1, TArg2 arg2)
		{
			return new ValueTask<Boolean>();
		}

		public ValueTask<Boolean> InvokeAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, T> valueFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			return new ValueTask<Boolean>();
		}



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


		#endregion

		#region ~~ Internal Interface ~~

		internal IDisposable Add(Action<T> action)
		{
			if (mIsDisposed)
				return NullSubscription.Instance;

			var subscription = new ActionSubscription<T>(this, action);

			_AddCore(subscription);

			return subscription;
		}

		internal IDisposable AddWeak(Action<T> action)
		{
			if (mIsDisposed)
				return NullSubscription.Instance;

			var strongSubscription = new ActionSubscription<T>(this, action);
			var weakSubscription = new WeakSubscription<T>(this, strongSubscription);
			strongSubscription.SetPreviousSubscription(weakSubscription);

			_AddCore(weakSubscription);

			return strongSubscription;
		}

		private void _AddCore(Subscription<T> subscription)
		{
			try
			{
				Interlocked.Increment(ref mNumberOfThreadsExecuting);

				ImmutableArray<Subscription<T>> initial, computed;
				do
				{
					initial = mSubscriptions;
					computed = initial.Add(subscription);
				}
				while (initial != ImmutableInterlocked.InterlockedCompareExchange(
					ref mSubscriptions, computed, initial));
			}
			finally
			{
				Interlocked.Decrement(ref mNumberOfThreadsExecuting);
			}
		}




		internal IDisposable Add(Func<T, Task> func)
		{
			var subscription = new FuncSubscription<T>(this, func);

			if (mIsDisposed)
				return null;

			try
			{
				Interlocked.Increment(ref mNumberOfThreadsExecuting);

				ImmutableArray<Subscription<T>> initial, computed;
				do
				{
					initial = mSubscriptions;
					computed = initial.Add(subscription);
				}
				while (initial != ImmutableInterlocked.InterlockedCompareExchange(
					ref mSubscriptions, computed, initial));

				return subscription;
			}
			finally
			{
				Interlocked.Decrement(ref mNumberOfThreadsExecuting);
			}
		}

		internal IDisposable AddWeak(Func<T, Task> func)
		{
			return null;
		}


		internal void Remove(Subscription<T> subscription)
		{
			try
			{
				Interlocked.Increment(ref mNumberOfThreadsExecuting);

				ImmutableArray<Subscription<T>> initial, computed;
				do
				{
					initial = mSubscriptions;
					computed = initial.Remove(subscription);
				}
				while (initial != ImmutableInterlocked.InterlockedCompareExchange(
					ref mSubscriptions, computed, initial));
			}
			finally
			{
				Interlocked.Decrement(ref mNumberOfThreadsExecuting);
			}
		}

		#endregion
	}
}