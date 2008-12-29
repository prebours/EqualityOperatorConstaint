﻿using System;
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
    // http://blogs.msdn.com/jmstall/articles/sample_pdb2xml.aspx
    // Encapsulate a set of helper classes to get a symbol reader from a file.
    // The symbol interfaces require an unmanaged metadata interface.
    static class SymUtil
    {
        static class NativeMethods
        {
            [DllImport("ole32.dll")]
            public static extern int CoCreateInstance([In] ref Guid rclsid,
                                                       [In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkOuter,
                                                       [In] uint dwClsContext,
                                                       [In] ref Guid riid,
                                                       [Out, MarshalAs(UnmanagedType.Interface)] out Object ppv);
        }

        // Wrapper.
        public static ISymbolReader GetSymbolReaderForFile(string pathModule, string searchPath)
        {
            return SymUtil.GetSymbolReaderForFile(new SymbolBinder(), pathModule, searchPath);
        }

        // We demand Unmanaged code permissions because we're reading from the file system and calling out to the Symbol Reader
        // @TODO - make this more specific.
        [System.Security.Permissions.SecurityPermission(
            System.Security.Permissions.SecurityAction.Demand,
            Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        public static ISymbolReader GetSymbolReaderForFile(SymbolBinder binder, string pathModule, string searchPath)
        {
              ISymbolReader reader= null;

            // Guids for imported metadata interfaces.
            Guid dispenserClassID = new Guid(0xe5cb7a31, 0x7512, 0x11d2, 0x89, 0xce, 0x00, 0x80, 0xc7, 0x92, 0xe5, 0xd8); // CLSID_CorMetaDataDispenser
            Guid dispenserIID = new Guid(0x809c652e, 0x7396, 0x11d2, 0x97, 0x71, 0x00, 0xa0, 0xc9, 0xb4, 0xd5, 0x0c); // IID_IMetaDataDispenser
            Guid importerIID = new Guid(0x7dac8207, 0xd3ae, 0x4c75, 0x9b, 0x67, 0x92, 0x80, 0x1a, 0x49, 0x7d, 0x44); // IID_IMetaDataImport

            // First create the Metadata dispenser.
            object objDispenser;
            int hResult = NativeMethods.CoCreateInstance(ref dispenserClassID, null, 1, ref dispenserIID, out objDispenser);
            if (hResult == 0)
            {
                // Now open an Importer on the given filename. We'll end up passing this importer straight
                // through to the Binder.
                object objImporter;
                IMetaDataDispenser dispenser = (IMetaDataDispenser)objDispenser;
                dispenser.OpenScope(pathModule, 0, ref importerIID, out objImporter);

                IntPtr importerPtr = IntPtr.Zero;

                try
                {
                    // This will manually AddRef the underlying object, so we need to be very careful to Release it.
                    importerPtr = Marshal.GetComInterfaceForObject(objImporter, typeof(IMetadataImport));

                    reader = binder.GetReader(importerPtr, pathModule, searchPath);
                }
                finally
                {
                    if (importerPtr != IntPtr.Zero)
                    {
                        Marshal.Release(importerPtr);
                    }
                }
            } else {
                throw new InvalidOperationException("Error while trying to create the symbol reader.");
            }
            return reader;
        }
    }

    // We can use reflection-only load context to use reflection to query for metadata information rather
    // than painfully import the com-classic metadata interfaces.
    [Guid("809c652e-7396-11d2-9771-00a0c9b4d50c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IMetaDataDispenser
    {
        // We need to be able to call OpenScope, which is the 2nd vtable slot.
        // Thus we need this one placeholder here to occupy the first slot..
        void DefineScope_Placeholder();

        //STDMETHOD(OpenScope)(                   // Return code.
        //LPCWSTR     szScope,                // [in] The scope to open.
        //  DWORD       dwOpenFlags,            // [in] Open mode flags.
        //  REFIID      riid,                   // [in] The interface desired.
        //  IUnknown    **ppIUnk) PURE;         // [out] Return interface on success.
        void OpenScope([In, MarshalAs(UnmanagedType.LPWStr)] String szScope, [In] Int32 dwOpenFlags, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.IUnknown)] out Object punk);

        // Don't need any other methods.
    }

    // Since we're just blindly passing this interface through managed code to the Symbinder, we don't care about actually
    // importing the specific methods.
    // This needs to be public so that we can call Marshal.GetComInterfaceForObject() on it to get the
    // underlying metadata pointer.
    [Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IMetadataImport
    {
        // Just need a single placeholder method so that it doesn't complain about an empty interface.
        void Placeholder();
    }
}
