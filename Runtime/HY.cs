#if UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace HuyaWASM
{
    /// <summary>
    /// 虎牙SDK的API
    /// </summary>
    public class HY
    {
        /// <summary>
        /// 判断判断当前基础库是否支持某个HY API
        /// 例如想要判断当前基础库HY.GetUserInfo是否可用，可以用HY.CanIUse("GetUserInfo")来判断
        /// </summary>
        public static bool CanIUse(string key)
        {
            return HYSDKManagerHandler.Instance.CanIUse(key);
        }

        public static void Invoke(string functionName, InvokeOption option) 
        {
            HYSDKManagerHandler.Instance.Invoke(functionName, option);
        }

        public static void CallJSFunction(string sdkName, string functionName, params object[] args)
        {
            HYSDKManagerHandler.CallJSFunction(sdkName, functionName, args);
        }

        public static string CallJSFunctionWithReturn(string sdkName, string functionName, params object[] args)
        {
            return HYSDKManagerHandler.CallJSFunctionWithReturn(sdkName, functionName, args);
        }

        public static void RemoveFirstScreen()
        {
            HYSDKManagerHandler.Instance.RemoveFirstScreen();
        }

        /// <summary>
        /// [hy.getWindowInfo()](https://dev.huya.com/docs/miniapp/dev/game/api/#%E8%8E%B7%E5%8F%96%E7%AA%97%E5%8F%A3%E4%BF%A1%E6%81%AF)
        /// 获取窗口信息
        /// </summary>
        public static WindowInfoCallbackResult GetWindowInfo()
        {
            return HYSDKManagerHandler.Instance.GetWindowInfo();
        }

        /// <summary>
        /// [hy.login(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E7%99%BB%E5%BD%95)
        /// 调用接口获取登录凭证（code），通过凭证进而换取用户登录态信息。
        /// </summary>
        public static void Login(LoginOption callback)
        {
            HYSDKManagerHandler.Instance.Login(callback);
        }

        /// <summary>
        /// [hy.checkSession(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E6%A3%80%E6%9F%A5%E7%99%BB%E5%BD%95%E6%80%81)
        /// 检查登录态是否过期。
        /// </summary>
        public static void CheckSession(CheckSessionOption callback)
        {
            HYSDKManagerHandler.Instance.CheckSession(callback);
        }

        /// <summary>
        /// [hy.getUserInfo(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E8%8E%B7%E5%8F%96%E7%94%A8%E6%88%B7%E4%BF%A1%E6%81%AF)
        /// 获取用户信息
        /// </summary>
        public static void GetUserInfo(GetUserInfoOption callback)
        {
            HYSDKManagerHandler.Instance.GetUserInfo(callback);
        }

        /// <summary>
        /// [hy.getStreamerInfo(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E8%8E%B7%E5%8F%96%E4%B8%BB%E6%92%AD%E4%BF%A1%E6%81%AF)
        /// 获取主播昵称和头像
        /// hy.getStreamerInfo(Object object)
        /// </summary>
        public static void GetStreamerInfo(GetStreamerInfoOption callback)
        {
            HYSDKManagerHandler.Instance.GetStreamerInfo(callback);
        }
        
        /// <summary>
        /// [hy.getSystemInfo(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E8%8E%B7%E5%8F%96%E7%B3%BB%E7%BB%9F%E4%BF%A1%E6%81%AF)
        /// 获取系统信息
        /// </summary>
        public static void GetSystemInfo(GetSystemInfoOption callback)
        {
            HYSDKManagerHandler.Instance.GetSystemInfo(callback);
        }

        /// <summary>
        /// [hy.requestPayment(Object object)](https://dev.huya.com/docs/miniapp/dev/game/api/#%E6%94%AF%E4%BB%98)
        /// 支付有可能存在延迟到账问题，建议游戏在收到回调后，向游戏服务端轮询发货结果，间隔 3 秒，持续约 1 ～ 2 分钟。 
        /// 同时也存在一些异常情况，导致充值成功后结果回调失败，因此建议游戏在启用游戏时主动查询余额，并且提供给用户主动刷新的功能。
        /// </summary>
        public static void RequestPayment(RequestPaymentOption callback) 
        {
            HYSDKManagerHandler.Instance.RequestPayment(callback);
        }

        /// <summary>
        /// [hy.startGame()](https://dev.huya.com/docs/miniapp/dev/game/api/#%E5%BC%80%E5%A7%8B%E6%B8%B8%E6%88%8F)
        /// 开始游戏时调用
        /// </summary>
        public static void StartGame()
        {
            HYSDKManagerHandler.Instance.StartGame();
        }

        /// <summary>
        /// [hyExt.env.getLaunchOptionsSync()](https://dev.huya.com/docs/miniapp/dev/game/api/#%E5%90%8C%E6%AD%A5%E8%8E%B7%E5%8F%96%E5%86%B7%E5%90%AF%E5%8A%A8%E6%97%B6%E7%9A%84%E5%8F%82%E6%95%B0)
        /// 同步获取冷启动时的参数
        /// </summary>
        public static LaunchOptionsGame GetLaunchOptionsSync()
        {
            return HYSDKManagerHandler.Instance.GetLaunchOptionsSync();
        }
        
        /// <summary>
        /// [hy.getNetworkType()](https://dev.huya.com/docs/miniapp/dev/game/api/#%E8%8E%B7%E5%8F%96%E7%BD%91%E7%BB%9C%E7%B1%BB%E5%9E%8B)
        /// 获取网络类型
        /// </summary>
        public static void GetNetworkType(GetNetworkTypeOption callback)
        {
            HYSDKManagerHandler.Instance.GetNetworkType(callback);
        }
    }
}
#endif
