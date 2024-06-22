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

            ServiceHandlerCount1 = new FooServiceImpl();
            ServiceHandlerCount1.FooChanged += _HandleFooChanged;

            ServiceHandlerCount3 = new FooServiceImpl();
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;
            ServiceHandlerCount3.FooChanged += _HandleFooChanged;

            ServiceHandlerCount9 = new FooServiceImpl();
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
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.819)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
  [Host]             : .NET Framework 4.8.1 (4.8.9105.0), X64 RyuJIT VectorSize=256
  .NET 6.0           : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2
  .NET 7.0           : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9105.0), X64 RyuJIT VectorSize=256


|                              Method |                Job |            Runtime |        Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|------------------------------------ |------------------- |------------------- |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
|      DotNet_RaiseSync_SyncHandler_0 |           .NET 6.0 |           .NET 6.0 |   0.6681 ns | 0.0113 ns | 0.0100 ns |  0.14 |    0.00 |      - |         - |        0.00 |
|      DotNet_RaiseSync_SyncHandler_1 |           .NET 6.0 |           .NET 6.0 |   4.7512 ns | 0.0543 ns | 0.0508 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_3 |           .NET 6.0 |           .NET 6.0 |  14.8018 ns | 0.0808 ns | 0.0717 ns |  3.11 |    0.04 | 0.0029 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_9 |           .NET 6.0 |           .NET 6.0 |  33.0682 ns | 0.2402 ns | 0.2130 ns |  6.96 |    0.08 | 0.0029 |      24 B |        1.00 |
|     Amarok_InvokeSync_SyncHandler_0 |           .NET 6.0 |           .NET 6.0 |   9.5846 ns | 0.0310 ns | 0.0242 ns |  2.01 |    0.02 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_1 |           .NET 6.0 |           .NET 6.0 |  13.6985 ns | 0.0642 ns | 0.0600 ns |  2.88 |    0.04 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_3 |           .NET 6.0 |           .NET 6.0 |  18.8355 ns | 0.0436 ns | 0.0387 ns |  3.96 |    0.04 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_9 |           .NET 6.0 |           .NET 6.0 |  28.8602 ns | 0.0970 ns | 0.0810 ns |  6.07 |    0.07 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_0 |           .NET 6.0 |           .NET 6.0 |  21.8195 ns | 0.0541 ns | 0.0480 ns |  4.59 |    0.05 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_1 |           .NET 6.0 |           .NET 6.0 |  33.0055 ns | 0.0916 ns | 0.0857 ns |  6.95 |    0.07 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_3 |           .NET 6.0 |           .NET 6.0 |  55.1787 ns | 0.2307 ns | 0.2158 ns | 11.61 |    0.13 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_9 |           .NET 6.0 |           .NET 6.0 | 132.6702 ns | 0.7611 ns | 0.6747 ns | 27.91 |    0.31 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 |           .NET 6.0 |           .NET 6.0 |   9.6477 ns | 0.0376 ns | 0.0351 ns |  2.03 |    0.02 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 |           .NET 6.0 |           .NET 6.0 |  17.3868 ns | 0.0293 ns | 0.0245 ns |  3.66 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 |           .NET 6.0 |           .NET 6.0 |  26.4914 ns | 0.0874 ns | 0.0774 ns |  5.57 |    0.06 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 |           .NET 6.0 |           .NET 6.0 |  53.7996 ns | 0.1205 ns | 0.1068 ns | 11.32 |    0.12 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |       |         |        |           |             |
|      DotNet_RaiseSync_SyncHandler_0 |           .NET 7.0 |           .NET 7.0 |   0.4466 ns | 0.0100 ns | 0.0084 ns |  0.09 |    0.01 |      - |         - |        0.00 |
|      DotNet_RaiseSync_SyncHandler_1 |           .NET 7.0 |           .NET 7.0 |   5.1049 ns | 0.1293 ns | 0.3021 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_3 |           .NET 7.0 |           .NET 7.0 |  15.4013 ns | 0.3318 ns | 0.2941 ns |  3.04 |    0.25 | 0.0029 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_9 |           .NET 7.0 |           .NET 7.0 |  31.0969 ns | 0.5366 ns | 0.5020 ns |  6.11 |    0.50 | 0.0029 |      24 B |        1.00 |
|     Amarok_InvokeSync_SyncHandler_0 |           .NET 7.0 |           .NET 7.0 |   9.9577 ns | 0.2270 ns | 0.2332 ns |  1.94 |    0.14 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_1 |           .NET 7.0 |           .NET 7.0 |  14.3013 ns | 0.2154 ns | 0.2015 ns |  2.81 |    0.27 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_3 |           .NET 7.0 |           .NET 7.0 |  18.5085 ns | 0.2470 ns | 0.2310 ns |  3.64 |    0.31 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_9 |           .NET 7.0 |           .NET 7.0 |  30.6859 ns | 0.5484 ns | 0.5130 ns |  6.03 |    0.57 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_0 |           .NET 7.0 |           .NET 7.0 |  20.5891 ns | 0.1735 ns | 0.1623 ns |  4.04 |    0.34 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_1 |           .NET 7.0 |           .NET 7.0 |  31.2967 ns | 0.4055 ns | 0.3793 ns |  6.15 |    0.53 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_3 |           .NET 7.0 |           .NET 7.0 |  53.4488 ns | 0.4279 ns | 0.3793 ns | 10.54 |    0.92 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_9 |           .NET 7.0 |           .NET 7.0 | 130.2601 ns | 2.2981 ns | 2.1497 ns | 25.59 |    2.25 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 |           .NET 7.0 |           .NET 7.0 |   9.7874 ns | 0.1280 ns | 0.1197 ns |  1.92 |    0.16 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 |           .NET 7.0 |           .NET 7.0 |  17.6838 ns | 0.0864 ns | 0.0766 ns |  3.49 |    0.31 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 |           .NET 7.0 |           .NET 7.0 |  27.2539 ns | 0.2580 ns | 0.2413 ns |  5.35 |    0.46 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 |           .NET 7.0 |           .NET 7.0 |  56.6940 ns | 0.9515 ns | 0.8435 ns | 11.17 |    0.90 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |       |         |        |           |             |
|      DotNet_RaiseSync_SyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |   0.9326 ns | 0.0449 ns | 0.0517 ns |  0.14 |    0.01 |      - |         - |        0.00 |
|      DotNet_RaiseSync_SyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |   6.6365 ns | 0.1514 ns | 0.1486 ns |  1.00 |    0.00 | 0.0038 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  12.0029 ns | 0.1731 ns | 0.1619 ns |  1.81 |    0.03 | 0.0038 |      24 B |        1.00 |
|      DotNet_RaiseSync_SyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  22.1233 ns | 0.4688 ns | 0.6573 ns |  3.38 |    0.16 | 0.0038 |      24 B |        1.00 |
|     Amarok_InvokeSync_SyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  18.8044 ns | 0.0786 ns | 0.0735 ns |  2.83 |    0.06 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  23.9147 ns | 0.0889 ns | 0.0788 ns |  3.61 |    0.07 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  31.5646 ns | 0.3128 ns | 0.2773 ns |  4.77 |    0.10 |      - |         - |        0.00 |
|     Amarok_InvokeSync_SyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  44.7551 ns | 0.3489 ns | 0.3263 ns |  6.74 |    0.16 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  33.5400 ns | 0.6927 ns | 0.9482 ns |  5.13 |    0.21 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  47.4569 ns | 0.3294 ns | 0.2751 ns |  7.17 |    0.17 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  74.2533 ns | 0.6221 ns | 0.5819 ns | 11.19 |    0.30 |      - |         - |        0.00 |
|    Amarok_InvokeAsync_SyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 | 164.7248 ns | 0.9623 ns | 0.9002 ns | 24.82 |    0.63 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  17.9528 ns | 0.0844 ns | 0.0790 ns |  2.71 |    0.06 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  26.9861 ns | 0.0593 ns | 0.0526 ns |  4.08 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  39.2026 ns | 0.1337 ns | 0.1185 ns |  5.92 |    0.13 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  80.7273 ns | 0.9429 ns | 0.8820 ns | 12.16 |    0.28 |      - |         - |        0.00 |
 */
