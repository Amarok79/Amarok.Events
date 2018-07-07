## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as replacement for ordinary .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. In addition, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events in regard to call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/) (currently in beta)

The package provides binaries for *.NET Standard 2.0* only. Tests are performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.

For development, you need *Visual Studio 2017* (v15.7 or later).

## Documentation

### Event Source and Event

Suppose you have an interface and you want to expose an event on that interface. You do that as following:

    public interface IFooService
    {
	    Event<Int32> Progress { get; }
    }
	
The event is declared as *getter-only property* of type **Event\<T>**, where **T** represents the type of event argument. This can be any type.

The implementation class  of that interface then initializes a field of type **EventSource\<T>** and implements the getter-only event property.

    internal sealed class FooServiceImpl : IFooService
    {
    	private readonly EventSource<Int32> mProgressEventSource = new EventSource<Int32>();
    
    	public Event<Int32> Progress => mProgressEventSource.Event;
    
    	public void DoSomething()
    	{
    		// raises the event
    		mProgressEventSource.Invoke(50);
    	}
    }

In general, the *event source* should be kept private, while the associated **Event\<T>** is made public.

For raising the event, one simply calls **Invoke(**..**)** on the *event source*. Here you supply the event argument that is forwarded to all event handler.

Next, a consumer of the service can subscribe to the event, as following:

	FooServiceImpl serviceImpl = new FooServiceImpl();
	IFooService service = serviceImpl;

	IDisposable subscription = service.Progress.Subscribe(x => {
		Console.WriteLine(x + "%");
	});

	serviceImpl.DoSomething();		// internally raises the event
	// console output:	50%

The object returned from **Subscribe(**..**)** can be used to cancel the subscription at any time.

    subscription.Dispose();
    
    serviceImpl.DoSomething();
    // does nothing, since no subscribers are registered anymore

It is recommended that subscriber store these subscription objects somewhere, otherwise they won't be able to remove their registered event handlers.

If instead the class exposing the event wants to cancel all subscriptions, for example, because it gets disposed, it can simply dispose the *event source* too, which automatically cancels all subscriptions and ignores further calls to **Invoke(**..**)**.

    internal sealed class FooServiceImpl :
    	IFooService
    {
	    ...
	
		public void Dispose()
		{
			mProgressEventSource.Dispose();
			// cancels all subscriptions, discards new subscriptions and
			// ignores any call to Invoke()
		}
	}


<!--stackedit_data:
eyJoaXN0b3J5IjpbLTc5NTc0MzI0OSwxODE3NTg3OTVdfQ==
-->