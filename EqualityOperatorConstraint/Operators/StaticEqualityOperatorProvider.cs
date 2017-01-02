using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NUnit.Framework.Constraints.Operators
{
    public interface IStaticEqualityOperatorProvider
    {
        bool EvaluateStaticEqualEqualOperator(object lhs, object rhs);
        bool EvaluateStaticNotEqualOperator(object lhs, object rhs);        
    }   
}
