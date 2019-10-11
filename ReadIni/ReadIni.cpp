// ReadIni.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

///////////////////// 2019/08/12 16:41:14 boost库头文件 ==> BEGIN /////////////////////////////////////////////////////
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/ini_parser.hpp>
///////////////////// 2019/08/12 16:41:14 boost库头文件 ==> END /////////////////////////////////////////////////////


#define EXTERN extern "C" __declspec(dllexport)
typedef void(__stdcall *PCALLBACK)(const char*);

PCALLBACK                   gl_funcCallback;		/*\ 调用者传递进来的回调函数 \*/
boost::property_tree::ptree gl_oParentPtree;        /*\ 用来存储父节点 \*/
boost::property_tree::ptree gl_oChildPtree;	        /*\ 用来存储子节点 \*/
std::string					gl_sIniFilePath;		/*\ 用来存储ini文件路径 \*/

 /****************************************!
 *@brief  初始化dll信息
 *@author Jinzi
 *@date   2019/08/12 16:37:23
 *@param[in]
	_sIniFilePath : ini文件的路径
	_funcCallBack : 调用者注册的回调函数
 *@param[out]
 *@return
 ****************************************/
EXTERN void StartUp(const char* _sIniFilePath, PCALLBACK _funcCallBack)
{
	gl_funcCallback = _funcCallBack;
	gl_sIniFilePath = _sIniFilePath;
	read_ini(_sIniFilePath, gl_oParentPtree);
}
/****************************************!
*@brief  根据标题值和key值得到value值
*@author Jinzi
*@date   2019/08/12 16:48:30
*@param[in]
   _sTitleString : 标题字符串
   _sTitleInKey  : 标题下的key值
*@param[out]
*@return    成功返回true 失败返回false 通过回调函数将value值返回回去
****************************************/
EXTERN bool GetValueWithTitleAndKey(const char* _sTitleString, const char* _sTitleInKey)
{
	bool bIsSucc = false;
	gl_oChildPtree = gl_oParentPtree.get_child(_sTitleString);
	std::string sValue = gl_oChildPtree.get<std::string>(_sTitleInKey);
	if (gl_funcCallback)
	{
		std::string sRetJsonData = "{\"" + static_cast<std::string>(_sTitleInKey) + "\":\"" + sValue + "\"}";
		gl_funcCallback(sRetJsonData.c_str());
		bIsSucc = true;
	}
	return bIsSucc;
}

/****************************************!
*@brief  根据标题和key设置value的值
*@author Jinzi
*@date   2019/08/12 17:01:11
*@param[in]
   _sTitleString : 标题字符串
   _sTitleInKey  : 标题下的key值
   _sChangeValue : 要修改的value得值
*@param[out]
*@return    成功返回true 失败返回false
****************************************/
EXTERN bool SetValueWithTitleAndKey(const char* _sTitleString, const char* _sTitleInKey,
	const char* _sChangeValue)
{
	bool bIsSucc = false;
	//gl_oChildPtree = gl_oParentPtree.get_child(_sTitleString);
	gl_oParentPtree.put<std::string>(_sTitleInKey, _sChangeValue);
	write_ini(gl_sIniFilePath, gl_oParentPtree);
	bIsSucc = true;
	return bIsSucc;
}

