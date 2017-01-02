using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Constraints.Operators;

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class StandardEqualityTest
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
                    return lhs.Name == rhs.Name;
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

        class CustomerWithBadEqualImplementation : Customer
        {
            public CustomerWithBadEqualImplementation(string name)
                : base(name)
            {

            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(obj, this))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        class CustomerWithConstantGetHashCodeImplementation : Customer
        {
            public CustomerWithConstantGetHashCodeImplementation(string name)
                : base(name)
            {

            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }


        class CustomerWithAlwaysVariableGetHashCodeImplementation : Customer
        {
            private readonly static Random s_random = new Random();

            public CustomerWithAlwaysVariableGetHashCodeImplementation(string name)
                : base(name)
            {

            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return s_random.NextDouble().GetHashCode();
            }
        }



        public class Simple : IStaticEqualityOperatorProvider
        {
            public bool EvaluateStaticEqualEqualOperator(object lhs, object rhs)
            {
                throw new NotImplementedException();
            }

            public bool EvaluateStaticNotEqualOperator(object lhs, object rhs)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestCustomerWithEquality()
        {
            Customer customer = new Customer("James Bond");
            Constraint constraint = new EqualityOperatorConstraint(customer, new ReflectiveStaticEqualityOperatorProvider<Customer>());
            
            Assert.IsTrue(constraint.Matches(new Customer("James Bond")));
            Assert.IsTrue(constraint.Matches(customer));
        }

        [Test]
        public void TestCustomerWithInequality()
        {
            Customer customer = new Customer("James Bond");
            Constraint constraint = new InequalityOperatorConstraint(customer, new ReflectiveStaticEqualityOperatorProvider<Customer>());

            Assert.IsTrue(constraint.Matches(new Customer("Superman")));
        }

        [Test]
        public void TestCustomerWithBadEqualsImplementation()
        {
            Customer customer = new CustomerWithBadEqualImplementation("James Bond");
            Constraint constraint = new EqualityOperatorConstraint(customer, new ReflectiveStaticEqualityOperatorProvider<Customer>());
            Assert.IsFalse(constraint.Matches(new CustomerWithBadEqualImplementation("James Bond")));
        }

        [Test]
        public void TestCustomerWithConstantGetHashCodeImplementation()
        {
            Customer customer = new CustomerWithAlwaysVariableGetHashCodeImplementation("James Bond");
            Constraint constraint = new EqualityOperatorConstraint(customer, new ReflectiveStaticEqualityOperatorProvider<Customer>());
            Assert.IsFalse(constraint.Matches(new CustomerWithAlwaysVariableGetHashCodeImplementation("James Bond")));
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestCustomerWhereStaticEqualEqualOperatorDoesNotCheckNullValues()
        {
            Customer customer = new Customer("James Bond");
            Constraint constraint = new InequalityOperatorConstraint(customer, new DelegatedStaticEqualityOperatorProvider<Customer>(
                delegate(Customer lhs, Customer rhs)
                {
                    return lhs.Name == rhs.Name;
                },
                delegate(Customer lhs, Customer rhs)
                {
                    return lhs != rhs; // we use the standard implementation
                }));

            constraint.Matches(new Customer("Superman"));
        }

        [Test]
        public void TestWithActualAndExpectedNull()
        {
            Constraint constraint = new EqualityOperatorConstraint(null, new ReflectiveStaticEqualityOperatorProvider<Customer>());
            Assert.IsTrue(constraint.Matches(null));
        }

        [Test]
        public void TestWithActualNull()
        {
            Constraint constraint = new InequalityOperatorConstraint(new Customer("James Bond"), new ReflectiveStaticEqualityOperatorProvider<Customer>());
            Assert.IsTrue(constraint.Matches(null));            
        }

        [Test]
        public void TestWithExpectedNull()
        {
            Constraint constraint = new InequalityOperatorConstraint(null, new ReflectiveStaticEqualityOperatorProvider<Customer>());
            Assert.IsTrue(constraint.Matches(new Customer("James Bond")));
        }

        [Test]
        public void TestCustomerWhereStaticEqualEqualOperatorReturnFalseWhenLhsIsNullAndRhsIsNull()
        {
            Customer customer = new Customer("James Bond");
            Constraint constraint = new InequalityOperatorConstraint(customer, new DelegatedStaticEqualityOperatorProvider<Customer>(
                delegate(Customer lhs, Customer rhs)
                {
                    if (lhs == null && rhs == null)
                    {
                        return false;
                    }
                    else
                    {
                        return lhs == rhs;
                    }
                },
                delegate(Customer lhs, Customer rhs)
                {
                    return lhs != rhs; // we use the standard implementation
                }));

            Assert.IsFalse(constraint.Matches(new Customer("Superman")));
        }
    }
}
