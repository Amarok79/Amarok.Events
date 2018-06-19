/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Amarok.Events
{
	/// <summary>
	/// </summary>
	[DebuggerStepThrough]
	public sealed class EventSource<T> :
		IDisposable
	{
		// data
		private readonly Event<T> mEvent;

		private Boolean mIsDisposed;

		private Int32 mNumberOfPendingCalls;

		private ImmutableArray<Subscription<T>> mSubscriptions = ImmutableArray<Subscription<T>>.Empty;





		#region ++ IDisposable Interface ++

		public void Dispose()
		{

		}

		#endregion

		#region ++ Public Interface ++

		public Event<T> Event => mEvent;

		public Int32 NumberOfSubscriptions => 0;

		public Boolean IsDisposed => false;



		public EventSource()
		{
			mEvent = new Event<T>(this);
		}



		public Boolean Invoke(T value)
		{
			if (mIsDisposed)
				return false;

			var subscriptions = mSubscriptions;

			if (subscriptions.Length == 0)
				return false;

			_ReportImpl(subscriptions, value);

			return true;
		}

		public Boolean Invoke(Func<T> valueFactory)
		{
			return true;
		}

		public Boolean Invoke<TArg>(Func<TArg, T> valueFactory, TArg arg)
		{
			return true;
		}

		public Boolean Invoke<TArg1, TArg2>(Func<TArg1, TArg2, T> valueFactory, TArg1 arg1, TArg2 arg2)
		{
			return true;
		}

		public Boolean Invoke<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, T> valueFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			return true;
		}


		private static void _ReportImpl(ImmutableArray<Subscription<T>> subscriptions, T value)
		{
			for (Int32 i = 0; i < subscriptions.Length; i++)
			{
				try
				{
					subscriptions[i].Invoke(value);
				}
				catch (Exception exception)
				{
				}
			}
		}


		public Task<Boolean> InvokeAsync(T value)
		{
			return null;
		}

		public Task<Boolean> InvokeAsync(Func<T> valueFactory)
		{
			return null;
		}

		public Task<Boolean> InvokeAsync<TArg>(Func<TArg, T> valueFactory, TArg arg)
		{
			return null;
		}

		public Task<Boolean> InvokeAsync<TArg1, TArg2>(Func<TArg1, TArg2, T> valueFactory, TArg1 arg1, TArg2 arg2)
		{
			return null;
		}

		public Task<Boolean> InvokeAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, T> valueFactory, TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			return null;
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
			var subscription = new ActionSubscription<T>(this, action);

			if (mIsDisposed)
				return null;

			try
			{
				Interlocked.Increment(ref mNumberOfPendingCalls);

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
				Interlocked.Decrement(ref mNumberOfPendingCalls);
			}
		}

		internal IDisposable Add(Func<T, Task> func)
		{
			var subscription = new FuncSubscription<T>(this, func);

			if (mIsDisposed)
				return null;

			try
			{
				Interlocked.Increment(ref mNumberOfPendingCalls);

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
				Interlocked.Decrement(ref mNumberOfPendingCalls);
			}
		}

		internal IDisposable AddWeak(Action<T> action)
		{
			var strongSubscription = new ActionSubscription<T>(this, action);

			var weakSubscription = new WeakSubscription<T>(this, strongSubscription);

			strongSubscription.SetBackReference(weakSubscription);

			if (mIsDisposed)
				return null;

			try
			{
				Interlocked.Increment(ref mNumberOfPendingCalls);

				ImmutableArray<Subscription<T>> initial, computed;
				do
				{
					initial = mSubscriptions;
					computed = initial.Add(weakSubscription);
				}
				while (initial != ImmutableInterlocked.InterlockedCompareExchange(
					ref mSubscriptions, computed, initial));

				return strongSubscription;
			}
			finally
			{
				Interlocked.Decrement(ref mNumberOfPendingCalls);
			}
		}

		internal IDisposable AddWeak(Func<T, Task> func)
		{
			return null;
		}

		internal void Remove(Subscription<T> subscription)
		{
		}

		#endregion


		#region Implementation



		#endregion








	}
}
