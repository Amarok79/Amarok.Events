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
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
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
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3737/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.302
  [Host]             : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  .NET 6.0           : .NET 6.0.31 (6.0.3124.26714), X64 RyuJIT AVX2
  .NET 7.0           : .NET 7.0.20 (7.0.2024.26716), X64 RyuJIT AVX2
  .NET 8.0           : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9241.0), X64 RyuJIT VectorSize=256


| Method                              | Job                | Runtime            | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------------ |------------------- |------------------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| DotNet_RaiseSync_SyncHandler_0      | .NET 6.0           | .NET 6.0           |   0.8459 ns | 0.0431 ns | 0.0443 ns |   0.8659 ns |  0.15 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 6.0           | .NET 6.0           |   5.6555 ns | 0.1401 ns | 0.2491 ns |   5.7292 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 6.0           | .NET 6.0           |  16.9765 ns | 0.3656 ns | 0.6866 ns |  17.2744 ns |  3.00 |    0.19 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 6.0           | .NET 6.0           |  33.5653 ns | 0.6947 ns | 1.0183 ns |  33.9036 ns |  5.96 |    0.35 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 6.0           | .NET 6.0           |   9.3920 ns | 0.1543 ns | 0.1444 ns |   9.4084 ns |  1.68 |    0.11 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 6.0           | .NET 6.0           |  13.9720 ns | 0.3056 ns | 0.4848 ns |  14.1031 ns |  2.47 |    0.13 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 6.0           | .NET 6.0           |  19.2539 ns | 0.4116 ns | 0.9120 ns |  19.3827 ns |  3.38 |    0.22 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 6.0           | .NET 6.0           |  33.7401 ns | 0.6855 ns | 0.9151 ns |  34.1175 ns |  5.96 |    0.32 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 6.0           | .NET 6.0           |  22.8054 ns | 0.4700 ns | 0.6433 ns |  23.0275 ns |  4.04 |    0.23 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 6.0           | .NET 6.0           |  36.2314 ns | 0.7060 ns | 0.7251 ns |  36.5331 ns |  6.44 |    0.38 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 6.0           | .NET 6.0           |  60.7207 ns | 1.1871 ns | 1.4131 ns |  61.2285 ns | 10.77 |    0.61 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 6.0           | .NET 6.0           | 142.1782 ns | 2.8368 ns | 3.8831 ns | 143.1593 ns | 25.22 |    1.51 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 6.0           | .NET 6.0           |   9.5414 ns | 0.2159 ns | 0.3297 ns |   9.4810 ns |  1.69 |    0.10 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 6.0           | .NET 6.0           |  18.2231 ns | 0.3808 ns | 0.4233 ns |  18.4194 ns |  3.24 |    0.19 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 6.0           | .NET 6.0           |  27.8794 ns | 0.5337 ns | 0.4993 ns |  28.0218 ns |  4.98 |    0.31 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 6.0           | .NET 6.0           |  58.4078 ns | 1.1860 ns | 1.8464 ns |  59.0945 ns | 10.35 |    0.68 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |             |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET 7.0           | .NET 7.0           |   0.5971 ns | 0.0403 ns | 0.0480 ns |   0.6123 ns |  0.12 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 7.0           | .NET 7.0           |   4.8202 ns | 0.1255 ns | 0.2230 ns |   4.9018 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 7.0           | .NET 7.0           |  14.7010 ns | 0.3224 ns | 0.5205 ns |  14.9079 ns |  3.06 |    0.13 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 7.0           | .NET 7.0           |  32.0333 ns | 0.6504 ns | 0.9933 ns |  32.3308 ns |  6.69 |    0.47 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 7.0           | .NET 7.0           |   8.9876 ns | 0.0267 ns | 0.0223 ns |   8.9913 ns |  1.88 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 7.0           | .NET 7.0           |  15.1766 ns | 0.1916 ns | 0.1600 ns |  15.2210 ns |  3.17 |    0.21 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 7.0           | .NET 7.0           |  17.2047 ns | 0.1984 ns | 0.1856 ns |  17.2732 ns |  3.58 |    0.23 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 7.0           | .NET 7.0           |  30.2012 ns | 0.2236 ns | 0.2092 ns |  30.2647 ns |  6.28 |    0.37 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 7.0           | .NET 7.0           |  19.3494 ns | 0.3259 ns | 0.3049 ns |  19.4729 ns |  4.03 |    0.22 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 7.0           | .NET 7.0           |  32.2576 ns | 0.6376 ns | 0.6548 ns |  32.4127 ns |  6.69 |    0.30 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 7.0           | .NET 7.0           |  54.7070 ns | 1.0268 ns | 0.9604 ns |  54.9895 ns | 11.38 |    0.65 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 7.0           | .NET 7.0           | 134.0357 ns | 0.9486 ns | 0.8874 ns | 134.3276 ns | 27.89 |    1.72 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 7.0           | .NET 7.0           |   8.9450 ns | 0.0415 ns | 0.0368 ns |   8.9338 ns |  1.86 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 7.0           | .NET 7.0           |  19.8755 ns | 0.4182 ns | 0.4107 ns |  20.0671 ns |  4.13 |    0.23 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 7.0           | .NET 7.0           |  25.9765 ns | 0.5380 ns | 0.6196 ns |  26.2010 ns |  5.44 |    0.40 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 7.0           | .NET 7.0           |  53.7964 ns | 1.0573 ns | 1.2176 ns |  54.2402 ns | 11.26 |    0.75 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |             |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET 8.0           | .NET 8.0           |   0.6330 ns | 0.0445 ns | 0.1254 ns |   0.5694 ns |  0.19 |    0.04 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 8.0           | .NET 8.0           |   3.3658 ns | 0.0957 ns | 0.1103 ns |   3.3925 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 8.0           | .NET 8.0           |  13.4616 ns | 0.2913 ns | 0.5024 ns |  13.6665 ns |  3.95 |    0.25 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 8.0           | .NET 8.0           |  30.6417 ns | 0.2242 ns | 0.1988 ns |  30.7163 ns |  9.14 |    0.42 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 8.0           | .NET 8.0           |   8.9091 ns | 0.0334 ns | 0.0296 ns |   8.9050 ns |  2.66 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 8.0           | .NET 8.0           |  11.7764 ns | 0.1272 ns | 0.1062 ns |  11.8121 ns |  3.52 |    0.16 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 8.0           | .NET 8.0           |  14.9088 ns | 0.3200 ns | 0.3286 ns |  15.0345 ns |  4.44 |    0.23 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 8.0           | .NET 8.0           |  25.8005 ns | 0.5146 ns | 0.4814 ns |  26.0586 ns |  7.69 |    0.36 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 8.0           | .NET 8.0           |  16.2889 ns | 0.0791 ns | 0.0740 ns |  16.2994 ns |  4.86 |    0.22 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 8.0           | .NET 8.0           |  23.2492 ns | 0.1056 ns | 0.0988 ns |  23.2850 ns |  6.93 |    0.31 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 8.0           | .NET 8.0           |  31.9987 ns | 0.6598 ns | 0.8104 ns |  32.4395 ns |  9.50 |    0.31 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 8.0           | .NET 8.0           |  61.3478 ns | 1.2554 ns | 1.8004 ns |  62.2707 ns | 18.22 |    0.95 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 8.0           | .NET 8.0           |   8.8867 ns | 0.0524 ns | 0.0490 ns |   8.8786 ns |  2.65 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 8.0           | .NET 8.0           |  14.8688 ns | 0.0575 ns | 0.0510 ns |  14.8791 ns |  4.44 |    0.20 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 8.0           | .NET 8.0           |  18.4821 ns | 0.2553 ns | 0.2389 ns |  18.5763 ns |  5.51 |    0.19 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 8.0           | .NET 8.0           |  33.2275 ns | 0.5768 ns | 0.5113 ns |  33.3327 ns |  9.91 |    0.29 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |             |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET Framework 4.8 | .NET Framework 4.8 |   0.9641 ns | 0.0317 ns | 0.0296 ns |   0.9730 ns |  0.18 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET Framework 4.8 | .NET Framework 4.8 |   5.2899 ns | 0.1294 ns | 0.2585 ns |   5.2637 ns |  1.00 |    0.00 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET Framework 4.8 | .NET Framework 4.8 |  11.3498 ns | 0.0770 ns | 0.0721 ns |  11.3571 ns |  2.10 |    0.11 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET Framework 4.8 | .NET Framework 4.8 |  19.8230 ns | 0.4170 ns | 0.6113 ns |  20.0485 ns |  3.68 |    0.21 | 0.0038 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET Framework 4.8 | .NET Framework 4.8 |  18.8297 ns | 0.3965 ns | 0.7047 ns |  19.1787 ns |  3.54 |    0.21 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET Framework 4.8 | .NET Framework 4.8 |  25.3644 ns | 0.5008 ns | 0.5142 ns |  25.5060 ns |  4.70 |    0.26 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET Framework 4.8 | .NET Framework 4.8 |  30.2491 ns | 0.6202 ns | 0.9283 ns |  30.6726 ns |  5.64 |    0.36 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET Framework 4.8 | .NET Framework 4.8 |  46.8548 ns | 0.9542 ns | 1.4855 ns |  47.3795 ns |  8.75 |    0.55 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET Framework 4.8 | .NET Framework 4.8 |  33.8427 ns | 0.7004 ns | 1.0266 ns |  34.2712 ns |  6.29 |    0.37 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET Framework 4.8 | .NET Framework 4.8 |  49.7666 ns | 0.9826 ns | 1.0922 ns |  50.2873 ns |  9.23 |    0.44 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET Framework 4.8 | .NET Framework 4.8 |  77.7693 ns | 1.5440 ns | 2.4039 ns |  78.9525 ns | 14.52 |    0.87 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET Framework 4.8 | .NET Framework 4.8 | 171.3303 ns | 3.4168 ns | 6.2477 ns | 173.8304 ns | 32.31 |    2.17 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  18.9957 ns | 0.2511 ns | 0.2226 ns |  19.0743 ns |  3.54 |    0.16 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  28.5997 ns | 0.5957 ns | 0.9619 ns |  29.0051 ns |  5.36 |    0.35 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  40.2775 ns | 0.8271 ns | 1.3119 ns |  40.7858 ns |  7.52 |    0.48 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  83.7744 ns | 1.7000 ns | 2.6466 ns |  85.0398 ns | 15.64 |    0.98 |      - |         - |        0.00 |
 */
