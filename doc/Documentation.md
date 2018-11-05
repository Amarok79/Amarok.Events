## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as a replacement for regular .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. Besides, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events in regard to raw call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/)

The package provides strong-named binaries for *.NET Standard 2.0* only. Tests are performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.

For development, you need *Visual Studio 2017* (v15.7 or later).


## Documentation

Table of Content:
- [Event Source And Event](#event-source-and-event)
- [Invoke with Synchronous Event Handler](#invoke-with-synchronous-event-handler)
- [Invoke with Asynchronous Event Handler](#invoke-with-asynchronous-event-handler)
- [InvokeAsync with Asynchronous Event Handler](#invokeasync-with-asynchronous-event-handler)
- [InvokeAsync with Synchronous Event Handler](#invokeasync-with-synchronous-event-handler)
- [Raising Events](#raising-events)
- [Weak Subscriptions](#weak-subscriptions)
- [Exception Behavior](#exception-behavior)
- [IProgress\<T> Integration](#iprogresst-integration)
- [Event Recorder](#event-recorder)


### Event Source and Event

Suppose you have an interface and you want to expose an event on that interface. You do that as follows:

```cs
public interface IFooService
{
    Event<Int32> Progress { get; }
}
```

The event is declared as *getter-only property* of type **Event\<T>**, where **T** represents the type of event argument. This **T** can be of any type.

The implementation class of that interface then initializes a field of type **EventSource\<T>** and implements the getter-only event property.

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

For raising the event, one calls **Invoke(**..**)** on the *event source*. Here you supply the event argument that is forwarded to all event handlers.

Next, a consumer of the service can subscribe to the event. It just has to call **Subscribe(**..**)** on the *event* that is made public by the service.

```cs
FooServiceImpl serviceImpl = new FooServiceImpl();
IFooService service = serviceImpl;

IDisposable subscription = service.Progress.Subscribe(x => {
    Console.WriteLine(x + "%");
});

serviceImpl.DoSomething();        // internally raises the event
// console output:    50%
```

The object returned from **Subscribe(**..**)** can be used to cancel the subscription at any time.

```cs
subscription.Dispose();
    
serviceImpl.DoSomething();
// does nothing, since no subscribers are registered anymore
```

It is recommended that subscribers store these subscription objects somewhere. Otherwise, they won't be able to remove their registered event handlers.

If instead the class exposing the event wants to cancel all subscriptions, for example, because it gets disposed, it can dispose the *event source* too, which automatically cancels all subscriptions and ignores further calls to **Invoke(**..**)**.

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

source.Event.Subscribe(x => {        // sync event handler
    Console.WriteLine(x + "1");
});

source.Event.Subscribe(x => {        // sync event handler
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

source.Event.Subscribe(async x => {        // async event handler
    await Task.Delay(100);
    Console.WriteLine(x + "1");
});

source.Event.Subscribe(async x => {        // async event handler
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
    B1    (100 ms delayed)
    B2    (200 ms delayed)

Again, the thread calling **Invoke()** is also calling the event handlers. However, this time, it returns after encountering the first *await* statement, causing **Invoke()** to return earlier as the event handler's continuations.

That means a consumer can decide whether it wants to register a synchronous or asynchronous event handler. In the latter case, from a perspective of the event raiser, the behavior is kind of fire-and-forget, because the event raiser can't be sure that all event handlers have completed when **Invoke()** returned.

If you need that guarantee that then use **InvokeAsync()** instead.


### InvokeAsync with Asynchronous Event Handler

As mentioned previously, **InvokeAsync()** can be used if awaiting the completion of all event handlers is necessary. 

```cs
var source = new EventSource<String>();

source.Event.Subscribe(async x => {        // async event handler
    await Task.Delay(100);
    Console.WriteLine(x + "1");
});

source.Event.Subscribe(async x => {        // async event handler
    await Task.Delay(200);
    Console.WriteLine(x + "2");
});

Console.WriteLine("A");
await source.InvokeAsync("B");            // await !!
Console.WriteLine("C");
```

This time the output is:

    A
    ...
    B1    (100 ms delayed)
    B2    (200 ms delayed)
    C

Feels sequential, although it runs fully asynchronous due to the magic of *async* and *await*.

Please note that there is still no additional threading involved. The thread calling **InvokeAsync()** still starts to execute the event handlers. The only special thing here is that **InvokeAsync()** awaits the completion of all those event handlers.

If for example, all registered event handlers are async methods but don't await anything, then the entire event invocation would be processed in a synchronous fashion. In fact, the library implementation has special optimizations in place for this specific scenario of async handlers that don't await and complete immediately.


### InvokeAsync with Synchronous Event Handler

Of course, it is also possible to use **InvokeAsync()** for raising events, but with subscribers that register only synchronous event handlers. This is valid, and the library implementation optimizes this scenario so that there is little overhead even though *async/await* is involved.

```cs
var source = new EventSource<String>();

source.Event.Subscribe(x => {        // sync event handler
    Console.WriteLine(x + "1");
});

source.Event.Subscribe(x => {        // sync event handler
    Console.WriteLine(x + "2");
});

Console.WriteLine("A");
await source.InvokeAsync("B");        // await !!
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

Now, what happens if you raise an event and not a single event handler has been registered? In that case, the construction of a new event argument is wasted CPU instructions and memory allocation, because **Invoke()** returns immediately without doing anything with the supplied event argument.

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
123);    // supplied as arg1
```

The same overloads are available for **InvokeAsync()** too.


### Weak Subscriptions

It is possible to register an event handler (synchronous or asynchronous) via weak subscription. That is one that is automatically removed after the event handler has been garbage collected.

Weak subscriptions support a particular but common use case. Using weak subscriptions in a wrong way can be dangerous as it can cause hard-to-reproduce bugs. Consider this as a warning. However, correctly used they can help in preventing memory leaks, for example, just because you forgot to manually remove an event handler.

Let's imagine an application where you have a set of services. These services are mainly persistent and live for the entire application lifetime. Such services are commonly registered as singletons in a dependency injection container.

Suppose **IUserManagement** represents such a service. As in all the previous examples, the service exposes an event.

```cs
public interface IUserManagement
{
    Event<User> UserAdded { get; }
}
```

Next, imagine we have consumers of that service that register on that event.

For example, we might have other persistent services, but they are not a big deal, because they get constructed at some time, register on our **UserAdded** event and the event subscription exists for the remaining application lifetime, same as the involved services.

Quite different are user interface related objects like views. Those don't live for the entire application lifetime, but get constructed, register on events, get closed, disposed. When you forget to remove a subscription taken by such a view you have a memory leak. The size of leaked memory increases as the view gets opened and closed multiple times.
This happens because as in any other observer pattern implementation the observer (in our case the event source) maintains a strong reference to the observable (event handler in our case). This causes quite often memory leaks.

Here come weak subscriptions into play as they can help prevent such memory leaks. They free the developer from the burden to manually remove event subscriptions.

Such a UI view can use weak subscriptions on our **UserAdded** event as follows.

```cs
public sealed class BarView
{
    // this field is necessary to hold the event subscription
    private IDisposable mUserAddedSubscription;

    public BarView(IFooService service)
    {
        // when using SubscribeWeak() the returned object must be
        // stored into a field, otherwise the subscription will get
        // out of scope and get garbage collected too early
        mUserAddedSubscription = service.UserAdded.SubscribeWeak(
            x => HandleUserAdded(x)
        );
    }
        
    private void HandleUserAdded(User user)
    {
        ...
    }
}
```

That's it.

In fact, there is just one important point here: Store the subscription object returned by **SubscribeWeak()** into a member field of the same object as your event handler.

You can use that returned object also to cancel the subscription at any time, for example, when the view gets closed. That makes subscription cancellation more deterministic.

If you don't cancel the subscription manually, it is automatically removed from the service event after the view gets closed and garbage collected. This works since only the view hold a strong reference to the subscription (as shown in the previous example). With weak subscriptions, the *event source* of our service doesn't maintain a strong reference to the subscription and the event handler anymore.

Since the view is kept in memory from other root objects (the UI framework) the subscription is kept alive, and the event handler in the view is invoked as expected. After the view gets closed, all strong references to the view are removed, meaning the view and also it's (the only) strong reference to the subscription are garbage collected.

Weak subscriptions are no silver bullet. As always choose the right tool for the job.


### Exception Behavior

Consider the following example. What if one of the event handlers throws an exception.

```cs
var source = new EventSource<String>();

source.Event.Subscribe(x => {        // sync event handler
    Console.WriteLine(x + "1");
    throw new Exception();
});

source.Event.Subscribe(x => {        // sync event handler
    Console.WriteLine(x + "2");
});

Console.WriteLine("A");
source.Invoke("B");
Console.WriteLine("C");
```

What would you expect to happen? Is the second event handler called regardless of the exception? Is the exception propagated back to the caller?

Well, regarding exception handling, this library takes a different, maybe controversial approach. I consider it a design flaw of regular .NET events that invocation of the remaining event handlers is aborted if one of the previously invoked event handlers threw an exception. Since the thrown exception is reported back to the event publisher, this makes the event publisher dependent on its subscribers, but the entire observer design pattern exists to decouple both, publisher and subscribers.

In my opinion, the pattern only makes sense, if a publisher doesn’t need to care about whether there are subscribers, or whether those subscribers fail with exceptions. The publisher’s only responsibility is to invoke all registered event handlers in all cases.

So, our example generates following output:

    A  
    B1  
    B2  
    C

B2 is reliably invoked, even though B1 threw an exception.

But, what happened with the exception?

Well, the event publisher is NOT bothered with exception handling. Events are there to notify other parts of the application. This is kind of one-way communication. Exceptions are therefore not propagated back to the caller.

Instead, the library catches all exceptions thrown in event handlers and forwards them to the global event **UnobservedException** on **EventSystem**. That way an application can observe all otherwise unobserved exceptions thrown in event handlers and at least log them.

```cs
EventSystem.UnobservedException.Subscribe(ex => {
    Console.WriteLine(ex);
});
```

If an application wants to handle those exceptions - and that is generally recommended – then the exception handling should happen in the event handlers itself. If exceptions can occur in an event handler, then the event handler is responsible for careful handling.


### IProgress\<T> Integration

The interface **IProgress\<T>** defined by .NET BCL is a commonly-used way to transfer progress.

For example, long-running methods often accept an **IProgress\<T>** as an argument to report back progress.

```cs
public void SomeLongRunningMethod(IProgress<Int32> progress)
{
    // reports progress back to caller
    for(Int32 i = 0; i < 100; i++)
        progress.Report(i);
}
```

The caller then supplies an implementation to receive the progress. This is often done using the ready-made **Progress\<T>** class.

```cs
var progress = new Progress<T>(x =>
    Console.WriteLine(x)
);

SomeLongRunningMethod(progress);

// output might be out-of-order 
0
1
2
6
3
4
...
99
98
```

Depending on from which thread you call this long-running method you might be surprised that the console output is not always sequentially increasing numbers from 0 to 99. That's because **Progress\<T>** invokes its callback via a synchronization context.

That can be the UI thread, then sequential numbers 0 to 99 will be the result because everything is synchronized via the UI thread.

If it's not the UI thread, then the callback will be invoked via the thread-pool thread, which means callbacks can come out-of-order!

As an alternative, you can supply an **EventSource\<T>** as progress object. It forwards progress to its subscribers in the correct order.

```cs
var progress = new EventSource<T>();

progress.Event.Subscribe(x =>
    Console.WriteLine(x)
);

SomeLongRunningMethod(progress);

// always generates:
0
1
2
...
97
98
99
```

Of course, it is also possible to subscribe an **IProgress\<T>** onto an event, so that raised event arguments are forwarded to the progress object.

```cs
IProgress<Int32> progress = new Progress<Int32>(x =>
    Console.WriteLine(x)
);

var source = new EventSource<Int32>();
source.Event.Subscribe(progress);

source.Invoke(123);

// output:
123
```


### Event Recorder

If you are writing unit tests, then there will come the time where you want to ensure that an event on your subject-under-test is correctly raised with the expected event arguments.

To ease unit testing, this library provides a ready-made event recorder that can be used to record events and then, later on, analyze them.

As an example, suppose we have the following implementation class.

```cs
public sealed class UserManagementService
{
    private readonly EventSource<String> mUserAddedEvent = new EventSource<String>();

    public void AddUser(String name)
    {
        mUserAddedEvent.Invoke(name);
    }
}
```

In a unit test, we want to assert that the event is raised and that the supplied name is supplied to the event.

```cs
[Test]
public void AddUserRaisesEventWithUserName
{
    // arrange
    var sut = new UserManagementService();
    var recorder = EventRecorder.From(sut.UserAdded);

    // act
    sut.AddUser("Foo");
    Thread.Sleep(50);
    sut.AddUser("Bar");
    
    // assert (using NFluent assertions)
    Check.That(recorder.Events)
        .HasSize(2);        // we expect two events

    Check.That(recorder.Events[0])
        .IsEqualTo("Foo");    // first "Foo" was recorded
    Check.That(recorder.Events[1])
        .IsEqualTo("Bar");    // then "Bar" as expected
}
```

The event recorder can be used to gain even more information. Instead of accessing property **Events**, one can use **EventInfos**, which returns timing and thread information about the recorded events.

```cs
    recorder.EventInfos[0].Value        // "Foo"
    recorder.EventInfos[0].Index        // 0
    recorder.EventInfos[0].Timestamp    // DateTimeOffset
    recorder.EventInfos[0].TimeOffset    // 0 ms
    recorder.EventInfos[0].Thread        // the calling thread
    
    recorder.EventInfos[1].Value        // "Bar"
    recorder.EventInfos[1].Index        // 1
    recorder.EventInfos[1].Timestamp    // DateTimeOffset
    recorder.EventInfos[1].TimeOffset    // 50 ms
    recorder.EventInfos[1].Thread        // the calling thread
```

If you don't want to record events temporarily, you can **Pause()** and finally **Resume()** the event recorder. If you want to turn off recording completely, call **Dispose()**. To clear the list of recorded events, you can use **Reset()**.
