using System;
using NUnit.Framework;
using NUnit.Mocks;
using MethodOrdering;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace MethodOrdering.Test
{
    [TestFixture]
    [Description("Test to show that we cannot rely on reflexion to order the function names")]
    public class PublicReflectionMethodProviderWithALotOfMethodsTest
    {
        FunctionGenerationCompiler compiler;
        IMethodProvider provider;
        DynamicMock filterMock;

        [SetUp]
        public void SetUp()
        {
            compiler = new FunctionGenerationCompiler();            
           
            filterMock = new DynamicMock(typeof(IMethodFilter));
            filterMock.Strict = false;
            filterMock.SetReturnValue("IsMatch", true);

            provider = new PublicReflectionMethodProvider((IMethodFilter)filterMock.MockInstance);
        }

        [TearDown]
        public void TearDown()
        {
            compiler.Dispose();
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        [Description("The order is messed up when there are a lot of functions")]
        public void TestWithLargeNumber()
        {
            try
            {
                FunctionGenerationResult result = compiler.CompileWithoutDebugInformation(200);

                MethodInfo[] methods = provider.GetMethods(result.CompiledType);

                Assert.AreEqual(result.FunctionCount, methods.Length);

                GenerateLog(result, methods, new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "index.csv")));

                int index = 0;
                foreach (MethodInfo method in methods)
                {
                    Trace.TraceInformation("Index: {0} - Index2: {1}", index, result.GetDeclaredIndex(method.Name));
                    Trace.WriteLine("");

                    //Assert.AreEqual(result.GetFunctionName(index), method.Name);
                    index++;
                }
            }
            catch (AssertionException ex)
            {
                throw new ApplicationException(ex.Message);
            }


        }

        private void GenerateLog(FunctionGenerationResult result, MethodInfo[] methods, FileInfo file)
        {            
            using (FileStream stream = file.OpenWrite())
            using(StreamWriter writer = new StreamWriter(stream))
            {
                int index = 0;
                writer.WriteLine("Returned Index,Declared Index,MetaData Token Offset");

                string firstDeclaredMethodName = result.GetFunctionName(0);
                int firstDeclaredMetaDataToken = -1;
                foreach (MethodInfo method in methods)
                {
                    if (string.Compare(method.Name, firstDeclaredMethodName, StringComparison.Ordinal) == 0)
                    {
                        firstDeclaredMetaDataToken = method.MetadataToken;
                        break;
                    }
                }

                Debug.Assert(firstDeclaredMetaDataToken > 0);

                foreach(MethodInfo method in methods) {
                    writer.WriteLine("{0},{1},{2}", index, result.GetDeclaredIndex(method.Name), method.MetadataToken - firstDeclaredMetaDataToken);
                    index++;
                }
            }
        }

        [Test]
        public void TestWithLowNumber()
        {
            FunctionGenerationResult result = compiler.CompileWithoutDebugInformation(10);

            MethodInfo[] methods = provider.GetMethods(result.CompiledType);

            Assert.AreEqual(result.FunctionCount, methods.Length);
         
            int index = 0;
            foreach (MethodInfo method in methods)
            {
                Assert.AreEqual(result.GetFunctionName(index), method.Name);
                index++;
            }
        }

    }
}
