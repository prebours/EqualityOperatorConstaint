using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints.Operators
{

    public abstract class BinaryOperator
    {
        public abstract string Description(string lhsName, string rhsName);
        public abstract bool Evaluate(object lhs, object rhs);
    }

    public class StaticEqualEqualOperator : BinaryOperator
    {
        private readonly IStaticEqualityOperatorProvider provider;

        public StaticEqualEqualOperator(IStaticEqualityOperatorProvider provider)
        {
            this.provider = provider;
        }

        public override string Description(string lhsName, string rhsName)
        {
            return string.Format("operator ==({0}, {1})", lhsName, rhsName);
        }

        public override bool Evaluate(object lhs, object rhs)
        {          
            return provider.EvaluateStaticEqualEqualOperator(lhs, rhs);
        }
    }

    public class StaticNotEqualOperator : BinaryOperator
    {
        private readonly IStaticEqualityOperatorProvider provider;

        public StaticNotEqualOperator(IStaticEqualityOperatorProvider provider)
        {
            this.provider = provider;
        }
        public override string Description(string lhsName, string rhsName)
        {
            return string.Format("operator !=({0}, {1})", lhsName, rhsName);
        }

        public override bool Evaluate(object lhs, object rhs)
        {
            return provider.EvaluateStaticNotEqualOperator(lhs, rhs);
        }
    }

    public class EqualOperator : BinaryOperator
    {
        public override string Description(string lhsName, string rhsName)
        {
            return string.Format("{0}.Equals({1})", lhsName, rhsName);
        }

        public override bool Evaluate(object lhs, object rhs)
        {
            return lhs.Equals(rhs);
        }
    }

    public class GetHashCodeOperator : BinaryOperator
    {

        public override string Description(string lhsName, string rhsName)
        {
            return string.Format("{0}.GetHashCode() == {1}.GetHashCode()", lhsName, rhsName);
        }

        public override bool Evaluate(object lhs, object rhs)
        {
            if (lhs == null || rhs == null)
            {
                throw new ArgumentNullException();
            }
            return lhs.GetHashCode() == rhs.GetHashCode();
        }
    }
}
