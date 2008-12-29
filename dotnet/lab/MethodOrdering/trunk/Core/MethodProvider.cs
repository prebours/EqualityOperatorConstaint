using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MethodOrdering
{
    public interface IMethodProvider
    {
        MethodInfo[] GetMethods(Type type);
    }

}
