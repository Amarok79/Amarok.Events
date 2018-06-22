/* Copyright(c) 2018, Olaf Kober
 * Licensed under GNU Lesser General Public License v3.0 (LPGL-3.0).
 * https://github.com/Amarok79/Events
 */

using NFluent;
using NUnit.Framework;


namespace Amarok.Events
{
	[TestFixture]
	public class Test_NullSubscription
	{
		[Test]
		public void Instance_Returns_SameObject()
		{
			var s1 = NullSubscription.Instance;
			var s2 = NullSubscription.Instance;

			Check.That(s1)
				.IsNotNull()
				.And
				.IsSameReferenceAs(s2);
		}

		[Test]
		public void Dispose_DoesNothing()
		{
			var s = NullSubscription.Instance;

			Check.ThatCode(() => s.Dispose())
				.DoesNotThrow();
		}
	}
}
