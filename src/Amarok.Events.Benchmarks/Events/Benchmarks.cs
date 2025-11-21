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
[SimpleJob(RuntimeMoniker.Net10_0)]
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

        private readonly List<IDisposable> mSubscriptions = [ ];


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
BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7019)
   13th Gen Intel Core i9-13900HX 2.20GHz, 1 CPU, 32 logical and 24 physical cores
     [Host]             : .NET Framework 4.8.1 (4.8.9323.0), X64 RyuJIT VectorSize=256
     .NET 10.0          : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
     .NET 6.0           : .NET 6.0.36 (6.0.36, 6.0.3624.51421), X64 RyuJIT x86-64-v3
     .NET 8.0           : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v3
     .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9323.0), X64 RyuJIT VectorSize=256


   | Method                              | Job                | Runtime            | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
   |------------------------------------ |------------------- |------------------- |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
   | DotNet_RaiseSync_SyncHandler_0      | .NET 10.0          | .NET 10.0          |   0.1181 ns | 0.0237 ns | 0.0210 ns |  0.08 |    0.01 |      - |         - |        0.00 |
   | DotNet_RaiseSync_SyncHandler_1      | .NET 10.0          | .NET 10.0          |   1.4700 ns | 0.0544 ns | 0.0455 ns |  1.00 |    0.04 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_3      | .NET 10.0          | .NET 10.0          |   6.7367 ns | 0.0806 ns | 0.0673 ns |  4.59 |    0.14 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_9      | .NET 10.0          | .NET 10.0          |  14.6737 ns | 0.1822 ns | 0.1704 ns |  9.99 |    0.31 | 0.0013 |      24 B |        1.00 |
   | Amarok_InvokeSync_SyncHandler_0     | .NET 10.0          | .NET 10.0          |   8.1095 ns | 0.0228 ns | 0.0202 ns |  5.52 |    0.16 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_1     | .NET 10.0          | .NET 10.0          |  10.3730 ns | 0.0554 ns | 0.0518 ns |  7.06 |    0.21 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_3     | .NET 10.0          | .NET 10.0          |  11.5765 ns | 0.0518 ns | 0.0459 ns |  7.88 |    0.23 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_9     | .NET 10.0          | .NET 10.0          |  18.2738 ns | 0.1773 ns | 0.1384 ns | 12.44 |    0.37 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_0    | .NET 10.0          | .NET 10.0          |   7.6528 ns | 0.0551 ns | 0.0515 ns |  5.21 |    0.15 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_1    | .NET 10.0          | .NET 10.0          |  13.0954 ns | 0.1342 ns | 0.1256 ns |  8.92 |    0.27 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_3    | .NET 10.0          | .NET 10.0          |  14.3559 ns | 0.2223 ns | 0.1735 ns |  9.77 |    0.30 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_9    | .NET 10.0          | .NET 10.0          |  23.8741 ns | 0.3271 ns | 0.3060 ns | 16.25 |    0.51 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_0 | .NET 10.0          | .NET 10.0          |   8.5489 ns | 0.1822 ns | 0.1705 ns |  5.82 |    0.20 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_1 | .NET 10.0          | .NET 10.0          |  13.8151 ns | 0.0900 ns | 0.0797 ns |  9.41 |    0.28 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_3 | .NET 10.0          | .NET 10.0          |  15.3800 ns | 0.1836 ns | 0.1717 ns | 10.47 |    0.32 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_9 | .NET 10.0          | .NET 10.0          |  22.2949 ns | 0.3442 ns | 0.3219 ns | 15.18 |    0.49 |      - |         - |        0.00 |
   |                                     |                    |                    |             |           |           |       |         |        |           |             |
   | DotNet_RaiseSync_SyncHandler_0      | .NET 6.0           | .NET 6.0           |   0.5178 ns | 0.0261 ns | 0.0244 ns |  0.14 |    0.01 |      - |         - |        0.00 |
   | DotNet_RaiseSync_SyncHandler_1      | .NET 6.0           | .NET 6.0           |   3.7650 ns | 0.0584 ns | 0.0546 ns |  1.00 |    0.02 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_3      | .NET 6.0           | .NET 6.0           |  10.0952 ns | 0.1084 ns | 0.0905 ns |  2.68 |    0.04 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_9      | .NET 6.0           | .NET 6.0           |  20.2811 ns | 0.1970 ns | 0.1645 ns |  5.39 |    0.09 | 0.0013 |      24 B |        1.00 |
   | Amarok_InvokeSync_SyncHandler_0     | .NET 6.0           | .NET 6.0           |   8.1670 ns | 0.0284 ns | 0.0252 ns |  2.17 |    0.03 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_1     | .NET 6.0           | .NET 6.0           |  12.2479 ns | 0.0645 ns | 0.0503 ns |  3.25 |    0.05 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_3     | .NET 6.0           | .NET 6.0           |  14.3224 ns | 0.1484 ns | 0.1388 ns |  3.80 |    0.06 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_9     | .NET 6.0           | .NET 6.0           |  20.2354 ns | 0.2155 ns | 0.2016 ns |  5.38 |    0.09 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_0    | .NET 6.0           | .NET 6.0           |  16.5251 ns | 0.1095 ns | 0.0970 ns |  4.39 |    0.07 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_1    | .NET 6.0           | .NET 6.0           |  24.8902 ns | 0.3575 ns | 0.3169 ns |  6.61 |    0.12 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_3    | .NET 6.0           | .NET 6.0           |  40.9135 ns | 0.8213 ns | 0.8067 ns | 10.87 |    0.26 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_9    | .NET 6.0           | .NET 6.0           | 103.4454 ns | 1.2615 ns | 1.1800 ns | 27.48 |    0.49 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_0 | .NET 6.0           | .NET 6.0           |   8.2725 ns | 0.0343 ns | 0.0268 ns |  2.20 |    0.03 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_1 | .NET 6.0           | .NET 6.0           |  15.9166 ns | 0.1706 ns | 0.1595 ns |  4.23 |    0.07 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_3 | .NET 6.0           | .NET 6.0           |  20.6348 ns | 0.2145 ns | 0.2007 ns |  5.48 |    0.09 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_9 | .NET 6.0           | .NET 6.0           |  35.7010 ns | 0.6719 ns | 0.6285 ns |  9.48 |    0.21 |      - |         - |        0.00 |
   |                                     |                    |                    |             |           |           |       |         |        |           |             |
   | DotNet_RaiseSync_SyncHandler_0      | .NET 8.0           | .NET 8.0           |   0.5125 ns | 0.0263 ns | 0.0246 ns |  0.26 |    0.01 |      - |         - |        0.00 |
   | DotNet_RaiseSync_SyncHandler_1      | .NET 8.0           | .NET 8.0           |   1.9760 ns | 0.0407 ns | 0.0361 ns |  1.00 |    0.02 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_3      | .NET 8.0           | .NET 8.0           |   9.0080 ns | 0.1110 ns | 0.1039 ns |  4.56 |    0.09 | 0.0013 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_9      | .NET 8.0           | .NET 8.0           |  20.3889 ns | 0.2066 ns | 0.1933 ns | 10.32 |    0.20 | 0.0013 |      24 B |        1.00 |
   | Amarok_InvokeSync_SyncHandler_0     | .NET 8.0           | .NET 8.0           |   8.1889 ns | 0.0823 ns | 0.0770 ns |  4.15 |    0.08 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_1     | .NET 8.0           | .NET 8.0           |  10.8548 ns | 0.1632 ns | 0.1447 ns |  5.49 |    0.12 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_3     | .NET 8.0           | .NET 8.0           |  12.2172 ns | 0.0837 ns | 0.0783 ns |  6.18 |    0.11 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_9     | .NET 8.0           | .NET 8.0           |  20.2804 ns | 0.2507 ns | 0.2345 ns | 10.27 |    0.21 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_0    | .NET 8.0           | .NET 8.0           |  14.6190 ns | 0.2940 ns | 0.5665 ns |  7.40 |    0.31 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_1    | .NET 8.0           | .NET 8.0           |  18.3099 ns | 0.2405 ns | 0.2250 ns |  9.27 |    0.20 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_3    | .NET 8.0           | .NET 8.0           |  22.5759 ns | 0.2777 ns | 0.2319 ns | 11.43 |    0.23 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_9    | .NET 8.0           | .NET 8.0           |  41.1467 ns | 0.4009 ns | 0.3750 ns | 20.83 |    0.41 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_0 | .NET 8.0           | .NET 8.0           |   8.2054 ns | 0.0253 ns | 0.0237 ns |  4.15 |    0.07 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_1 | .NET 8.0           | .NET 8.0           |  13.8616 ns | 0.1126 ns | 0.0879 ns |  7.02 |    0.13 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_3 | .NET 8.0           | .NET 8.0           |  15.4116 ns | 0.1189 ns | 0.1112 ns |  7.80 |    0.15 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_9 | .NET 8.0           | .NET 8.0           |  22.2139 ns | 0.2708 ns | 0.2401 ns | 11.25 |    0.23 |      - |         - |        0.00 |
   |                                     |                    |                    |             |           |           |       |         |        |           |             |
   | DotNet_RaiseSync_SyncHandler_0      | .NET Framework 4.8 | .NET Framework 4.8 |   0.7027 ns | 0.0230 ns | 0.0215 ns |  0.21 |    0.01 |      - |         - |        0.00 |
   | DotNet_RaiseSync_SyncHandler_1      | .NET Framework 4.8 | .NET Framework 4.8 |   3.4185 ns | 0.0863 ns | 0.0807 ns |  1.00 |    0.03 | 0.0038 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_3      | .NET Framework 4.8 | .NET Framework 4.8 |   7.9714 ns | 0.0916 ns | 0.0765 ns |  2.33 |    0.06 | 0.0038 |      24 B |        1.00 |
   | DotNet_RaiseSync_SyncHandler_9      | .NET Framework 4.8 | .NET Framework 4.8 |  15.2271 ns | 0.1786 ns | 0.1670 ns |  4.46 |    0.11 | 0.0038 |      24 B |        1.00 |
   | Amarok_InvokeSync_SyncHandler_0     | .NET Framework 4.8 | .NET Framework 4.8 |  11.6855 ns | 0.1389 ns | 0.1299 ns |  3.42 |    0.09 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_1     | .NET Framework 4.8 | .NET Framework 4.8 |  19.3663 ns | 0.2117 ns | 0.1980 ns |  5.67 |    0.14 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_3     | .NET Framework 4.8 | .NET Framework 4.8 |  17.4488 ns | 0.2319 ns | 0.2056 ns |  5.11 |    0.13 |      - |         - |        0.00 |
   | Amarok_InvokeSync_SyncHandler_9     | .NET Framework 4.8 | .NET Framework 4.8 |  27.7364 ns | 0.2520 ns | 0.2357 ns |  8.12 |    0.20 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_0    | .NET Framework 4.8 | .NET Framework 4.8 |  22.5933 ns | 0.2830 ns | 0.2647 ns |  6.61 |    0.17 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_1    | .NET Framework 4.8 | .NET Framework 4.8 |  31.7098 ns | 0.3020 ns | 0.2825 ns |  9.28 |    0.23 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_3    | .NET Framework 4.8 | .NET Framework 4.8 |  51.6281 ns | 0.6710 ns | 0.5948 ns | 15.11 |    0.39 |      - |         - |        0.00 |
   | Amarok_InvokeAsync_SyncHandler_9    | .NET Framework 4.8 | .NET Framework 4.8 | 113.9140 ns | 1.2083 ns | 1.0711 ns | 33.34 |    0.82 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_0 | .NET Framework 4.8 | .NET Framework 4.8 |  11.5190 ns | 0.1322 ns | 0.1172 ns |  3.37 |    0.08 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_1 | .NET Framework 4.8 | .NET Framework 4.8 |  18.4677 ns | 0.1824 ns | 0.1617 ns |  5.41 |    0.13 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_3 | .NET Framework 4.8 | .NET Framework 4.8 |  25.2114 ns | 0.1806 ns | 0.1690 ns |  7.38 |    0.18 |      - |         - |        0.00 |
   | Amarok_InvokeSync_WeakSyncHandler_9 | .NET Framework 4.8 | .NET Framework 4.8 |  49.6787 ns | 0.9834 ns | 0.9658 ns | 14.54 |    0.43 |      - |         - |        0.00 |
*/
