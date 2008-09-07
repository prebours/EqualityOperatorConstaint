using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class NullConstraintTest
    {
        [Test]
        public void TestNull()
        {
            Constraint constraint = new NullConstraint();
            Assert.IsTrue(constraint.Matches(null));
        }

        [Test]
        public void TestInteger()
        {
            int i = 10;

            Constraint constraint = new NullConstraint();
            Assert.IsFalse(constraint.Matches(i));
        }

        [Test]
        public void TestNullInteger()
        {
            int? i = null;

            Constraint constraint = new NullConstraint();
            Assert.IsTrue(constraint.Matches(i));
        }
    }
}
