using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace HuyaWASM
{
    public class HYBaseResponse
    {
        public string callbackId; //回调id,调用者不需要关注
        public string errMsg;   //失败示例 getUserInfo:fail auth deny; 成功示例 getUserInfo:ok
    }

    public class HYBaseActionParam<T>
    {
        public Action<T> success;  //接口调用成功的回调函数
        public Action<T> fail;   //接口调用失败的回调函数	
        public Action<T> complete;  //接口调用结束的回调函数（调用成功、失败都会执行）
    }

    public class HYTextResponse : HYBaseResponse
    {
        public int errCode;
    }

    [Preserve]
    public class GeneralCallbackResult 
    {
        /// <summary> 
        /// 错误信息
        /// </summary>
        public string errMsg;
    }


    [Preserve]
	public class InvokeOption
	{
        public string text;

		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<RequestFailCallbackErr> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<string> success { get; set; }
	}

    [Preserve]
    public class LoginSuccessCallbackResult 
    {
        /// <summary> 
        /// 调用接口获取登录凭证（code），通过凭证进而换取用户登录态信息。
        /// https://dev.huya.com/docs/miniapp/dev/game/api/#%E7%99%BB%E5%BD%95
        /// </summary>
        public string code;

        public string errMsg;
    }

    [Preserve]
	public class LoginOption : ICallback<LoginSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>
	{
		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<RequestFailCallbackErr> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<LoginSuccessCallbackResult> success { get; set; }
	}
    
	[Preserve]
	public class CheckSessionOption : ICallback<GeneralCallbackResult, GeneralCallbackResult, GeneralCallbackResult>
	{
		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> success { get; set; }
	}
	
	[Preserve]
	public class GetUserInfoOption : ICallback<GetUserInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>
	{
		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<GetUserInfoSuccessCallbackResult> success { get; set; }
	}

	[Preserve]
	public class GetUserInfoSuccessCallbackResult
	{
		/// <summary>
		/// 用户昵称
		/// </summary>
		public string nickName;

		/// <summary>
		/// 用户头像地址
		/// </summary>
		public string userAvatarUrl;

		/// <summary>
		/// 用户等级
		/// </summary>
		public int userLevel;

		/// <summary>
		/// 用户的unionId
		/// </summary>
		public string userUnionId;
	}

	[Preserve]
	public class GetStreamerInfoOption : ICallback<GetStreamerInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>
	{
		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<GetStreamerInfoSuccessCallbackResult> success { get; set; }
	}

	[Preserve]
	public class GetStreamerInfoSuccessCallbackResult
	{
		/// <summary>
		/// 主播昵称
		/// </summary>
		public string streamerNick;

		/// <summary>
		/// 主播头像地址
		/// </summary>
		public string streamerAvatarUrl;

		/// <summary>
		/// 主播性别；取值说明: 1 表示男; 2 表示女
		/// </summary>
		public int streamerSex;

		/// <summary>
		/// 主播等级
		/// </summary>
		public int streamerLevel;

		/// <summary>
		/// 房间Id
		/// </summary>
		public int streamerRoomId;

		/// <summary>
		/// 主播unionId，没有则为空
		/// </summary>
		public string streamerUnionId;
	}

	[Preserve]
	public class GetSystemInfoAsyncOption : ICallback<SystemInfo, GeneralCallbackResult, GeneralCallbackResult>
	{
		/// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<SystemInfo> success { get; set; }
	}

    [Preserve]
	public class RequestPaymentOption : ICallback<GeneralCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>
	{
        /// <summary>
		/// 时间戳
		/// </summary>
        public float timeStamp;

        /// <summary>
		/// 随机字符串，长度为32个字符以下
		/// </summary>
        public string nonceStr;

        /// <summary>
		/// 下单接口返回
		/// </summary>
        public string package;

        /// <summary>
		/// 签名算法类型，默认值：'MD5'
		/// </summary>
        public string signType;

        /// <summary>
		/// 签名 - 签名需使用开发者密钥，为了密钥安全，建议在请求服务端预下单时，由服务端返回相关的请求参数。
        /// 具体签名规则请查看服务端接入文档。
		/// </summary>
        public string paySign;

        /// <summary>
		/// 接口调用结束的回调函数（调用成功、失败都会执行）
		/// </summary>
		public Action<GeneralCallbackResult> complete { get; set; }

		/// <summary>
		/// 接口调用失败的回调函数
		/// </summary>
		public Action<RequestFailCallbackErr> fail { get; set; }

		/// <summary>
		/// 接口调用成功的回调函数
		/// </summary>
		public Action<GeneralCallbackResult> success { get; set; }
    }


	[Preserve]
	public class SafeArea
	{
		/// <summary>
		/// 安全区域左上角横坐标
		/// </summary>
		public float left;
		
		/// <summary>
		/// 安全区域右下角横坐标
		/// </summary>
		public float right;
		
		/// <summary>
		/// 安全区域左上角纵坐标
		/// </summary>
		public float top;
		
		/// <summary>
		/// 安全区域右下角纵坐标
		/// </summary>
		public float bottom;
		
		/// <summary>
		/// 安全区域的宽度，单位逻辑像素
		/// </summary>
		public float width;
		
		/// <summary>
		/// 安全区域的高度，单位逻辑像素
		/// </summary>
		public float height;
	}

	[Preserve]
	public class WindowInfoCallbackResult
	{
		/// <summary>
		/// 屏幕宽度
		/// </summary>
		public float screenWidth;
		
		/// <summary>
		/// 屏幕高度
		/// </summary>
		public float screenHeight;
		
		/// <summary>
		/// 屏幕是否是横屏
		/// </summary>
		public bool isLandscape;
		
		/// <summary>
		/// 可使用窗口宽度，单位 px
		/// </summary>
		public float windowWidth;
		
		/// <summary>
		/// 可使用窗口高度，单位 px
		/// </summary>
		public float windowHeight;
		
		/// <summary>
		/// 设备像素比
		/// </summary>
		public float pixelRatio;
		
		/// <summary>
		/// 状态栏的高度，单位 px
		/// </summary>
		public float statusBarHeight;
		
		/// <summary>
		/// 窗口上边缘的 y 值
		/// </summary>
		public float screenTop;
		
		/// <summary>
		/// 在竖屏正方向下的安全区域。部分机型没有安全区域概念，则不返回 safeArea 字段
		/// </summary>
		public SafeArea safeArea;
	}

	[Preserve]
	public class SystemInfo
	{
		/// <summary>
		/// 平台，示例："android"，"ios"
		/// </summary>
		public string platform;
		
		/// <summary>
		/// 虎牙直播app版本
		/// </summary>
		public string version;
		
		/// <summary>
		/// 虎牙直播app基础包版本
		/// </summary>
		public string baseVersion;
		
		/// <summary>
		/// 屏幕宽度
		/// </summary>
		public float screenWidth;
		
		/// <summary>
		/// 屏幕高度
		/// </summary>
		public float screenHeight;
		
		/// <summary>
		/// 屏幕是否是横屏
		/// </summary>
		public bool isLandscape;
		
		/// <summary>
		/// 可使用窗口宽度，单位 px
		/// </summary>
		public float windowWidth;
		
		/// <summary>
		/// 可使用窗口高度，单位 px
		/// </summary>
		public float windowHeight;
		
		/// <summary>
		/// 设备像素比
		/// </summary>
		public float pixelRatio;
		
		/// <summary>
		/// 状态栏的高度，单位 px
		/// </summary>
		public float statusBarHeight;
		
		/// <summary>
		/// 窗口上边缘的 y 值
		/// </summary>
		public float screenTop;
		
		/// <summary>
		/// 在竖屏正方向下的安全区域。部分机型没有安全区域概念，则不返回 safeArea 字段
		/// </summary>
		public SafeArea safeArea;
	}

}
