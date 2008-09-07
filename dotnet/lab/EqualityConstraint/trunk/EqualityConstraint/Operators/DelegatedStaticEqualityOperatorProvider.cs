using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints.Operators
{
    public class DelegatedStaticEqualityOperatorProvider<T> : BaseStaticEqualityOperatorProvider<T>     
    {
        private readonly StaticEqualityOperatorHandler staticEqualEqualOperator;
        private readonly StaticEqualityOperatorHandler staticNotEqualOperator;

        public DelegatedStaticEqualityOperatorProvider(StaticEqualityOperatorHandler staticEqualEqualOperator,
            StaticEqualityOperatorHandler staticNotEqualOperator)
        {
            if (staticNotEqualOperator == null || staticEqualEqualOperator == null)
            {
                throw new ArgumentNullException();
            }
            this.staticEqualEqualOperator = staticEqualEqualOperator;
            this.staticNotEqualOperator = staticNotEqualOperator;
        }

        public override bool EvaluateStaticEqualEqualOperator(T lhs, T rhs)
        {
            return staticEqualEqualOperator(lhs, rhs);
        }

        public override bool EvaluateStaticNotEqualOperator(T lhs, T rhs)
        {
            return staticNotEqualOperator(lhs, rhs);
        }

         public delegate bool StaticEqualityOperatorHandler(T lhs, T rhs);
    }

   

}
