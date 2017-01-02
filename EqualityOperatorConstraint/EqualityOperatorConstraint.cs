using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Constraints.Operators;

namespace NUnit.Framework.Constraints
{
    public class BaseEqualityOperatorConstraint : Constraint
    {
        private readonly object expected;
        private readonly IStaticEqualityOperatorProvider equalityOperatorProvider;
        private readonly bool expectedValueIfActualIsEqualToExpected;

        private Constraint innerConstraint;
        private string failingMessage;

        protected BaseEqualityOperatorConstraint(object expected,
            bool expectedValueIfActualIsEqualToExpected,
            IStaticEqualityOperatorProvider equalityOperatorProvider)
        {
            this.expected = expected;
            this.expectedValueIfActualIsEqualToExpected = expectedValueIfActualIsEqualToExpected;
            this.equalityOperatorProvider = equalityOperatorProvider;
        }

        private void constraint_OnMatch(object sender, BinaryOperatorConstraintMatchEventArgs args)
        {
            if (!args.IsMatch && failingMessage == null)
            {
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.Append(
                   args.BinaryOperatorDescription(
                   args.ParameterOrder == ParameterOrder.ActualIsLeftSideOperator ? "actual" : "expected",
                   args.ParameterOrder == ParameterOrder.ActualIsRightSideOperator ? "actual" : "expected")); ;
                messageBuilder.AppendFormat(" where \"actual\" was {0} and \"expected\" was {1}",
                    FormatValue(args.Actual), FormatValue(args.Expected));
                messageBuilder.AppendLine();
                messageBuilder.AppendFormat("Expected: {0}", args.ExpectedValue);
                messageBuilder.AppendLine();
                messageBuilder.AppendFormat("Actual: {0}", args.ActualValue);

                failingMessage = messageBuilder.ToString();
            }
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                return string.Format("\"{0}\"", value);
            }
        }

        private BinaryOperatorConstraint CreateBinaryOperatorConstraint(object expected, bool expectedValue, ParameterOrder order, BinaryOperator binaryOperator)
        {
            BinaryOperatorConstraint constraint = new BinaryOperatorConstraint(
                expected,
                expectedValue,
                order,
                binaryOperator);

            constraint.OnMatch += new BinaryOperatorConstraintMatchHandler(constraint_OnMatch);

            return constraint;
        }

        private bool IsNullable(object value)
        {
            return new NullableConstraint().Matches(value);
        }

        private bool IsNull(object value)
        {
            return new NullConstraint().Matches(value);
        }

        private Constraint InnerConstraint
        {
            get
            {
                if (innerConstraint == null)
                {
                    BinaryOperator operatorEqualEqual = new StaticEqualEqualOperator(equalityOperatorProvider);
                    BinaryOperator operatorNotEqual = new StaticNotEqualOperator(equalityOperatorProvider);

                    // actual == expected
                    innerConstraint = CreateBinaryOperatorConstraint(expected, expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsLeftSideOperator, operatorEqualEqual);
                    innerConstraint &= CreateBinaryOperatorConstraint(expected, expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsRightSideOperator, operatorEqualEqual);

                    // expected == actual
                    innerConstraint &= CreateBinaryOperatorConstraint(expected, !expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsLeftSideOperator, operatorNotEqual);
                    innerConstraint &= CreateBinaryOperatorConstraint(expected, !expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsRightSideOperator, operatorNotEqual);

                    if (!IsNull(expected))
                    {
                        // expected.Equals(actual)
                        innerConstraint &= CreateBinaryOperatorConstraint(expected, expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsRightSideOperator, new EqualOperator());

                        // actual.GetHashCode() == expected.GetHasCode()
                        innerConstraint &= new NullConstraint() |
                            CreateBinaryOperatorConstraint(expected, expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsLeftSideOperator, new GetHashCodeOperator());

                    }


                    // actual.Equals(expected)
                    innerConstraint &= new NullConstraint() |
                        CreateBinaryOperatorConstraint(expected, expectedValueIfActualIsEqualToExpected, ParameterOrder.ActualIsLeftSideOperator, new EqualOperator());

                    // actual.Equals(null)
                    innerConstraint &= new NullConstraint() |
                        CreateBinaryOperatorConstraint(null, false, ParameterOrder.ActualIsLeftSideOperator, new EqualOperator());
                   
                   
                    if (IsNullable(expected))
                    {
                        // null == null
                        innerConstraint &= new NotNullableConstraint() |
                            new ConstraintWhenActualIsAlwaysNull(
                            new BinaryOperatorConstraint(null, true, ParameterOrder.ActualIsLeftSideOperator, operatorEqualEqual));
                        innerConstraint &= new NotNullableConstraint() |
                            new ConstraintWhenActualIsAlwaysNull(
                            new BinaryOperatorConstraint(null, false, ParameterOrder.ActualIsLeftSideOperator, operatorNotEqual));

                        // null == expected
                        innerConstraint &= new NotNullableConstraint() |
                            new ConstraintWhenActualIsAlwaysNull(
                            new BinaryOperatorConstraint(expected, IsNull(expected), ParameterOrder.ActualIsLeftSideOperator, operatorEqualEqual));
                       
                        // expected == null
                        innerConstraint &= new NotNullableConstraint() |
                            new ConstraintWhenActualIsAlwaysNull(
                            new BinaryOperatorConstraint(expected, IsNull(expected), ParameterOrder.ActualIsRightSideOperator, operatorEqualEqual));
                      
                        // actual == null
                        innerConstraint &= new NullConstraint() |
                            CreateBinaryOperatorConstraint(null, false, ParameterOrder.ActualIsLeftSideOperator, operatorEqualEqual);
                        innerConstraint &= new NullConstraint() |
                            CreateBinaryOperatorConstraint(null, true, ParameterOrder.ActualIsLeftSideOperator, operatorNotEqual);

                        // null == actual
                        innerConstraint &= new NullConstraint() |
                            CreateBinaryOperatorConstraint(null, true, ParameterOrder.ActualIsLeftSideOperator, operatorNotEqual);
                        innerConstraint &= new NullConstraint() |
                            CreateBinaryOperatorConstraint(null, true, ParameterOrder.ActualIsRightSideOperator, operatorNotEqual);
                    }


                }
                return innerConstraint;
            }
        }

        public override bool Matches(object actual)
        {
            // Reset the message
            failingMessage = null;

            return InnerConstraint.Matches(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            InnerConstraint.WriteDescriptionTo(writer);

        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            InnerConstraint.WriteActualValueTo(writer);
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            if (failingMessage != null)
            {
                writer.Write(failingMessage);
            }
            else
            {
                base.WriteMessageTo(writer);
            }
        }
    }


    public class EqualityOperatorConstraint : BaseEqualityOperatorConstraint
    {
        public EqualityOperatorConstraint(object expected, IStaticEqualityOperatorProvider equalityOperatorProvider)
            : base(expected, true, equalityOperatorProvider)
        {

        }
    }

    public class InequalityOperatorConstraint : BaseEqualityOperatorConstraint
    {
        public InequalityOperatorConstraint(object expected, IStaticEqualityOperatorProvider equalityOperatorProvider)
            : base(expected, false, equalityOperatorProvider)
        {

        }
    }




}
