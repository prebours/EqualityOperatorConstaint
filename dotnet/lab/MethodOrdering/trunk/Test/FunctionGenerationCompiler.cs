using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;

namespace MethodOrdering.Test
{
    class FunctionGenerationResult
    {
        private readonly List<string> m_functionNames = new List<string>();
        private readonly Type m_testType;
        private readonly FileInfo m_assemblyFile;

        public FunctionGenerationResult(List<string> functionNames, Type testType, FileInfo assemblyFile)
        {
            m_functionNames = functionNames;
            m_testType = testType;
            m_assemblyFile = assemblyFile;
        }

        public int FunctionCount
        {
            get
            {
                return m_functionNames.Count;
            }
        }
      

        public string GetFunctionName(int index)
        {
            return m_functionNames[index];
        }

        public int GetDeclaredIndex(string name)
        {
            return m_functionNames.FindIndex(delegate(string value)
            {
                return string.Compare(value, name, StringComparison.Ordinal) == 0;
            });
        }

        public Type CompiledType
        {
            get
            {
                return m_testType;
            }
        }

        public FileInfo AssemblyFile
        {
            get
            {
                return m_assemblyFile;
            }
        }


    }

    class FunctionGenerationCompiler : IDisposable
    {
        private const int FUNCTION_SIZE_NAME = 8;

        private FunctionGenerationResult m_result;
        private readonly Random m_random;

        public FunctionGenerationCompiler()
        {
            m_random = new Random(DateTime.Now.Second);
        }

        public FunctionGenerationResult CompileWithDebugInformation(int functionCount)
        {
            return Compile(functionCount, true);
        }

        public FunctionGenerationResult CompileWithoutDebugInformation(int functionCount)
        {
            return Compile(functionCount, false);
        }

        private FunctionGenerationResult Compile(int functionCount, bool includeDebugInformation)
        {
            if (m_result != null)
            {
                throw new ApplicationException("Can only compile once");
            }

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.IncludeDebugInformation = includeDebugInformation;
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = string.Format("{0}-{1}.dll", GetType().Name, Guid.NewGuid());
            parameters.ReferencedAssemblies.Add("System.dll");

            List<string> functionNames = new List<string>();
            string code = GenerateSourceCode(functionCount, functionNames);

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            Assert.AreEqual(0, results.Errors.Count, "Compilation Errors. First Error: " + GetFirstErrorMessage(results));


            m_result = new FunctionGenerationResult(
                functionNames,
                results.CompiledAssembly.GetType("Test"),
                 new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), results.PathToAssembly)));

            return m_result;
        }

        private static string GetFirstErrorMessage(CompilerResults results)
        {
            if (results.Errors.Count != 0)
            {
                return results.Errors[0].ErrorText;
            }
            else
            {
                return null;
            }

        }

        public void Dispose()
        {
            if (m_result != null)
            {
                try
                {
                    File.Delete(m_result.AssemblyFile.FullName);
                }
                catch (UnauthorizedAccessException)
                {

                }
            }
        }

        // http://www.c-sharpcorner.com/UploadFile/mahesh/RandomNumber11232005010428AM/RandomNumber.aspx
        private string GetFunctionName(int index)
        {
            StringBuilder builder = new StringBuilder();

            char ch;
            for (int i = 0; i < FUNCTION_SIZE_NAME; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * m_random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        private string GenerateSourceCode(int functionCount, List<string> functionNames)
        {
            Debug.Assert(functionNames.Count == 0);

            StringBuilder code = new StringBuilder();
            code.Append(@"
using System;

class Test {");

            for (int index = 0; index < functionCount; index++)
            {
                string functionName = GetFunctionName(index);

                functionNames.Add(functionName);
                code.AppendFormat(@"
public decimal {0}(int parameter) {{
    throw new NotImplementedException();
}}", functionName);
                code.AppendLine();
            }

            code.AppendLine(@"}");

            return code.ToString();
        }


    }

}
