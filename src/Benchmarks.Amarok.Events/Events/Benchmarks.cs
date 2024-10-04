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
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.4249/23H2/2023Update/SunValley3)
   Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
     [Host]             : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256
     .NET 6.0           : .NET 6.0.33 (6.0.3324.36610), X64 RyuJIT AVX2
     .NET 7.0           : .NET 7.0.20 (7.0.2024.26716), X64 RyuJIT AVX2
     .NET 8.0           : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
     .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256


| Method                              | Job                | Runtime            | Mean        | Error     | StdDev    | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------------ |------------------- |------------------- |------------:|----------:|----------:|------------:|------:|--------:|-------:|----------:|------------:|
| DotNet_RaiseSync_SyncHandler_0      | .NET 6.0           | .NET 6.0           |   0.7864 ns | 0.0226 ns | 0.0200 ns |   0.7767 ns |  0.16 |    0.01 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 6.0           | .NET 6.0           |   5.0758 ns | 0.1199 ns | 0.1283 ns |   5.1423 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 6.0           | .NET 6.0           |  13.7030 ns | 0.2877 ns | 0.2691 ns |  13.8368 ns |  2.70 |    0.08 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 6.0           | .NET 6.0           |  27.8496 ns | 0.1097 ns | 0.0972 ns |  27.8802 ns |  5.49 |    0.16 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 6.0           | .NET 6.0           |   9.3039 ns | 0.0677 ns | 0.0565 ns |   9.2949 ns |  1.84 |    0.06 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 6.0           | .NET 6.0           |  13.2259 ns | 0.0535 ns | 0.0474 ns |  13.2203 ns |  2.61 |    0.07 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 6.0           | .NET 6.0           |  17.0574 ns | 0.1099 ns | 0.1028 ns |  17.0654 ns |  3.36 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 6.0           | .NET 6.0           |  28.3558 ns | 0.5284 ns | 0.4943 ns |  28.1452 ns |  5.59 |    0.20 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 6.0           | .NET 6.0           |  21.1527 ns | 0.1330 ns | 0.1244 ns |  21.1158 ns |  4.17 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 6.0           | .NET 6.0           |  32.2323 ns | 0.1399 ns | 0.1308 ns |  32.2656 ns |  6.35 |    0.18 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 6.0           | .NET 6.0           |  53.1425 ns | 0.4617 ns | 0.4092 ns |  53.0596 ns | 10.48 |    0.33 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 6.0           | .NET 6.0           | 124.4745 ns | 0.6603 ns | 0.5853 ns | 124.5927 ns | 24.55 |    0.62 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 6.0           | .NET 6.0           |   9.2897 ns | 0.0523 ns | 0.0489 ns |   9.2862 ns |  1.83 |    0.05 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 6.0           | .NET 6.0           |  17.5615 ns | 0.1370 ns | 0.1282 ns |  17.5895 ns |  3.46 |    0.10 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 6.0           | .NET 6.0           |  25.1708 ns | 0.1378 ns | 0.1289 ns |  25.1529 ns |  4.96 |    0.14 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 6.0           | .NET 6.0           |  52.3665 ns | 0.4898 ns | 0.4582 ns |  52.4500 ns | 10.32 |    0.32 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |             |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET 8.0           | .NET 8.0           |   0.5648 ns | 0.0391 ns | 0.0891 ns |   0.5425 ns |  0.18 |    0.03 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET 8.0           | .NET 8.0           |   3.1687 ns | 0.0110 ns | 0.0103 ns |   3.1661 ns |  1.00 |    0.00 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET 8.0           | .NET 8.0           |  14.0636 ns | 0.3072 ns | 0.8962 ns |  13.6619 ns |  4.26 |    0.10 | 0.0029 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET 8.0           | .NET 8.0           |  31.7769 ns | 0.5248 ns | 0.4382 ns |  31.8659 ns | 10.03 |    0.14 | 0.0029 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET 8.0           | .NET 8.0           |   9.2696 ns | 0.1422 ns | 0.1330 ns |   9.2184 ns |  2.93 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET 8.0           | .NET 8.0           |  11.7944 ns | 0.0475 ns | 0.0445 ns |  11.7868 ns |  3.72 |    0.02 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET 8.0           | .NET 8.0           |  14.6974 ns | 0.1213 ns | 0.1076 ns |  14.6729 ns |  4.64 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET 8.0           | .NET 8.0           |  25.0889 ns | 0.1538 ns | 0.1438 ns |  25.0757 ns |  7.92 |    0.05 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET 8.0           | .NET 8.0           |  15.9754 ns | 0.0813 ns | 0.0721 ns |  15.9777 ns |  5.04 |    0.03 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET 8.0           | .NET 8.0           |  22.0249 ns | 0.2728 ns | 0.2551 ns |  22.0775 ns |  6.95 |    0.08 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET 8.0           | .NET 8.0           |  30.6511 ns | 0.2810 ns | 0.2628 ns |  30.6724 ns |  9.67 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET 8.0           | .NET 8.0           |  60.1825 ns | 1.2328 ns | 1.2108 ns |  60.6308 ns | 18.98 |    0.37 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET 8.0           | .NET 8.0           |   9.1494 ns | 0.0569 ns | 0.0533 ns |   9.1532 ns |  2.89 |    0.02 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET 8.0           | .NET 8.0           |  15.0047 ns | 0.1336 ns | 0.1250 ns |  15.0148 ns |  4.74 |    0.03 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET 8.0           | .NET 8.0           |  18.3713 ns | 0.2325 ns | 0.1942 ns |  18.2884 ns |  5.80 |    0.06 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET 8.0           | .NET 8.0           |  32.3757 ns | 0.2630 ns | 0.2332 ns |  32.3264 ns | 10.22 |    0.07 |      - |         - |        0.00 |
|                                     |                    |                    |             |           |           |             |       |         |        |           |             |
| DotNet_RaiseSync_SyncHandler_0      | .NET Framework 4.8 | .NET Framework 4.8 |   0.8845 ns | 0.0075 ns | 0.0070 ns |   0.8839 ns |  0.17 |    0.00 |      - |         - |        0.00 |
| DotNet_RaiseSync_SyncHandler_1      | .NET Framework 4.8 | .NET Framework 4.8 |   5.0736 ns | 0.0376 ns | 0.0334 ns |   5.0672 ns |  1.00 |    0.00 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_3      | .NET Framework 4.8 | .NET Framework 4.8 |  10.5048 ns | 0.1706 ns | 0.1425 ns |  10.5407 ns |  2.07 |    0.03 | 0.0038 |      24 B |        1.00 |
| DotNet_RaiseSync_SyncHandler_9      | .NET Framework 4.8 | .NET Framework 4.8 |  18.5688 ns | 0.3148 ns | 0.2791 ns |  18.7109 ns |  3.66 |    0.06 | 0.0038 |      24 B |        1.00 |
| Amarok_InvokeSync_SyncHandler_0     | .NET Framework 4.8 | .NET Framework 4.8 |  17.6857 ns | 0.1395 ns | 0.1236 ns |  17.6999 ns |  3.49 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_1     | .NET Framework 4.8 | .NET Framework 4.8 |  22.7452 ns | 0.0704 ns | 0.0659 ns |  22.7310 ns |  4.48 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_3     | .NET Framework 4.8 | .NET Framework 4.8 |  27.9412 ns | 0.2337 ns | 0.2072 ns |  27.9832 ns |  5.51 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeSync_SyncHandler_9     | .NET Framework 4.8 | .NET Framework 4.8 |  43.2117 ns | 0.1871 ns | 0.1750 ns |  43.1621 ns |  8.52 |    0.06 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_0    | .NET Framework 4.8 | .NET Framework 4.8 |  31.4628 ns | 0.1141 ns | 0.0952 ns |  31.4978 ns |  6.20 |    0.04 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_1    | .NET Framework 4.8 | .NET Framework 4.8 |  46.7330 ns | 0.5570 ns | 0.4938 ns |  46.7058 ns |  9.21 |    0.12 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_3    | .NET Framework 4.8 | .NET Framework 4.8 |  77.0015 ns | 0.4818 ns | 0.4271 ns |  77.0203 ns | 15.18 |    0.09 |      - |         - |        0.00 |
| Amarok_InvokeAsync_SyncHandler_9    | .NET Framework 4.8 | .NET Framework 4.8 | 160.4786 ns | 2.5790 ns | 2.0135 ns | 160.8352 ns | 31.64 |    0.42 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  17.7137 ns | 0.0511 ns | 0.0427 ns |  17.7067 ns |  3.49 |    0.03 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  26.7313 ns | 0.2397 ns | 0.2242 ns |  26.7490 ns |  5.27 |    0.05 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  37.4732 ns | 0.2904 ns | 0.2716 ns |  37.5353 ns |  7.40 |    0.05 |      - |         - |        0.00 |
| Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  77.4060 ns | 0.7930 ns | 0.7030 ns |  77.5733 ns | 15.26 |    0.17 |      - |         - |        0.00 |
*/
