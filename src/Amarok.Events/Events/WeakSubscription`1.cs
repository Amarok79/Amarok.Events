using System;


namespace Amarok.Events
{
	internal sealed class WeakSubscription<T> : Subscription<T>
	{
		private readonly EventSource<T> mSource;
		private readonly WeakReference<Subscription<T>> mWeakReference;


		public WeakSubscription(EventSource<T> source, Subscription<T> subscription)
		{
			mSource = source;
			mWeakReference = new WeakReference<Subscription<T>>(subscription);
		}


		public override void Dispose()
		{
			if (mWeakReference.TryGetTarget(out var target))
				target.Dispose();
			else
				mSource.Remove(this);
		}

		public override void Invoke(T value)
		{
			if (mWeakReference.TryGetTarget(out var target))
				target.Invoke(value);
			else
				mSource.Remove(this);
		}
	}
}
