/**********************************************************************
* Copyright (C) 2010-2012 天津市天祥世联网络科技有限公司
* 文件名: Win32API.cs
* 创建者: 肖元
* 创建时间: 2011-08-30
* 描述: 系统API引出类
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
/*********************************************************************/
using System.Runtime.InteropServices;
using IcomClient.Common; // DllImport 函数在此命名空间下

namespace IcomClient.API
{
    /// <summary>
    /// 系统API引出类
    /// </summary>
    public class Win32API
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_strFilePath"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern Int32 LoadLibrary(string _strFilePath);//对非托管函数进行声明

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_hDllHandle"></param>
        /// <param name="_strFuncName"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern IntPtr GetProcAddress(Int32 _hDllHandle, string _strFuncName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iHandle"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern Int32 FreeLibrary(Int32 _iHandle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iHandle"></param>
        /// <returns></returns>
        [DllImport("Shlwapi.dll")]
        public static extern bool PathFileExists(Int32 _iHandle);

        /// <summary>
        /// 通过消息发送一个对象指针
        /// </summary>
        /// <param name="_hWnd">接收消息的窗体句柄</param>
        /// <param name="_Msg">消息号</param>
        /// <param name="_wParam">w参数</param>
        /// <param name="lParam">l参数 对象指针</param>
        /// <returns>是否成功</returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessageObject(IntPtr _hWnd, Int32 _Msg, Int32 _wParam, IntPtr _lParam);
        /// <summary>
        /// 通过消息发送两个对象指针
        /// </summary>
        /// <param name="_hWnd">接收消息的窗体句柄</param>
        /// <param name="_Msg">消息号</param>
        /// <param name="_wParam">w参数 对象指针1</param>
        /// <param name="lParam">l参数 对象指针2</param>
        /// <returns>是否成功</returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessageDblObject(IntPtr _hWnd, Int32 _Msg, IntPtr _wParam, IntPtr _lParam);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_hWnd"></param>
        /// <param name="_Msg"></param>
        /// <param name="_wParam"></param>
        /// <param name="_lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(IntPtr _hWnd, Int32 _Msg, Int32 _wParam, Int32 _lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_hWnd"></param>
        /// <param name="_Msg"></param>
        /// <param name="_wParam"></param>
        /// <param name="_lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessageEx(IntPtr _hWnd, Int32 _Msg, Int32 _wParam, ref sOBJECTSTRUCT _lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32")]
        public static extern Int32 WritePrivateProfileString(string _strSec, string _strKey, string _strValue, string _strPath);

        [DllImport("kernel32")]
        public static extern Int32 GetPrivateProfileString(string _strSec, string _strKey, string _strDef, StringBuilder _strValue, Int32 _iSize, string _strPath);

    }
}
