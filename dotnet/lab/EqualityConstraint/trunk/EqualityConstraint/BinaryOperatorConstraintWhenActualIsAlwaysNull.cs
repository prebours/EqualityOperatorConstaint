using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints.Operators;

namespace NUnit.Framework.Constraints
{
    class ConstraintWhenActualIsAlwaysNull : Constraint
    {
        private readonly Constraint innerConstraint;

        public ConstraintWhenActualIsAlwaysNull(Constraint innerConstraint)
        {
            this.innerConstraint = innerConstraint;
        }

        public override bool Matches(object actual)
        {
            return innerConstraint.Matches(null);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            innerConstraint.WriteDescriptionTo(writer);
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            innerConstraint.WriteMessageTo(writer);
        }
    }
}
