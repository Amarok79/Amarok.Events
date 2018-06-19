using System;
using System.Threading.Tasks;


namespace Amarok.Events
{
	internal sealed class FuncSubscription<T> : Subscription<T>
	{
		private readonly EventSource<T> mSource;
		private readonly Func<T, Task> mFunc;


		public FuncSubscription(EventSource<T> source, Func<T, Task> func)
		{
			mSource = source;
			mFunc = func;
		}

		public override void Dispose()
		{
			mSource.Remove(this);
		}

		public override void Invoke(T value)
		{
			var task = mFunc(value);

			if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
				return;

			task.ContinueWith(
				x => { /* forward exception */ },
				TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted
			);
		}
	}
}
