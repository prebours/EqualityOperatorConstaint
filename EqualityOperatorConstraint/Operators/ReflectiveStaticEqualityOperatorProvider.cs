using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NUnit.Framework.Constraints.Operators
{
  
    public class ReflectiveStaticEqualityOperatorProvider<T> : BaseStaticEqualityOperatorProvider<T> where T : class
    {

        private bool? InvokeStaticOperator(T lhs, T rhs, string methodName)
        {
            MethodInfo mi = typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] {
                typeof(T),
                typeof(T)},
               null);

            if (mi != null)
            {
                object result = mi.Invoke(null, new object[] { lhs, rhs });
                if (!(result is bool))
                {
                    throw new InvalidCastException();
                }
                return (bool)result;
            }
            else
            {
                return null;
            }
        }
        
        public override bool EvaluateStaticEqualEqualOperator(T lhs, T rhs)
        {
            bool? result = InvokeStaticOperator(lhs, rhs, "op_Equality");
            if (result == null)
            {
                throw new ReflectiveStaticEqualEqualOperatorNotFound(string.Format("operator== is not defined in \"{0}\"", typeof(T).Name));
            }
            else
            {
                return result.Value;
            }
        }

        public override bool EvaluateStaticNotEqualOperator(T lhs, T rhs)
        {
            bool? result = InvokeStaticOperator(lhs, rhs, "op_Inequality");
            if (result == null)
            {
                throw new ReflectiveStaticNotEqualOperatorNotFound(string.Format("operator!= is not defined in \"{0}\"", typeof(T).Name));
            }
            else
            {
                return result.Value;
            }
        }

       
    }

    public abstract class ReflectiveStaticOperatorNotFound : Exception
    {
        
        protected ReflectiveStaticOperatorNotFound(string description) : base(description)
        {
            
        }
    }

    public class ReflectiveStaticEqualEqualOperatorNotFound : ReflectiveStaticOperatorNotFound
    {

        public ReflectiveStaticEqualEqualOperatorNotFound(string description) : base(description)
        {

        }
    }

    public class ReflectiveStaticNotEqualOperatorNotFound : ReflectiveStaticOperatorNotFound
    {

        public ReflectiveStaticNotEqualOperatorNotFound(string description)
            : base(description)
        {

        }
    }
}
