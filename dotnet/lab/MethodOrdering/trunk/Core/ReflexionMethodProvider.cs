using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MethodOrdering
{
    public class PublicReflectionMethodProvider : IMethodProvider
    {
        private readonly IMethodFilter m_filter;

        public PublicReflectionMethodProvider(IMethodFilter filter)
        {
            m_filter = filter;
        }

        public MethodInfo[] GetMethods(Type type)
        {
            m_filter.Initialize();

            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | 
                                                            BindingFlags.Static | 
                                                            BindingFlags.Public | 
                                                            BindingFlags.DeclaredOnly))
            {
                if (m_filter.IsMatch(method))
                {
                    methods.Add(method);
                }
            }

            return methods.ToArray();
        }
    }
}
