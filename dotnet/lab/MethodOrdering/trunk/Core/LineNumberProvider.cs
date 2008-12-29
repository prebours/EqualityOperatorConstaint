using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Diagnostics.SymbolStore;
using Microsoft.Samples.Debugging.CorSymbolStore;


namespace MethodOrdering
{
    public interface ILineNumberProvider
    {
        int GetStartLineNumber(MethodInfo method);
    }


    public class MDbgLineNumberProvider : ILineNumberProvider
    {
        private static FileInfo GetAssemblyFile(MethodInfo method)
        {
            string assemblyLocation = method.DeclaringType.Assembly.Location;
            if (!File.Exists(assemblyLocation))
            {
                throw new FileNotFoundException("The assembly file is not found");
            }
            FileInfo assemblyFile = new FileInfo(assemblyLocation);
            if (assemblyFile.Extension != ".dll")
            {
                throw new ArgumentException("Only dll extension are accepted");
            }

            string rootName = assemblyFile.Name.Substring(0, assemblyFile.Name.LastIndexOf(assemblyFile.Extension, StringComparison.OrdinalIgnoreCase));
            string pdbLocation = Path.Combine(assemblyFile.Directory.FullName, string.Concat(rootName, ".pdb"));

            if (!File.Exists(pdbLocation))
            {
                throw new FileNotFoundException("The related pdb file is not found");
            }

            return assemblyFile;
        }

        private static ISymbolReader CreateReader(MethodInfo method)
        {
            FileInfo assemblyFile = GetAssemblyFile(method);

            ISymbolReader reader = SymUtil.GetSymbolReaderForFile(assemblyFile.FullName, assemblyFile.Directory.FullName);
            return reader;

        }

        private static int GetStartLineNumber(ISymbolReader symbolReader, MethodInfo method)
        {
            ISymbolMethod symbolMethod = symbolReader.GetMethod(new SymbolToken(method.MetadataToken));
            if (symbolMethod == null)
            {
                throw new ArgumentException("The method was not found");
            }

            // Get the sequence points from the symbol store. 
            // We could cache these arrays and reuse them.
            int count = symbolMethod.SequencePointCount;
            int[] offsets = new int[count];
            ISymbolDocument[] docs = new ISymbolDocument[count];
            int[] startColumn = new int[count];
            int[] endColumn = new int[count];
            int[] startRow = new int[count];
            int[] endRow = new int[count];
            symbolMethod.GetSequencePoints(offsets, docs, startRow, startColumn, endRow, endColumn);

            return startRow[0];
        }



        public int GetStartLineNumber(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            ISymbolReader symbolReader = CreateReader(method);

            return GetStartLineNumber(symbolReader, method);

        }
    }
}
