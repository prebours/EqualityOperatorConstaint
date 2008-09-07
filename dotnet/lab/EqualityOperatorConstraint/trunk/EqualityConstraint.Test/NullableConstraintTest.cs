using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class NullableConstraintTest
    {
        [Test]
        public void TestWithInteger()
        {
            int actual = 1;

            Constraint constraint = new NullableConstraint();
            Assert.IsFalse(constraint.Matches(actual));
        }

        [Test]
        public void TestWithNull()
        {            
            Constraint constraint = new NullableConstraint();
            Assert.IsTrue(constraint.Matches(null));
        }
      

        [Test]
        public void TestWithClass()
        {
            StringBuilder stringBuilder = new StringBuilder();

            Constraint constraint = new NullableConstraint();
            Assert.IsTrue(constraint.Matches(stringBuilder));
        }

        [Test]
        public void TestWithStruct()
        {
            DateTime dateTime = DateTime.Now;

            Constraint constraint = new NullableConstraint();
            Assert.IsFalse(constraint.Matches(dateTime));
        }

        
    }
}
