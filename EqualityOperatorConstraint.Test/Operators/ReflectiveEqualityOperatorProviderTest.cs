using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Framework.Constraints.Operators.Tests
{
    [TestFixture]
    public class ReflectiveEqualityOperatorProviderTest
    {

        class FirstExample
        {
            public readonly bool result;

            public FirstExample(bool result)
            {
                this.result = result;
            }

            public static bool operator ==(FirstExample lhs, FirstExample rhs)
            {
                return lhs.result;
            }

            public static bool operator !=(FirstExample lhs, FirstExample rhs)
            {
                return !lhs.result;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        class SecondExample
        {
            public readonly bool result;

            public SecondExample(bool result)
            {
                this.result = result;
            }

            public static bool operator ==(SecondExample lhs, FirstExample rhs)
            {
                return lhs.result;
            }

            public static bool operator ==(FirstExample lhs, SecondExample rhs)
            {
                return lhs.result;
            }

            public static bool operator !=(FirstExample lhs, SecondExample rhs)
            {
                return !lhs.result;
            }


            public static bool operator !=(SecondExample lhs, FirstExample rhs)
            {
                return !lhs.result;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
          
        }

        class ThirdExample
        {

        }


        [Test]
        public void TestWithExample()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<FirstExample>();

            Assert.IsTrue(provider.EvaluateStaticEqualEqualOperator(new FirstExample(true), null));
            Assert.IsFalse(provider.EvaluateStaticEqualEqualOperator(new FirstExample(false), null));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEvaluateStaticEqualEqualOperatorWithExampleWithIncorrectParameters()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<FirstExample>();
            provider.EvaluateStaticEqualEqualOperator(new FirstExample(true), (int)12);            
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEvaluateStaticEqualEqualOperatorWithSecondExampleWithParameterOfWrongType()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<SecondExample>();
           provider.EvaluateStaticEqualEqualOperator(new SecondExample(true), new FirstExample(false));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEvaluateStaticNotEqualOperatorWithSecondExampleWithParameterOfWrongType()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<SecondExample>();
            provider.EvaluateStaticNotEqualOperator(new SecondExample(true), new FirstExample(false));
        }

        [Test]
        [ExpectedException(typeof(ReflectiveStaticNotEqualOperatorNotFound))]
        public void TestEvaluateStaticNotEqualWithThirdExample()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<ThirdExample>();
            provider.EvaluateStaticNotEqualOperator(new ThirdExample(), null);

        }

        [Test]
        [ExpectedException(typeof(ReflectiveStaticEqualEqualOperatorNotFound))]
        public void TestEvaluateStaticEqualEqualWithThirdExample()
        {
            IStaticEqualityOperatorProvider provider = new ReflectiveStaticEqualityOperatorProvider<ThirdExample>();
            provider.EvaluateStaticEqualEqualOperator(new ThirdExample(), null);

        }
    }
}
