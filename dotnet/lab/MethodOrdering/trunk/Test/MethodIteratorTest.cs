using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Mocks;
using MethodParsing;
using System.Reflection;

namespace MethodIteration.Test
{

    [TestFixture]
    public class OrderedMethodIteratorTest
    {
        IMethodProvider provider;
        DynamicMock underlyingProvider;
        DynamicMock symbolMock;

        class Name1Attribute : Attribute
        {
            private readonly string m_name;

            public Name1Attribute(string name)
            {
                m_name = name;
            }

            public string Name1
            {
                get
                {
                    return m_name;
                }
            }
        }

        class Name2Attribute : Attribute
        {
            private readonly string m_name;

            public Name2Attribute(string name)
            {
                m_name = name;
            }

            public string Name2
            {
                get
                {
                    return m_name;
                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            underlyingProvider = new DynamicMock(typeof(IMethodProvider));
            underlyingProvider.Strict = true;

            symbolMock = new DynamicMock(typeof(ISymbolOrderProvider));
            symbolMock.Strict = true;

            provider = new OrderedMethodProvider(
                (IMethodProvider)underlyingProvider.MockInstance,
                (ISymbolOrderProvider)symbolMock.MockInstance);
        }

        


        [Test]
        [Description("The mock inverts the order")]
        public void TestBasic()
        {
            symbolMock.ExpectAndReturn("GetStartLineNumber", 20, typeof(BasicType).GetMethod("Value1"));
            symbolMock.ExpectAndReturn("GetStartLineNumber", 3, typeof(BasicType).GetMethod("Value2"));
            symbolMock.ExpectAndReturn("GetStartLineNumber", 41, typeof(BasicType).GetMethod("_Test"));
            symbolMock.ExpectAndReturn("GetStartLineNumber", 40, typeof(BasicType).GetMethod("Empty"));

            underlyingProvider.Expect("Initialize");
            underlyingProvider.SetReturnValue("IsMatch", true);

            MethodInfo[] methods = provider.GetMethods(typeof(BasicType));
            Assert.AreEqual(3, methods.Length);

            int index = 0;
            foreach (MethodInfo method in methods)
            {
                if (index == 0)
                {
                    Assert.AreEqual("Value2", method.Name);
                }
                else if (index == 1)
                {
                    Assert.AreEqual("Value1", method.Name);
                }
                else if (index == 2)
                {
                    Assert.AreEqual("Empty", method.Name);
                }
                else if (index == 3)
                {
                    Assert.AreEqual("_Test", method.Name);
                }

                index++;
            }
        }


        class NullVariableNameProvider
        {
            [Name1(" ")]
            public decimal Value()
            {
                throw new NotImplementedException();
            }

            [Name1(null)]
            public decimal Value2(string param, string param2)
            {
                throw new NotImplementedException();
            }

            [Name1("")]
            public decimal Value3(int param)
            {
                throw new NotImplementedException();
            }

            public decimal Value4(decimal param)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestNullVariable()
        {
            Assert.AreEqual(0, iterator.GetCount(typeof(NullVariableNameProvider)));
        }

        class NonPublicProvider
        {
            [Name1("Test")]
            private decimal Value3(int param)
            {
                throw new NotImplementedException();
            }

            [Name1("Test")]
            protected decimal Value4(decimal param)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestNonPublicVariable()
        {
            Assert.AreEqual(0, iterator.GetCount(typeof(NonPublicProvider)));
        }

        class ProviderWithCategoryAttribute
        {
            [Name2("Category1")]
            [Name1("Hello1")]
            public decimal Value1()
            {
                throw new NotImplementedException();
            }

            [Name1("Hello2")]
            public decimal Value2()
            {
                throw new NotImplementedException();
            }

            [Name1("Hello4")]
            public decimal Value4()
            {
                throw new NotImplementedException();
            }

            [Name1("Hello3")]
            [Name2("Category2")]
            public decimal Value3(decimal param)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        [Description("Notice that Value4 will have the same category as Value3")]
        public void TestCategoryAttribute()
        {
            Assert.AreEqual(4, iterator.GetCount(typeof(ProviderWithCategoryAttribute)));

            symbolOrderMock.ExpectAndReturn("GetStartLineNumber", 0, null, typeof(ProviderWithCategoryAttribute).GetMethod("Value1"));
            symbolOrderMock.ExpectAndReturn("GetStartLineNumber", 1, null, typeof(ProviderWithCategoryAttribute).GetMethod("Value2"));
            symbolOrderMock.ExpectAndReturn("GetStartLineNumber", 4, null, typeof(ProviderWithCategoryAttribute).GetMethod("Value4"));
            symbolOrderMock.ExpectAndReturn("GetStartLineNumber", 3, null, typeof(ProviderWithCategoryAttribute).GetMethod("Value3"));

            int index = 0;
            foreach (MemberIterationItem method in iterator.GetMethods(null, typeof(ProviderWithCategoryAttribute)))
            {
                if (index <= 1)
                {
                    Assert.AreEqual("Category1", ((Name2Attribute)method.GetAttribute(typeof(Name2Attribute))).Name2);
                }
                else
                {
                    Assert.AreEqual("Category2", ((Name2Attribute)method.GetAttribute(typeof(Name2Attribute))).Name2);
                }
                index++;
            }
        }

        class ProviderWithSameVariableName
        {
            [Name1("Hello1")]
            public decimal Value1()
            {
                throw new NotImplementedException();
            }

            [Name1("Hello1")]
            public decimal Value2()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void TestSameVariableName()
        {
            Assert.AreEqual(1, iterator.GetCount(typeof(ProviderWithSameVariableName)));

            symbolOrderMock.ExpectAndReturn("GetStartLineNumber", 0, null, typeof(ProviderWithSameVariableName).GetMethod("Value1"));


            int index = 0;
            foreach (MemberIterationItem method in iterator.GetMethods(null, typeof(ProviderWithSameVariableName)))
            {
                if (index == 0)
                {
                    Assert.AreEqual("Hello1", ((Name1Attribute)method.GetAttribute(typeof(Name1Attribute))).Name1);
                    Assert.AreEqual("Value1", method.MethodInfo.Name);
                }
                index++;
            }
        }


    }




}
