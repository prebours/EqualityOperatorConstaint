using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MethodOrdering
{
    public class OrderedMethodProvider : IMethodProvider
    {
        private readonly ILineNumberProvider m_symbolOrderProvider;
        private readonly IMethodProvider m_underlyingProvider;

        public OrderedMethodProvider(IMethodProvider underlyingProvider, ILineNumberProvider symbolOrderProvider)
        {
            m_symbolOrderProvider = symbolOrderProvider;
            m_underlyingProvider = underlyingProvider;
        }        

        class SortableMethodWrapper : IComparable<SortableMethodWrapper>
        {
            private readonly int m_lineNumber;
            private readonly MethodInfo m_method;

            public SortableMethodWrapper(int lineNumber, MethodInfo method)
            {
                m_lineNumber = lineNumber;
                m_method = method;
            }

            public MethodInfo MethodInfo
            {
                get
                {
                    return m_method;
                }
            }


            public int CompareTo(SortableMethodWrapper other)
            {
                return m_lineNumber.CompareTo(other.m_lineNumber);
            }
        }

        public MethodInfo[] GetMethods(Type type)
        {
            List<SortableMethodWrapper> wrappedMethods = new List<SortableMethodWrapper>();

            foreach (MethodInfo method in m_underlyingProvider.GetMethods(type))
            {
                int lineNumber = m_symbolOrderProvider.GetStartLineNumber(method);
                wrappedMethods.Add(new SortableMethodWrapper(lineNumber, method));
            }

            wrappedMethods.Sort();

            List<MethodInfo> extractedMethods = new List<MethodInfo>();
            foreach (SortableMethodWrapper wrapper in wrappedMethods)
            {
                extractedMethods.Add(wrapper.MethodInfo);

            }

            return extractedMethods.ToArray();

        }
    }


}
