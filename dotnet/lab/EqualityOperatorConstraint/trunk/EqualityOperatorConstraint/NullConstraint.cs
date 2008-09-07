using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints
{
   
    public class BaseNullConstraint : Constraint
    { 
        private readonly bool expectedToBeNull;

        protected BaseNullConstraint(bool expectedToBeNull)
        {
            this.expectedToBeNull = expectedToBeNull;
        }

        public override bool Matches(object actual)
        {
            return object.ReferenceEquals(actual, null) == expectedToBeNull;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {         
            writer.WritePredicate("object.ReferenceEquals(actual, null)");
            writer.WriteExpectedValue(expectedToBeNull);
        }
    }

    /// <summary>
    /// Test if the value is null
    /// </summary>
    public class NullConstraint : BaseNullConstraint
    {
        public NullConstraint()
            : base(true)
        {

        }
    }

    /// <summary>
    /// Test if the value is not null
    /// </summary>
    public class NotNullConstraint : BaseNullConstraint
    {
        public NotNullConstraint()
            : base(false)
        {

        }
    }
}
