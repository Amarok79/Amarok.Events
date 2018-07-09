## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as replacement for ordinary .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. In addition, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events in regard to call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/) (currently in beta)

The package provides binaries for *.NET Standard 2.0* only. Tests are performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.

For development, you need *Visual Studio 2017* (v15.7 or later).


## Documentation

### Event Source and Event

Suppose you have an interface and you want to expose an event on that interface. You do that as following:

```cs
public interface IFooService
{
	Event<Int32> Progress { get; }
}
```

The event is declared as *getter-only property* of type **Event\<T>**, where **T** represents the type of event argument. This can be any type.

The implementation class  of that interface then initializes a field of type **EventSource\<T>** and implements the getter-only event property.

```cs
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
```

In general, the *event source* should be kept private, while the associated **Event\<T>** is made public. This is similar to the pattern used for *CancellationToken* and *CancellationTokenSource*, or *Task\<T>* and *TaskCompletionSource\<T>*.

For raising the event, one simply calls **Invoke(**..**)** on the *event source*. Here you supply the event argument that is forwarded to all event handlers.

Next, a consumer of the service can subscribe to the event. It just have to call **Subscribe(**..**)** on the *event* that is made public by the service.

```cs
FooServiceImpl serviceImpl = new FooServiceImpl();
IFooService service = serviceImpl;

IDisposable subscription = service.Progress.Subscribe(x => {
	Console.WriteLine(x + "%");
});

serviceImpl.DoSomething();		// internally raises the event
// console output:	50%
```

The object returned from **Subscribe(**..**)** can be used to cancel the subscription at any time.

```cs
subscription.Dispose();
    
serviceImpl.DoSomething();
// does nothing, since no subscribers are registered anymore
```

It is recommended that subscribers store these subscription objects somewhere, otherwise they won't be able to remove their registered event handlers.

If instead the class exposing the event wants to cancel all subscriptions, for example, because it gets disposed, it can simply dispose the *event source* too, which automatically cancels all subscriptions and ignores further calls to **Invoke(**..**)**.

```cs
internal sealed class FooServiceImpl : IFooService
{
	...
	
	public void Dispose()
	{
		mProgressEventSource.Dispose();
		// cancels all subscriptions, discards new subscriptions and
		// ignores any call to Invoke()
	}
}
```

### Invoke with Synchronous Event Handler

The following code snippet shows a single *event source* with two event handlers. Both event handler and also the code invoking the event print to the console. What`s the console output?

```cs
var source = new EventSource<String>();

source.Event.Subscribe(x => {		// sync event handler
	Console.WriteLine(x + "1");
});

source.Event.Subscribe(x => {		// sync event handler
	Console.WriteLine(x + "2");
});

Console.WriteLine("A");
source.Invoke("B");
Console.WriteLine("C");
```

The output is:

	A
	B1
	B2
	C

This shows that event handlers are invoked directly by the thread that calls **Invoke()**. There is no additional threading introduced by the library.  Also, **Invoke()** returns directly after all event handlers have completed.

Please note, the order in which event handlers are invoked is not deterministic. You shouldn't rely on that.


### Invoke with Asynchronous Event Handler

Now, let's take the same example but slightly modified with *async* event handlers. What's the output of this?

```cs
var source = new EventSource<String>();

source.Event.Subscribe(async x => {		// async event handler
	await Task.Delay(100);
	Console.WriteLine(x + "1");
});

source.Event.Subscribe(async x => {		// async event handler
	await Task.Delay(200);
	Console.WriteLine(x + "2");
});

Console.WriteLine("A");
source.Invoke("B");
Console.WriteLine("C");
```

The output is:

	A
	C
	...
	B1	(100 ms delayed)
	B2	(200 ms delayed)

Again, the thread calling **Invoke()** is also calling the event handlers. But this time, it returns after encountering the first *await* statement, causing **Invoke()** to return earlier as the event handler's continuations.

That means a consumer can decide whether it wants to register a synchronous or asynchronous event handler. In the latter case, from perspective of the event raiser the behavior is kind of fire-and-forget, because the event raiser can't be sure that all event handlers have completed when **Invoke()** returned.

If you need that guarantee that then use **InvokeAsync()** instead.


### InvokeAsync with Asynchronous Event Handler

As mentioned previously, **InvokeAsync()** can be used if awaiting the completion of all event handlers is necessary. 

```cs
var source = new EventSource<String>();

source.Event.Subscribe(async x => {		// async event handler
	await Task.Delay(100);
	Console.WriteLine(x + "1");
});

source.Event.Subscribe(async x => {		// async event handler
	await Task.Delay(200);
	Console.WriteLine(x + "2");
});

Console.WriteLine("A");
await source.InvokeAsync("B");			// await !!
Console.WriteLine("C");
```

This time the output is:

    A
    ...
    B1	(100 ms delayed)
    B2	(200 ms delayed)
    C

Feels sequential, although it runs fully asynchronous due to the magic of *async* and *await*.

Please note that there is still no additional threading involved. The thread calling **InvokeAsync()** still starts to execute the event handlers. The only special thing here is that **InvokeAsync()** awaits the completion of all those event handlers.

If for example, all registered event handlers are async methods but don't await anything, then the entire event invocation would be processed in synchronous fashion. In fact, the library implementation has special optimizations in place for this specific scenario of async handler that don't await or that complete immediately.


### InvokeAsync with Synchronous Event Handler

Of course, it is also possible to use **InvokeAsync()** for raising events, but with subscribers that register only synchronous event handlers. This is valid and the library implementation optimizes this scenario so that there is little overhead even though *async/await* is involved.

```cs
var source = new EventSource<String>();

source.Event.Subscribe(x => {		// sync event handler
	Console.WriteLine(x + "1");
});

source.Event.Subscribe(x => {		// sync event handler
	Console.WriteLine(x + "2");
});

Console.WriteLine("A");
await source.InvokeAsync("B");		// await !!
Console.WriteLine("C");
```

Of course, the output is:

	A
	B1
	B2
	C


### Raising Events

We have already learned that events are raised by calling **Invoke()** or **InvokeAsync()** providing the event argument. In most cases, the event argument won't be a simple string or integer value but some class that must be constructed and filled with information.

```cs
var source = new EventSource<FooEventArg>();
	
var arg = new FooEventArg() { ... };
source.Invoke(arg);
```

Now, what happens if you raise an event and not a single event handler has been registered? In that case, the construction of a new event argument will be wasted CPU instructions and memory allocation, because **Invoke()** will return immediately without doing anything with the supplied event argument.

What if you want to avoid such wasteful instructions?

Well, you can use one of the provided overloads that accept a *value factory* for constructing the event argument.

```cs
source.Invoke(() => {
	return new FooEventArg() { ... }
	// value factory is only called, if at least a single
	// event handler is registered
});
```

If you need to pass some value to the *value factory*, you can do that too. Use it to avoid closure allocations.

```cs
source.Invoke((arg1) => {
	return new FooEventArg() { ... }
	// value factory is only called, if at least a single
	// event handler is registered
},
123);	// supplied as arg1
```

The same overloads are available for **InvokeAsync()** too.


### Weak Subscriptions

\<TODO>

### Exception Behavior

\<TODO>

<!--stackedit_data:
eyJoaXN0b3J5IjpbMzQ0MDkwNjIzXX0=
-->