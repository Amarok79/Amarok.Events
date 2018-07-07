## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as replacement for ordinary .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. In addition, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events in regard to call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/) (currently in beta)

The package provides binaries for *.NET Standard 2.0* only. Tests are performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.

For development, you need *Visual Studio 2017* (v15.7 or later).
