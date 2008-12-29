using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using MethodOrdering;

namespace MethodOrdering.Test
{
    [TestFixture]
    public class PdbSymbolOrderProviderTest
    {
        FunctionGenerationCompiler compiler;
        ILineNumberProvider provider;

        [SetUp]
        public void SetUp()
        {
            compiler = new FunctionGenerationCompiler();
            provider =  new MDbgLineNumberProvider();
        }

        [TearDown]
        public void TearDown()
        {
            compiler.Dispose();
        }

        private void Test(FunctionGenerationResult result, ILineNumberProvider provider)
        {
            int maxLineNumber = 0;
            for (int index = 0; index < result.FunctionCount; index++)
            {
                MethodInfo method = result.CompiledType.GetMethod(result.GetFunctionName(index));
                Assert.IsNotNull(method);

                int lineNumber = provider.GetStartLineNumber(method);
                Assert.Greater(lineNumber, 0);

                Assert.Greater(lineNumber, maxLineNumber);

                maxLineNumber = lineNumber;
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestWithNullMethod()
        {
            provider.GetStartLineNumber(null);
        }

        [Test]        
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWithoutPdb()
        {
            FunctionGenerationResult result = compiler.CompileWithoutDebugInformation(10);

            Test(result, provider);         
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestWithGacAssembly()
        {
            provider.GetStartLineNumber(typeof(string).GetMethod("Clone"));
        }
        

        [Test]
        public void TestWith10Functions()
        {
            FunctionGenerationResult result = compiler.CompileWithDebugInformation(10);
           
            Test(result, provider);
        }

        [Test]
        public void TestWith200Functions()
        {
            FunctionGenerationResult result = compiler.CompileWithDebugInformation(200);
            
            Test(result, provider);
        }

        [Test]
        public void TestWithBasicType()
        {
            Assert.Greater(provider.GetStartLineNumber(typeof(BasicType).GetMethod("Value2")),
                           provider.GetStartLineNumber(typeof(BasicType).GetMethod("Value1")));

            Assert.Greater(provider.GetStartLineNumber(typeof(BasicType).GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance)),
                          provider.GetStartLineNumber(typeof(BasicType).GetMethod("_Test")));
        }
    }
}
