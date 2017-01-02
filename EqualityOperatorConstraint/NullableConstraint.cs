using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints
{
    public class BaseNullableConstraint : Constraint
    {
        private readonly bool expectedToBeNullable;

        protected BaseNullableConstraint(bool expectedToBeNull)
        {
            this.expectedToBeNullable = expectedToBeNull;
        }

        private bool IsNullable(object actual)
        {
            if (object.ReferenceEquals(actual, null))
            {
                return true;
            }
            else if (actual.GetType().IsValueType)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool Matches(object actual)
        {
            return IsNullable(actual) == expectedToBeNullable;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate(string.Format("is {0}nullable", expectedToBeNullable ? string.Empty : "not "));
            writer.WriteExpectedValue(expectedToBeNullable);
        }
    }

    /// <summary>
    /// Test if the value is nullable
    /// </summary>
    public class NullableConstraint : BaseNullableConstraint
    {
        public NullableConstraint()
            : base(true)
        {

        }
    }

    /// <summary>
    /// Test if the value is a value type (i.e. not nullable)
    /// </summary>
    public class NotNullableConstraint : BaseNullableConstraint
    {
        public NotNullableConstraint()
            : base(false)
        {
        }
    }
}
