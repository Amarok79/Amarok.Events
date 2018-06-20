/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;


namespace Amarok.Events
{
	internal sealed class ActionSubscription<T> : Subscription<T>
	{
		// data
		private readonly EventSource<T> mSource;
		private readonly Action<T> mAction;
		private WeakReference<WeakSubscription<T>> mBackReference;


		public ActionSubscription(EventSource<T> source, Action<T> action)
		{
			mSource = source;
			mAction = action;
		}


		public void SetBackReference(WeakSubscription<T> subscription)
		{
			mBackReference = new WeakReference<WeakSubscription<T>>(subscription);
		}

		public override void Invoke(T value)
		{
			mAction(value);
		}

		public override void Dispose()
		{
			if (mBackReference != null)
			{
				if (mBackReference.TryGetTarget(out var target))
					target.Dispose();
			}
			else
			{
				mSource.Remove(this);
			}
		}
	}
}
