/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Amarok.Events
{
	/// <summary>
	/// </summary>
	internal sealed class Subscription<T> :
		IDisposable
	{
		private readonly EventSource<T> mSource;
		private readonly Action<T> mAction;
		private readonly Func<T, Task> mFunc;
		private readonly WeakReference<Subscription<T>> mWeakReference;
		private WeakReference<Subscription<T>> mBackReference;


		public Subscription(EventSource<T> source, Action<T> action)
		{
			mSource = source;
			mAction = action;
		}

		public Subscription(EventSource<T> source, Subscription<T> subscription)
		{
			mSource = source;
			mWeakReference = new WeakReference<Subscription<T>>(subscription);
		}

		public void SetBackReference(Subscription<T> backReference)
		{
			mBackReference = new WeakReference<Subscription<T>>(backReference, false);
		}


		public void Dispose()
		{
			if (mBackReference != null)
			{
				if (mBackReference.TryGetTarget(out Subscription<T> backReference))
					backReference.Dispose();
			}
			else
			{
				mSource.Remove(this);
			}
		}

		public void Invoke(T value)
		{
			if (mAction != null)
				mAction(value);
			else
			if (mFunc != null)
				_CallFireAndForget(value);
			else
				_CallWeaklyHeldSubscription(value);
		}

		private void _CallFireAndForget(T value)
		{

		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void _CallWeaklyHeldSubscription(T value)
		{
			if (mWeakReference.TryGetTarget(out Subscription<T> subscription))
				subscription.Invoke(value);
			else
				mSource.Remove(this);
		}
	}
}
