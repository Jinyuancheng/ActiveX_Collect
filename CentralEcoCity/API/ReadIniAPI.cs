using BethVideo;
using IcomClient.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CentralEcoCity.API
{
    #region [定义ReadIni委托类型]
    /// <summary>
    /// 初始化的回调函数，所有读取的数据都从该函数回调回来
    /// </summary>
    /// <param name="_sCallBackData">回调回来的JSON数据</param>
    public delegate void pReadIniCallBack(string _sCallBackData);
    /// <summary>
    /// 初始化ReadIni
    /// </summary>
    /// <param name="_sIniFilePath">配置文件的路径</param>
    /// <param name="_funcCallBack">回调函数</param>
    public delegate void pStartUp(string _sIniFilePath, pReadIniCallBack _funcCallBack);
    /// <summary>
    /// 根据标题和key值寻找配置文件中对应的value
    /// </summary>
    /// <param name="_sTitleString">标题名称</param>
    /// <param name="_sTitleInKey">该标题下的key名称</param>
    /// <param name="_sSaveValue">key值对应的value</param>
    /// <returns></returns>
    public delegate bool pGetValueWithTitleAndKey(string _sTitleString, string _sTitleInKey);
    /// <summary>
    /// 根据标题和key值 进行ini文件的写入
    /// </summary>
    /// <param name="_sTitleString">标题名称</param>
    /// <param name="_sTitleInKey">该标题下的key值</param>
    /// <param name="_sChangeValue">要设置该key值的value</param>
    /// <returns></returns>
    public delegate bool pSetValueWithTitleAndKey(string _sTitleString, string _sTitleInKey, string _sChangeValue);
    #endregion

    class ReadIniAPI
    {
        #region [定义一个静态的成员变量用来存储加载dll文件的句柄]
        public static int m_ReadIniHandle = 0;
        #endregion

        #region [声明委托并初始化为null]
        public static pStartUp StartUp = null;
        public static pGetValueWithTitleAndKey GetValueWithTitleAndKey = null;
        public static pSetValueWithTitleAndKey SetValueWithTitleAndKey = null;
        #endregion

        #region [加载动态库]
        /// <summary>
        /// 初始化ReadIni.dll文件
        /// </summary>
        /// <param name="_sFilePath">ReadIni.dll的路径信息</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool LoadReadIniAPI(string _sFilePath)
        {
            if (m_ReadIniHandle != 0)
            {
                return true;
            }
            if (File.Exists(_sFilePath) != true)
            {
                return false;
            }
            m_ReadIniHandle = Win32API.LoadLibrary(_sFilePath);
            if (m_ReadIniHandle <= 0)
            {
                return false;
            }
            #region [加载函数api]
            StartUp = (pStartUp)FuncCommon.GetFunctionAddress(m_ReadIniHandle, "StartUp",
                typeof(pStartUp));
            if (StartUp == null) { return false; }
            GetValueWithTitleAndKey = (pGetValueWithTitleAndKey)FuncCommon.GetFunctionAddress(m_ReadIniHandle,
                "GetValueWithTitleAndKey", typeof(pGetValueWithTitleAndKey));
            if (GetValueWithTitleAndKey == null) { return false; }
            SetValueWithTitleAndKey = (pSetValueWithTitleAndKey)FuncCommon.GetFunctionAddress(m_ReadIniHandle,
                "SetValueWithTitleAndKey", typeof(pSetValueWithTitleAndKey));
            if (GetValueWithTitleAndKey == null) { return false; }
            #endregion

            return true;
        }
        #endregion

        #region [释放动态库]
        /// <summary>
        /// 释放动态库
        /// </summary>
        /// <returns>成功返回 true 失败返回 false</returns>
        public static bool FreeVsClientAPI()
        {
            if (m_ReadIniHandle != 0)
            {
                Win32API.FreeLibrary(m_ReadIniHandle);
                m_ReadIniHandle = 0;
            }
            return true;
        }
        #endregion
    }
}
