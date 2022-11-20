// Copyright (c) 2022, Olaf Kober <olaf.kober@outlook.com>

using NFluent;
using NUnit.Framework;


namespace Amarok.Events;


[TestFixture]
public class Test_NullSubscription
{
    [Test]
    public void Instance_Returns_SameObject()
    {
        var s1 = NullSubscription.Instance;
        var s2 = NullSubscription.Instance;

        Check.That(s1).IsNotNull().And.IsSameReferenceAs(s2);
    }

    [Test]
    public void Dispose_DoesNothing()
    {
        var s = NullSubscription.Instance;

        Check.ThatCode(() => s.Dispose()).DoesNotThrow();
    }
}
