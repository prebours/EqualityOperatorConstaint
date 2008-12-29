using System;
using System.Collections.Generic;
using System.Text;

namespace MethodOrdering.Test
{
    class BasicType
    {
        public decimal Value1()
        {
            throw new NotImplementedException();
        }

        public void Value2()
        {
            throw new NotImplementedException();
        }

        public string Empty(int param)
        {
            throw new NotImplementedException();
        }

        public static decimal _Test(float param)
        {
            throw new NotImplementedException();
        }

        private static int Value = 1;

        private decimal PrivateMethod(string param)
        {
            throw new NotImplementedException();
        }
    }
}
