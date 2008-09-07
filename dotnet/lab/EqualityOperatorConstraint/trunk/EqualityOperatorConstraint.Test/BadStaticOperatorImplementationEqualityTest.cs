using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;
using NUnit.Framework.Constraints.Operators;
using NUnit.Framework;

namespace NUnit.TestData.EqualityConstraint.Test
{
    [TestFixture]
    public class BadStaticOperatorImplementationEqualityTest
    {

        class Customer
        {
            private readonly string name;

            public Customer(string name)
            {
                this.name = name;
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }


            public override bool Equals(object obj)
            {
                Customer customer = obj as Customer;
                if (customer != null)
                {
                    return this == customer;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }

            public static bool operator ==(Customer lhs, Customer rhs)
            {
                if (object.ReferenceEquals(lhs, rhs))
                {
                    return true;
                }
                else if ((object)lhs == null || (object)rhs == null)
                {
                    return false;
                }
                else
                {
                    return lhs.Name.Length == rhs.Name.Length;
                }
            }

            public static bool operator !=(Customer lhs, Customer rhs)
            {
                return !(lhs == rhs);
            }

            public override string ToString()
            {
                return Name;
            }
        }

        [Test]
        public void TestInequalityWithTwoNamesOfDifferentLength()
        {
            Customer customer = new Customer("James");
            Constraint constraint = new InequalityOperatorConstraint(customer,
                    new ReflectiveStaticEqualityOperatorProvider<Customer>());

            Assert.IsTrue(constraint.Matches(new Customer("Alexandre")));
        }

        [Test]
        public void TestInequalityWithTwoNamesOfSameLength()
        {
            Customer customer = new Customer("James");
            Constraint constraint = new InequalityOperatorConstraint(customer,
                    new ReflectiveStaticEqualityOperatorProvider<Customer>());

            Assert.IsFalse(constraint.Matches(new Customer("Henry")));
          
            Assert.That(new Customer("Henry"), new InequalityOperatorConstraint(new Customer("James"),
                    new ReflectiveStaticEqualityOperatorProvider<Customer>()));
        }

        [Test]
        public void TestEqualityWithTwoNamesOfSameLength()
        {
            Customer customer = new Customer("James");
            Constraint constraint = new EqualityOperatorConstraint(customer,
                    new ReflectiveStaticEqualityOperatorProvider<Customer>());

            Assert.IsFalse(constraint.Matches(new Customer("Henry")));

            Assert.That(new Customer("Henry"), new EqualityOperatorConstraint(new Customer("James"),
                   new ReflectiveStaticEqualityOperatorProvider<Customer>()));
           
        }
    }
}
