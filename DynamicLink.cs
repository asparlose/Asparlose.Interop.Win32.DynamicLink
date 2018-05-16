using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Asparlose.Interop.Win32
{
    public abstract class DynamicLink : DisposableObject
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        readonly IntPtr lib;
        protected string FilePath { get; }

        protected DynamicLink(string dll)
        {
            FilePath = dll;
            lib = LoadLibrary(dll);

            if (lib == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        protected T GetProc<T>(string name)
        {
            var addr = GetProcAddress(lib, name);
            if (addr == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return Marshal.GetDelegateForFunctionPointer<T>(addr);
        }

        protected override void ReleaseUnmanagedResources()
        {
            base.ReleaseUnmanagedResources();

            FreeLibrary(lib);
        }

    }
}
