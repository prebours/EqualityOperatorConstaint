using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MethodOrdering
{
    public interface IMethodFilter
    {
        bool IsMatch(MethodInfo methodInfo);
        void Initialize();
    }
}
