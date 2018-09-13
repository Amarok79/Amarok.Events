[![Build Status](https://dev.azure.com/amarok79/Amarok.Events/_apis/build/status/Amarok79.Events)](https://dev.azure.com/amarok79/Amarok.Events/_build/latest?definitionId=1)
![NuGet](https://img.shields.io/nuget/v/Amarok.Events.svg?logo=)(https://www.nuget.org/packages/Amarok.Events/)

## Introduction

This library provides a fast and light-weight implementation of the observer pattern, which can be used as a replacement for regular .NET events. The implementation supports raising events in a *synchronous, blocking* or *asynchronous, await-able* fashion. Besides, both *synchronous* and *asynchronous* event handler can be registered.

The implementation is a bit slower than ordinary .NET events regarding raw call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.


## Redistribution

The library is redistributed as NuGet package: [Amarok.Events](https://www.nuget.org/packages/Amarok.Events/)

The package provides strong-named binaries for *.NET Standard 2.0* only. Tests are generally performed with *.NET Framework 4.7.1* and *.NET Core 2.1*.


## Further Links

For comprehensive documentation about how to use this library, refer to [Documentation](doc/Documentation.md).

Also, visit the [FAQ](doc/FAQ.md) which hopefully answers your questions.
