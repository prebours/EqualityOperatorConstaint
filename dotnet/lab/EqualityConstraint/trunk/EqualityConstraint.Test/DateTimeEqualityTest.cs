using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints.Operators;
using NUnit.Framework.Constraints;

namespace NUnit.TestData.EqualityConstraint.Test
{
    [TestFixture]
    public class DateTimeEqualityTest
    {
        [Test]
        public void TestDateTimeWithDelegatedStaticEqualityOperatorProvider()
        {
            IStaticEqualityOperatorProvider provider = new DelegatedStaticEqualityOperatorProvider<DateTime>(
                delegate(DateTime lhs, DateTime rhs)
                {
                    return lhs == rhs;
                },
                delegate(DateTime lhs, DateTime rhs)
                {
                    return lhs != rhs;
                });

            DateTime date1 = DateTime.Now;
            DateTime date2 = date1;

            Constraint equalityConstraint = new EqualityOperatorConstraint(date2, provider);

            Assert.IsTrue(equalityConstraint.Matches(date1));
            Assert.IsTrue(equalityConstraint.Matches(date2));

        }

    }
}
