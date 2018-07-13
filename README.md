## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as replacement for ordinary .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. In addition, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events in regard to raw call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.


## Redistribution

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/)

The package provides strong-named binaries for *.NET Standard 2.0* only. Tests are generally performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.


## Further Links

For a comprehensive documentation about how to use this library, refer to [Documentation](doc/Documentation.md).


