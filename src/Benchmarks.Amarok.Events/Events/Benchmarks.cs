// Copyright (c) 2024, Olaf Kober <olaf.kober@outlook.com>

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA2012  // Use ValueTasks correctly

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace Amarok.Events;


[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class Benchmarks
{
    public class DotNet
    {
        public sealed class FooEventArgs : EventArgs
        {
            public String Payload { get; }

            public FooEventArgs(String payload)
            {
                Payload = payload;
            }
        }

        public interface IFooService
        {
            event EventHandler<FooEventArgs> FooChanged;
        }

        public sealed class FooServiceImpl : IFooService
        {
            public event EventHandler<FooEventArgs> FooChanged = null!;

            public void Do()
            {
                FooChanged?.Invoke(this, new FooEventArgs("A"));
            }
        }

        private void _HandleFooChanged(Object sender, FooEventArgs e)
        {
            // Method intentionally left empty.
        }


        public FooServiceImpl ServiceHandlerCount0;
        public FooServiceImpl ServiceHandlerCount1;
        public FooServiceImpl ServiceHandlerCount3;
        public FooServiceImpl ServiceHandlerCount9;


        public DotNet()
        {
            ServiceHandlerCount0 = new FooServiceImpl();

            ServiceHandlerCount1            =  new FooServiceImpl();
            ServiceHandlerCount1.FooChanged += _HandleFooChanged;

            ServiceHandlerCount3            =  new FooServiceImpl();
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;

            ServiceHandlerCount9            =  new FooServiceImpl();
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
            ServiceHandlerCount9.FooChanged += _HandleFooChanged;
        }
    }

    public class Amarok
    {
        public interface IFooService
        {
            Event<String> FooChanged { get; }
        }

        public sealed class FooServiceImpl : IFooService
        {
            private readonly EventSource<String> mFooChanged = new();

            public Event<String> FooChanged => mFooChanged.Event;

            public void Do()
            {
                mFooChanged.Invoke("A");
            }

            public ValueTask<Boolean> DoAsync()
            {
                return mFooChanged.InvokeAsync("A");
            }
        }

        private void _HandleFooChanged(String args)
        {
            // Method intentionally left empty.
        }

        private Task _HandleFooChangedAsync(String args)
        {
            return Task.CompletedTask;
        }


        public FooServiceImpl ServiceSyncHandlerCount0;
        public FooServiceImpl ServiceSyncHandlerCount1;
        public FooServiceImpl ServiceSyncHandlerCount3;
        public FooServiceImpl ServiceSyncHandlerCount9;

        public FooServiceImpl ServiceAsyncHandlerCount0;
        public FooServiceImpl ServiceAsyncHandlerCount1;
        public FooServiceImpl ServiceAsyncHandlerCount3;
        public FooServiceImpl ServiceAsyncHandlerCount9;

        public FooServiceImpl ServiceWeakHandlerCount0;
        public FooServiceImpl ServiceWeakHandlerCount1;
        public FooServiceImpl ServiceWeakHandlerCount3;
        public FooServiceImpl ServiceWeakHandlerCount9;

        private readonly List<IDisposable> mSubscriptions = new();


        public Amarok()
        {
            // sync handler
            ServiceSyncHandlerCount0 = new FooServiceImpl();

            ServiceSyncHandlerCount1 = new FooServiceImpl();
            ServiceSyncHandlerCount1.FooChanged.Subscribe(_HandleFooChanged);

            ServiceSyncHandlerCount3 = new FooServiceImpl();
            ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount3.FooChanged.Subscribe(_HandleFooChanged);

            ServiceSyncHandlerCount9 = new FooServiceImpl();
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);
            ServiceSyncHandlerCount9.FooChanged.Subscribe(_HandleFooChanged);

            // async handler
            ServiceAsyncHandlerCount0 = new FooServiceImpl();

            ServiceAsyncHandlerCount1 = new FooServiceImpl();
            ServiceAsyncHandlerCount1.FooChanged.Subscribe(_HandleFooChangedAsync);

            ServiceAsyncHandlerCount3 = new FooServiceImpl();
            ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount3.FooChanged.Subscribe(_HandleFooChangedAsync);

            ServiceAsyncHandlerCount9 = new FooServiceImpl();
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);
            ServiceAsyncHandlerCount9.FooChanged.Subscribe(_HandleFooChangedAsync);

            // weak sync handler
            ServiceWeakHandlerCount0 = new FooServiceImpl();

            ServiceWeakHandlerCount1 = new FooServiceImpl();

            mSubscriptions.Add(ServiceWeakHandlerCount1.FooChanged.SubscribeWeak(_HandleFooChanged));

            ServiceWeakHandlerCount3 = new FooServiceImpl();

            mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount3.FooChanged.SubscribeWeak(_HandleFooChanged));

            ServiceWeakHandlerCount9 = new FooServiceImpl();

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));

            mSubscriptions.Add(ServiceWeakHandlerCount9.FooChanged.SubscribeWeak(_HandleFooChanged));
        }
    }


    private readonly DotNet mDotNet;
    private readonly Amarok mAmarok;


    public Benchmarks()
    {
        mDotNet = new DotNet();
        mAmarok = new Amarok();
    }


    // ----- .NET Events -----


    [Benchmark]
    public void DotNet_RaiseSync_SyncHandler_0()
    {
        mDotNet.ServiceHandlerCount0.Do();
    }

    [Benchmark(Baseline = true)]
    public void DotNet_RaiseSync_SyncHandler_1()
    {
        mDotNet.ServiceHandlerCount1.Do();
    }

    [Benchmark]
    public void DotNet_RaiseSync_SyncHandler_3()
    {
        mDotNet.ServiceHandlerCount3.Do();
    }

    [Benchmark]
    public void DotNet_RaiseSync_SyncHandler_9()
    {
        mDotNet.ServiceHandlerCount9.Do();
    }


    // ----- sync handler -----


    [Benchmark]
    public void Amarok_InvokeSync_SyncHandler_0()
    {
        mAmarok.ServiceSyncHandlerCount0.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_SyncHandler_1()
    {
        mAmarok.ServiceSyncHandlerCount1.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_SyncHandler_3()
    {
        mAmarok.ServiceSyncHandlerCount3.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_SyncHandler_9()
    {
        mAmarok.ServiceSyncHandlerCount9.Do();
    }


    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_0()
    {
        mAmarok.ServiceSyncHandlerCount0.DoAsync().GetAwaiter().GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_1()
    {
        mAmarok.ServiceSyncHandlerCount1.DoAsync().GetAwaiter().GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_3()
    {
        mAmarok.ServiceSyncHandlerCount3.DoAsync().GetAwaiter().GetResult();
    }

    [Benchmark]
    public void Amarok_InvokeAsync_SyncHandler_9()
    {
        mAmarok.ServiceSyncHandlerCount9.DoAsync().GetAwaiter().GetResult();
    }


    // ----- weak sync handler -----


    [Benchmark]
    public void Amarok_InvokeSync_WeakSyncHandler_0()
    {
        mAmarok.ServiceWeakHandlerCount0.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_WeakSyncHandler_1()
    {
        mAmarok.ServiceWeakHandlerCount1.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_WeakSyncHandler_3()
    {
        mAmarok.ServiceWeakHandlerCount3.Do();
    }

    [Benchmark]
    public void Amarok_InvokeSync_WeakSyncHandler_9()
    {
        mAmarok.ServiceWeakHandlerCount9.Do();
    }
}

/*
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4249/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
 [Host]             : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256
 .NET 6.0           : .NET 6.0.33 (6.0.3324.36610), X64 RyuJIT AVX2
 .NET 8.0           : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256


| Method                              | Job                | Runtime            | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------------ |------------------- |------------------- |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| DotNet_RaiseSync_SyncHandler_0      | .NET 6.0           | .NET 6.0           |   0.7003 ns | 0.0394 ns | 0.0768 ns |  0.13 |    0.02 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 6.0           | .NET 6.0           |   5.2652 ns | 0.1307 ns | 0.2549 ns |  1.00 |    0.07 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 6.0           | .NET 6.0           |  15.9479 ns | 0.3215 ns | 0.3301 ns |  3.04 |    0.16 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 6.0           | .NET 6.0           |  33.4044 ns | 0.6660 ns | 0.7403 ns |  6.36 |    0.35 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 6.0           | .NET 6.0           |   9.4464 ns | 0.0535 ns | 0.0474 ns |  1.80 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 6.0           | .NET 6.0           |  13.4490 ns | 0.1023 ns | 0.0957 ns |  2.56 |    0.13 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 6.0           | .NET 6.0           |  17.3711 ns | 0.1694 ns | 0.1585 ns |  3.31 |    0.17 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 6.0           | .NET 6.0           |  28.5248 ns | 0.0941 ns | 0.0834 ns |  5.43 |    0.27 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 6.0           | .NET 6.0           |  21.3748 ns | 0.2030 ns | 0.1695 ns |  4.07 |    0.21 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 6.0           | .NET 6.0           |  35.5891 ns | 0.7253 ns | 0.7760 ns |  6.78 |    0.37 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 6.0           | .NET 6.0           |  59.1666 ns | 1.2148 ns | 3.5436 ns | 11.26 |    0.88 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 6.0           | .NET 6.0           | 143.3530 ns | 2.8934 ns | 5.1430 ns | 27.29 |    1.68 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 6.0           | .NET 6.0           |   9.4788 ns | 0.2096 ns | 0.2650 ns |  1.80 |    0.10 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 6.0           | .NET 6.0           |  17.4709 ns | 0.1611 ns | 0.1507 ns |  3.33 |    0.17 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 6.0           | .NET 6.0           |  26.5528 ns | 0.3904 ns | 0.3260 ns |  5.06 |    0.26 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 6.0           | .NET 6.0           |  54.2649 ns | 0.7201 ns | 0.6736 ns | 10.33 |    0.53 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET 8.0           | .NET 8.0           |   0.5125 ns | 0.0375 ns | 0.0446 ns |  0.15 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 8.0           | .NET 8.0           |   3.4385 ns | 0.0906 ns | 0.0890 ns |  1.00 |    0.04 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 8.0           | .NET 8.0           |  14.3698 ns | 0.0406 ns | 0.0339 ns |  4.18 |    0.11 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 8.0           | .NET 8.0           |  31.7148 ns | 0.6679 ns | 0.7147 ns |  9.23 |    0.31 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 8.0           | .NET 8.0           |   9.2277 ns | 0.0631 ns | 0.0527 ns |  2.69 |    0.07 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 8.0           | .NET 8.0           |  11.7886 ns | 0.0653 ns | 0.0611 ns |  3.43 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 8.0           | .NET 8.0           |  14.7174 ns | 0.0512 ns | 0.0428 ns |  4.28 |    0.11 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 8.0           | .NET 8.0           |  25.2025 ns | 0.2367 ns | 0.1976 ns |  7.33 |    0.19 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 8.0           | .NET 8.0           |  16.0748 ns | 0.1070 ns | 0.1001 ns |  4.68 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 8.0           | .NET 8.0           |  22.0171 ns | 0.2328 ns | 0.2178 ns |  6.41 |    0.17 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 8.0           | .NET 8.0           |  30.8757 ns | 0.3024 ns | 0.2525 ns |  8.99 |    0.24 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 8.0           | .NET 8.0           |  59.3357 ns | 0.1407 ns | 0.1316 ns | 17.27 |    0.44 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 8.0           | .NET 8.0           |   9.1891 ns | 0.0504 ns | 0.0447 ns |  2.67 |    0.07 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 8.0           | .NET 8.0           |  15.0480 ns | 0.1043 ns | 0.0925 ns |  4.38 |    0.11 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 8.0           | .NET 8.0           |  18.4216 ns | 0.0999 ns | 0.0934 ns |  5.36 |    0.14 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 8.0           | .NET 8.0           |  32.6103 ns | 0.1751 ns | 0.1638 ns |  9.49 |    0.25 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET Framework 4.8 | .NET Framework 4.8 |   0.7037 ns | 0.0379 ns | 0.0421 ns |  0.13 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET Framework 4.8 | .NET Framework 4.8 |   5.2537 ns | 0.0183 ns | 0.0171 ns |  1.00 |    0.00 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET Framework 4.8 | .NET Framework 4.8 |  11.0828 ns | 0.2094 ns | 0.1856 ns |  2.11 |    0.03 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET Framework 4.8 | .NET Framework 4.8 |  20.4994 ns | 0.3663 ns | 0.3059 ns |  3.90 |    0.06 | 0.0038 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET Framework 4.8 | .NET Framework 4.8 |  18.2018 ns | 0.0447 ns | 0.0396 ns |  3.46 |    0.01 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET Framework 4.8 | .NET Framework 4.8 |  22.9181 ns | 0.4014 ns | 0.3755 ns |  4.36 |    0.07 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET Framework 4.8 | .NET Framework 4.8 |  31.2571 ns | 0.2455 ns | 0.2177 ns |  5.95 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET Framework 4.8 | .NET Framework 4.8 |  45.5039 ns | 0.5848 ns | 0.4884 ns |  8.66 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET Framework 4.8 | .NET Framework 4.8 |  32.3326 ns | 0.0985 ns | 0.0873 ns |  6.15 |    0.03 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET Framework 4.8 | .NET Framework 4.8 |  47.6829 ns | 0.9317 ns | 1.1091 ns |  9.08 |    0.21 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET Framework 4.8 | .NET Framework 4.8 |  80.1755 ns | 1.6278 ns | 1.5226 ns | 15.26 |    0.28 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET Framework 4.8 | .NET Framework 4.8 | 168.7550 ns | 3.2871 ns | 3.5171 ns | 32.12 |    0.66 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  18.2167 ns | 0.2684 ns | 0.2511 ns |  3.47 |    0.05 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  27.2556 ns | 0.4236 ns | 0.3962 ns |  5.19 |    0.07 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  38.5338 ns | 0.5647 ns | 0.5282 ns |  7.33 |    0.10 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  83.0986 ns | 0.4799 ns | 0.4255 ns | 15.82 |    0.09 |      - |         - |        0.00 |
*/
