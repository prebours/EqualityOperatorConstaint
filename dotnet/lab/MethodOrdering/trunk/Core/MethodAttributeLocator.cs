using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MethodOrdering
{
    public interface IMethodAttributeLocator<T> where T : Attribute
    {
        T Locate(MethodInfo method);
    }

    public class DirectMethodAttributeLocator<T> : IMethodAttributeLocator<T> where T : Attribute
    {
        public T Locate(MethodInfo method)
        {
            T foundAttribute = null;
            foreach (Attribute attribute in method.GetCustomAttributes(false))
            {
                if (attribute.GetType() == typeof(T))
                {
                    foundAttribute = (T)attribute;
                    break;
                }
            }

            return foundAttribute;
        }
    }

    public class MethodAttributeDirectAndAboveLocator<T> : IMethodAttributeLocator<T> where T : Attribute
    {
        private readonly MethodInfo[] m_methods;
        private readonly IMethodAttributeLocator<T> m_directLocator;

        public MethodAttributeDirectAndAboveLocator(MethodInfo[] methods, IMethodAttributeLocator<T> directLocator)
        {
            m_methods = methods;
            m_directLocator = directLocator;
        }

        private int GetIndex(MethodInfo method)
        {
            int foundIndex = -1;
            for (int index = 0; index < m_methods.Length; index++)
            {
                if (method == m_methods[index])
                {
                    foundIndex = index;
                    break;
                }
            }
            return foundIndex;
        }

        public T Locate(MethodInfo method)
        {
            int methodIndex = GetIndex(method);
            if (methodIndex < 0)
            {
                throw new ArgumentOutOfRangeException("method");
            }

            T foundAttribute = null;
            for (int index = methodIndex; index >= 0; index--)
            {
                foundAttribute = m_directLocator.Locate(m_methods[index]);
                if (foundAttribute != null)
                {
                    break;
                }
            }

            return foundAttribute;
        }
    }
}
