using System;


namespace Amarok.Events
{
	internal abstract class Subscription<T> :
		IDisposable
	{
		public abstract void Invoke(T value);

		public abstract void Dispose();
	}
}
