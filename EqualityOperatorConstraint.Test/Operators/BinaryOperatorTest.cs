using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints.Operators;
using NUnit.Framework;
using NUnit.Mocks;

namespace NUnit.Framework.Constraints.Operators.Tests
{
    [TestFixture]
    public class EqualOperatorTest
    {
        private EqualOperator binaryOperator;

        [SetUp]
        public void Setup()
        {
            binaryOperator = new EqualOperator();
        }

        [Test]
        public void TestInteger()
        {
            Assert.IsTrue(binaryOperator.Evaluate((int)34, (int)34));
            Assert.IsFalse(binaryOperator.Evaluate((int)32, (int)34));
        }


    }

    [TestFixture]
    public class StaticOperatorEqualEqualTest
    {
        [Test]
        public void TestWithMock()
        {
            DynamicMock provider = new DynamicMock(typeof(IStaticEqualityOperatorProvider));
            BinaryOperator binaryOperator = new StaticEqualEqualOperator((IStaticEqualityOperatorProvider)provider.MockInstance);

            provider.ExpectAndReturn("EvaluateStaticEqualEqualOperator", true, "a", 12);
            provider.ExpectNoCall("EvaluateStaticNotEqualOperator");
            Assert.IsTrue(binaryOperator.Evaluate("a", 12));

            provider.Verify();
        }
    }

    [TestFixture]
    public class StaticOperatorNotEqualTest
    {
        [Test]
        public void TestWithMock()
        {
            DynamicMock provider = new DynamicMock(typeof(IStaticEqualityOperatorProvider));
            BinaryOperator binaryOperator = new StaticNotEqualOperator((IStaticEqualityOperatorProvider)provider.MockInstance);

            provider.ExpectAndReturn("EvaluateStaticNotEqualOperator", true, "a", 12);
            provider.ExpectNoCall("EvaluateStaticEqualEqualOperator");
            Assert.IsTrue(binaryOperator.Evaluate("a", 12));

            provider.Verify();
        }
    }

    [TestFixture]
    public class GetHashCodeOperatorTest
    {
        class Example
        {
            private readonly int hashCode;

            public Example(int hashCode)
            {
                this.hashCode = hashCode;
            }

            public override int GetHashCode()
            {
                return hashCode;
            }
        }

        [Test]
        public void TestWithInteger()
        {
            BinaryOperator hashCodeOperator = new GetHashCodeOperator();
            Assert.IsTrue(hashCodeOperator.Evaluate(10, 10));
        }

        [Test]
        public void TestWithExample()
        {
            BinaryOperator hashCodeOperator = new GetHashCodeOperator();
            Assert.IsTrue(hashCodeOperator.Evaluate(new Example(34), new Example(34)));
            Assert.IsFalse(hashCodeOperator.Evaluate(new Example(33), new Example(34)));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWithLHSNull()
        {
            BinaryOperator hashCodeOperator = new GetHashCodeOperator();
            hashCodeOperator.Evaluate(null, new Example(36));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWithRHSNull()
        {
            BinaryOperator hashCodeOperator = new GetHashCodeOperator();
            hashCodeOperator.Evaluate(new Example(24), null);
        }

    }
}
