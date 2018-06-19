using System;


namespace Amarok.Events
{
	internal sealed class ActionSubscription<T> : Subscription<T>
	{
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

		public override void Dispose()
		{
			if (mBackReference != null && mBackReference.TryGetTarget(out var target))
				target.Dispose();
			else
				mSource.Remove(this);
		}

		public override void Invoke(T value)
		{
			mAction(value);
		}
	}
}
