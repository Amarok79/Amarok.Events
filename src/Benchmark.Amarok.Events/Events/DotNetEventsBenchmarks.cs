/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;


namespace Amarok.Events
{
	[ClrJob, CoreJob, MemoryDiagnoser]
	public class DotNetEventsBenchmarks
	{
		public sealed class FooEventArgs : EventArgs
		{
			public String Payload { get; }

			public FooEventArgs(String payload)
			{
				this.Payload = payload;
			}
		}

		public interface IFooService
		{
			event EventHandler<FooEventArgs> FooChanged;
		}

		internal sealed class FooServiceImpl : IFooService
		{
			public event EventHandler<FooEventArgs> FooChanged;

			public void Do()
			{
				FooChanged?.Invoke(this, new FooEventArgs("A"));
			}
		}

		private void HandleFooChanged(Object sender, FooEventArgs e)
		{
		}


		private FooServiceImpl mServiceHandlerCount0;
		private FooServiceImpl mServiceHandlerCount1;
		private FooServiceImpl mServiceHandlerCount3;
		private FooServiceImpl mServiceHandlerCount5;
		private FooServiceImpl mServiceHandlerCount9;


		public DotNetEventsBenchmarks()
		{
			mServiceHandlerCount0 = new FooServiceImpl();

			mServiceHandlerCount1 = new FooServiceImpl();
			mServiceHandlerCount1.FooChanged += HandleFooChanged;

			mServiceHandlerCount3 = new FooServiceImpl();
			mServiceHandlerCount3.FooChanged += HandleFooChanged;
			mServiceHandlerCount3.FooChanged += HandleFooChanged;
			mServiceHandlerCount3.FooChanged += HandleFooChanged;

			mServiceHandlerCount5 = new FooServiceImpl();
			mServiceHandlerCount5.FooChanged += HandleFooChanged;
			mServiceHandlerCount5.FooChanged += HandleFooChanged;
			mServiceHandlerCount5.FooChanged += HandleFooChanged;
			mServiceHandlerCount5.FooChanged += HandleFooChanged;
			mServiceHandlerCount5.FooChanged += HandleFooChanged;

			mServiceHandlerCount9 = new FooServiceImpl();
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
			mServiceHandlerCount9.FooChanged += HandleFooChanged;
		}


		[Benchmark]
		public void Raise_HandlerCount_0()
		{
			mServiceHandlerCount0.Do();
		}

		[Benchmark]
		public void Raise_HandlerCount_1()
		{
			mServiceHandlerCount1.Do();
		}

		[Benchmark]
		public void Raise_HandlerCount_3()
		{
			mServiceHandlerCount3.Do();
		}

		[Benchmark]
		public void Raise_HandlerCount_5()
		{
			mServiceHandlerCount5.Do();
		}

		[Benchmark]
		public void Raise_HandlerCount_9()
		{
			mServiceHandlerCount9.Do();
		}

		//	BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
		//	Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
		//	Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
		//	.NET Core SDK=2.1.300
		//	  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
		//	  Clr    : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3110.0
		//	  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


		//	               Method |  Job | Runtime |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
		//	--------------------- |----- |-------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
		//	 Raise_HandlerCount_0 |  Clr |     Clr |  1.2960 ns | 0.0625 ns | 0.0585 ns |  1.2724 ns |      - |       0 B |
		//	 Raise_HandlerCount_1 |  Clr |     Clr |  7.9731 ns | 0.1993 ns | 0.5355 ns |  7.7345 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_3 |  Clr |     Clr | 16.6088 ns | 0.3680 ns | 1.0259 ns | 16.3498 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_5 |  Clr |     Clr | 20.2713 ns | 0.4222 ns | 0.3526 ns | 20.2630 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_9 |  Clr |     Clr | 30.3619 ns | 0.7340 ns | 0.8158 ns | 30.1317 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_0 | Core |    Core |  0.9000 ns | 0.0627 ns | 0.1798 ns |  0.8521 ns |      - |       0 B |
		//	 Raise_HandlerCount_1 | Core |    Core |  6.9822 ns | 0.1211 ns | 0.0946 ns |  6.9841 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_3 | Core |    Core | 19.9962 ns | 0.4343 ns | 0.9979 ns | 19.4473 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_5 | Core |    Core | 29.2388 ns | 0.6118 ns | 0.8374 ns | 29.1110 ns | 0.0076 |      24 B |
		//	 Raise_HandlerCount_9 | Core |    Core | 42.8381 ns | 0.8959 ns | 2.0584 ns | 41.8598 ns | 0.0076 |      24 B |	
	}
}
