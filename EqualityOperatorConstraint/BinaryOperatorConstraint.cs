using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints.Operators;
using System.Text.RegularExpressions;


namespace NUnit.Framework.Constraints
{
    public enum ParameterOrder
    {
        ActualIsLeftSideOperator,
        ActualIsRightSideOperator
    }

    public class BinaryOperatorConstraint : Constraint
    {
        private readonly ParameterOrder order;
        private readonly bool expectedValue;
        private readonly object expected;
        private readonly BinaryOperator binaryOperator;

        public event BinaryOperatorConstraintMatchHandler OnMatch;

        public BinaryOperatorConstraint(object expected,
            bool expectedValue,
            ParameterOrder order,
            BinaryOperator binaryOperator)
        {
            this.expected = expected;
            this.expectedValue = expectedValue;
            this.order = order;
            this.binaryOperator = binaryOperator;
        }

        public object Expected {
            get {
                return expected;
            }
        }

        public bool ExpectedValue
        {
            get
            {
                return expectedValue;
            }
        }

        public BinaryOperator BinaryOperator
        {
            get
            {
                return binaryOperator;
            }
        }

        public ParameterOrder ParameterOrder
        {
            get
            {
                return order;
            }
        }

        public override bool Matches(object actual)
        {
            object lhs = order == ParameterOrder.ActualIsLeftSideOperator ? actual : expected;
            object rhs = order == ParameterOrder.ActualIsRightSideOperator ? actual : expected;

            bool isMatch = binaryOperator.Evaluate(lhs, rhs) == expectedValue;

            if (OnMatch != null)
            {
                OnMatch(this, new BinaryOperatorConstraintMatchEventArgs(this, isMatch, actual));
            }

            return isMatch;
        }
       

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            string lhsName = order == ParameterOrder.ActualIsLeftSideOperator ? "actual" : "expected";
            string rhsName = order == ParameterOrder.ActualIsRightSideOperator ? "actual" : "expected";

            writer.WritePredicate(binaryOperator.Description(lhsName, rhsName));
            writer.WriteExpectedValue(expectedValue);
        }
    }

    public class BinaryOperatorConstraintMatchEventArgs : EventArgs {
        private readonly BinaryOperatorConstraint constraint;
        private readonly bool isMatch;
        private readonly object actual;

        public BinaryOperatorConstraintMatchEventArgs(BinaryOperatorConstraint constraint, bool isMatch, object actual) {
            this.constraint = constraint;
            this.isMatch = isMatch;
            this.actual = actual;
        }

        public ParameterOrder ParameterOrder
        {
            get {
                return constraint.ParameterOrder;
            }
        }

        public object Expected
        {
            get
            {
                return constraint.Expected;               
            }
        }

        public bool ExpectedValue
        {
            get
            {
                return constraint.ExpectedValue;
            }
        }

        public bool ActualValue
        {
            get
            {
                if (isMatch)
                {
                    return ExpectedValue;
                }
                else
                {
                    return !ExpectedValue;
                }
            }
        }

        public string BinaryOperatorDescription(string lhsName, string rhsName)
        {
            return constraint.BinaryOperator.Description(lhsName, rhsName);
        }

        public bool IsMatch
        {
            get
            {
                return isMatch;
            }
        }

        public object Actual
        {
            get
            {
                return actual;
            }
        }
    }

    public delegate void BinaryOperatorConstraintMatchHandler(object sender, BinaryOperatorConstraintMatchEventArgs args);

    

}
