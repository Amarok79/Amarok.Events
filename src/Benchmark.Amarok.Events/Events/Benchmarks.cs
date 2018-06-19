/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;


namespace Amarok.Events
{
	[ClrJob, /*CoreJob, */MemoryDiagnoser]
	public class Benchmarks
	{
		public class DotNet
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

			public sealed class FooServiceImpl : IFooService
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


			public FooServiceImpl ServiceHandlerCount0;
			public FooServiceImpl ServiceHandlerCount1;
			public FooServiceImpl ServiceHandlerCount3;
			public FooServiceImpl ServiceHandlerCount5;
			public FooServiceImpl ServiceHandlerCount9;

			public DotNet()
			{
				ServiceHandlerCount0 = new FooServiceImpl();

				ServiceHandlerCount1 = new FooServiceImpl();
				ServiceHandlerCount1.FooChanged += HandleFooChanged;

				ServiceHandlerCount3 = new FooServiceImpl();
				ServiceHandlerCount3.FooChanged += HandleFooChanged;
				ServiceHandlerCount3.FooChanged += HandleFooChanged;
				ServiceHandlerCount3.FooChanged += HandleFooChanged;

				ServiceHandlerCount5 = new FooServiceImpl();
				ServiceHandlerCount5.FooChanged += HandleFooChanged;
				ServiceHandlerCount5.FooChanged += HandleFooChanged;
				ServiceHandlerCount5.FooChanged += HandleFooChanged;
				ServiceHandlerCount5.FooChanged += HandleFooChanged;
				ServiceHandlerCount5.FooChanged += HandleFooChanged;

				ServiceHandlerCount9 = new FooServiceImpl();
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
				ServiceHandlerCount9.FooChanged += HandleFooChanged;
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
				private readonly EventSource<String> mFooChanged = new EventSource<String>();

				public Event<String> FooChanged => mFooChanged.Event;

				public void Do()
				{
					mFooChanged.Invoke("A");
				}
			}

			private void HandleFooChanged(String args)
			{
			}


			public FooServiceImpl ServiceHandlerCount0;
			public FooServiceImpl ServiceHandlerCount1;
			public FooServiceImpl ServiceHandlerCount3;
			public FooServiceImpl ServiceHandlerCount5;
			public FooServiceImpl ServiceHandlerCount9;

			public Amarok()
			{
				ServiceHandlerCount0 = new FooServiceImpl();

				ServiceHandlerCount1 = new FooServiceImpl();
				ServiceHandlerCount1.FooChanged.Subscribe(HandleFooChanged);

				ServiceHandlerCount3 = new FooServiceImpl();
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount3.FooChanged.Subscribe(HandleFooChanged);

				ServiceHandlerCount5 = new FooServiceImpl();
				ServiceHandlerCount5.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount5.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount5.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount5.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount5.FooChanged.Subscribe(HandleFooChanged);

				ServiceHandlerCount9 = new FooServiceImpl();
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
				ServiceHandlerCount9.FooChanged.Subscribe(HandleFooChanged);
			}
		}


		private readonly DotNet mDotNet;
		private readonly Amarok mAmarok;


		public Benchmarks()
		{
			mDotNet = new DotNet();
			mAmarok = new Amarok();
		}


		//[Benchmark]
		//public void DotNet_Raise_HandlerCount_0()
		//{
		//	mDotNet.ServiceHandlerCount0.Do();
		//}

		[Benchmark(Baseline = true)]
		public void DotNet_Raise_HandlerCount_1()
		{
			mDotNet.ServiceHandlerCount1.Do();
		}

		//[Benchmark]
		//public void DotNet_Raise_HandlerCount_3()
		//{
		//	mDotNet.ServiceHandlerCount3.Do();
		//}

		//[Benchmark]
		//public void DotNet_Raise_HandlerCount_5()
		//{
		//	mDotNet.ServiceHandlerCount5.Do();
		//}

		//[Benchmark]
		//public void DotNet_Raise_HandlerCount_9()
		//{
		//	mDotNet.ServiceHandlerCount9.Do();
		//}


		//[Benchmark]
		//public void Amarok_Raise_HandlerCount_0()
		//{
		//	mAmarok.ServiceHandlerCount0.Do();
		//}

		[Benchmark]
		public void Amarok_Raise_HandlerCount_1()
		{
			mAmarok.ServiceHandlerCount1.Do();
		}

		//[Benchmark]
		//public void Amarok_Raise_HandlerCount_3()
		//{
		//	mAmarok.ServiceHandlerCount3.Do();
		//}

		//[Benchmark]
		//public void Amarok_Raise_HandlerCount_5()
		//{
		//	mAmarok.ServiceHandlerCount5.Do();
		//}

		//[Benchmark]
		//public void Amarok_Raise_HandlerCount_9()
		//{
		//	mAmarok.ServiceHandlerCount9.Do();
		//}



		//	BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
		//	Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
		//	Frequency=2531251 Hz, Resolution=395.0616 ns, Timer=TSC
		//	.NET Core SDK=2.1.300
		//	  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
		//	  Clr    : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3110.0
		//	  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


		//	               Method |  Job | Runtime |      Mean |     Error |    StdDev |    Median | Allocated |
		//	--------------------- |----- |-------- |----------:|----------:|----------:|----------:|----------:|
		//	 Raise_HandlerCount_0 |  Clr |     Clr |  2.766 ns | 0.0943 ns | 0.1291 ns |  2.721 ns |       0 B |
		//	 Raise_HandlerCount_1 |  Clr |     Clr | 10.377 ns | 0.2267 ns | 0.2010 ns | 10.353 ns |       0 B |
		//	 Raise_HandlerCount_3 |  Clr |     Clr | 20.191 ns | 0.2030 ns | 0.1799 ns | 20.250 ns |       0 B |
		//	 Raise_HandlerCount_5 |  Clr |     Clr | 32.490 ns | 0.6872 ns | 1.9041 ns | 32.135 ns |       0 B |
		//	 Raise_HandlerCount_9 |  Clr |     Clr | 50.709 ns | 0.6106 ns | 0.4767 ns | 50.708 ns |       0 B |
		//	 Raise_HandlerCount_0 | Core |    Core |  2.018 ns | 0.0913 ns | 0.1051 ns |  1.993 ns |       0 B |
		//	 Raise_HandlerCount_1 | Core |    Core |  8.622 ns | 0.2513 ns | 0.2894 ns |  8.509 ns |       0 B |
		//	 Raise_HandlerCount_3 | Core |    Core | 16.591 ns | 0.1582 ns | 0.1235 ns | 16.622 ns |       0 B |
		//	 Raise_HandlerCount_5 | Core |    Core | 24.190 ns | 0.5631 ns | 0.6485 ns | 23.966 ns |       0 B |
		//	 Raise_HandlerCount_9 | Core |    Core | 40.276 ns | 0.8409 ns | 1.9152 ns | 39.589 ns |       0 B |	
	}
}
