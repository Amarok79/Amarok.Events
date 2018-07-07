---


---

<h2 id="introduction">Introduction</h2>
<p>This library provides a fast and light-weight implementation of the observer pattern, which can be used as replacement for ordinary .NET events. The implementation supports raising events in a <em>synchronous, blocking</em> or <em>asynchronous, await-able</em> fashion. In addition, both <em>synchronous</em> and <em>asynchronous</em> event handler can be registered.</p>
<p>The implementation is a bit slower than ordinary .NET events in regard to call performance, but optimized to avoid allocations and therefore suitable for high-performance scenarios or resource-constraint embedded systems.</p>
<p>The library is redistributed as NuGet package: <a href="https://www.nuget.org/packages/Amarok.Events/">Amarok.Events</a> (currently in beta)</p>
<p>The package provides binaries for <em>.NET Standard 2.0</em> only. Tests are performed with <em>.NET Framework 4.7.1</em> and <em>.NET Core 2.1</em>.</p>
<p>For development, you need <em>Visual Studio 2017</em> (v15.7 or later).</p>

