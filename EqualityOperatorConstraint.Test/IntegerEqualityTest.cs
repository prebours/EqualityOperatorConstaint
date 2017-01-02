using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using NUnit.Framework.Constraints.Operators;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class IntegerEqualityTest : AssertionHelper
    {
        class IntegerEqualityOperatorProvider : IStaticEqualityOperatorProvider
        {
            public bool EvaluateStaticEqualEqualOperator(object lhs, object rhs)
            {
                if (!(lhs is int) || !(rhs is int))
                {
                    throw new ArgumentException();
                }
                return (int)lhs == (int)rhs;
            }

            public bool EvaluateStaticNotEqualOperator(object lhs, object rhs)
            {
                if (!(lhs is int) || !(rhs is int))
                {
                    throw new ArgumentException();
                }
                return (int)lhs != (int)rhs;
            }
        }

        [Test]
        public void TestEquality()
        {
            Constraint constraintEquality = new EqualityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            Assert.IsTrue(constraintEquality.Matches(1));

            Constraint constraintInequality = new InequalityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            Assert.IsFalse(constraintInequality.Matches((int)1));            
        }

        [Test]
        public void TestInequality()
        {
            Constraint constraintInequality = new InequalityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            Assert.IsTrue(constraintInequality.Matches(12));

            Constraint constraintEquality = new EqualityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            Assert.IsFalse(constraintEquality.Matches(12));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestComparisonWithDouble()
        {
            Constraint constraint = new EqualityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            constraint.Matches((double)12);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestComparisonWithNull()
        {
            Constraint constraint = new EqualityOperatorConstraint(1, new IntegerEqualityOperatorProvider());
            constraint.Matches(null);
        }
    }
}
