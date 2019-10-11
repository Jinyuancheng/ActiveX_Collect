using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BethVideo
{
    /// <summary>
    /// 系统API引出类
    /// </summary>
    public class Win32API
    {
        [DllImport("Kernel32")]
        public static extern void OutputDebugString(string message);

        [DllImport("Kernel32")]
        public static extern Int32 LoadLibrary(string _strFilePath);//对非托管函数进行声明

        [DllImport("Kernel32")]
        public static extern IntPtr GetProcAddress(Int32 _hDllHandle, string _strFuncName);

        [DllImport("Kernel32")]
        public static extern Int32 FreeLibrary(Int32 _iHandle);

        [DllImport("Shlwapi.dll")]
        public static extern bool PathFileExists(Int32 _iHandle);

    }
}
