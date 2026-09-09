using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using LitJson;

namespace HuyaWASM
{
    public class HYSDKManagerHandler : MonoBehaviour
    {
        public static string GetCallbackId<T>(Dictionary<string, T> dict)
        {
            var id = dict.Count;
            var res = (id + UnityEngine.Random.value).ToString();
            while (dict.ContainsKey(res))
            {
                id++;
                res = (id + UnityEngine.Random.value).ToString();
            }
            return res;
        }

        /// <summary>
        /// 解析平台回包并执行用户回调。
        /// WebGL 下从 JS 侧 SendMessage 进来的调用栈里抛出的 C# 异常无法穿过 wasm 边界，
        /// 会直接触发 abort() 让整个游戏挂掉（例如平台把 int 字段返回成字符串），
        /// 所以这里统一降级为错误日志：解析失败则跳过本次回调，回调异常不影响其它实例。
        /// </summary>
        private static void DispatchCallback<T>(Action<T> action, string res, string callbackId, string type)
        {
            if (action == null)
            {
                return;
            }
            T result;
            try
            {
                result = JsonMapper.ToObject<T>(res);
            }
            catch (Exception e)
            {
                Debug.LogError("[HY] 回调数据解析失败，已跳过本次回调 callbackId=" + callbackId + " type=" + type + " 目标类型=" + typeof(T).Name + " 原因=" + e.Message + "\nres=" + res);
                return;
            }
            try
            {
                action(result);
            }
            catch (Exception e)
            {
                Debug.LogError("[HY] 回调执行异常 callbackId=" + callbackId + " type=" + type + "\n" + e);
            }
        }

        /// <summary>
        /// HY.Invoke 的 success 直接拿原始字符串，不涉及反序列化，只需挡住业务侧异常。
        /// </summary>
        private static void DispatchRawCallback(Action<string> action, string res, string callbackId, string type)
        {
            if (action == null)
            {
                return;
            }
            try
            {
                action(res);
            }
            catch (Exception e)
            {
                Debug.LogError("[HY] 回调执行异常 callbackId=" + callbackId + " type=" + type + "\n" + e);
            }
        }

        public void OneWayCallback<TConfig, TSuccess, TFail, TComplete>(string msg, Dictionary<string, TConfig> configList) where TConfig : class, ICallback<TSuccess, TFail, TComplete>, new()
        {
            if (!string.IsNullOrEmpty(msg) && configList != null)
            {
                HYJSCallback jsCallback;
                try
                {
                    jsCallback = JsonUtility.FromJson<HYJSCallback>(msg);
                }
                catch (Exception e)
                {
                    Debug.LogError("[HY] 回调报文解析失败，已忽略本次回调: " + e.Message + "\n" + msg);
                    return;
                }
                if (jsCallback == null)
                {
                    return;
                }
                string callbackId = jsCallback.callbackId;
                string type = jsCallback.type;
                string res = jsCallback.res;
                if (configList.ContainsKey(callbackId))
                {
                    TConfig tconfig = configList[callbackId];
                    if (type == "complete")
                    {
                        DispatchCallback(tconfig.complete, res, callbackId, type);
                        tconfig.complete = null;
                    }
                    else
                    {
                        if (type == "success")
                        {
                            DispatchCallback(tconfig.success, res, callbackId, type);
                        }
                        else if (type == "fail")
                        {
                            DispatchCallback(tconfig.fail, res, callbackId, type);
                        }
                        tconfig.success = null;
                        tconfig.fail = null;
                    }
                    if (tconfig.complete == null && tconfig.success == null && tconfig.fail == null)
                    {
                        configList.Remove(callbackId);
                    }
                }
            }
        }

        public void OneWayFunction<TConfig, TSuccess, TFail, TComplete>(string functionName, TConfig config, Dictionary<string, TConfig> configList) where TConfig : class, ICallback<TSuccess, TFail, TComplete>, new()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            string successType = typeof(TSuccess).Name;
            string failType = typeof(TFail).Name;
            string completeType = typeof(TComplete).Name;
            string callbackId = GetCallbackId<TConfig>(configList);
            TConfig tconfig = Activator.CreateInstance<TConfig>();
            tconfig.success = config.success;
            tconfig.fail = config.fail;
            tconfig.complete = config.complete;
            TConfig tconfig2 = tconfig;
            configList.Add(callbackId, tconfig2);
            Action<TSuccess> success = config.success;
            Action<TFail> fail = config.fail;
            Action<TComplete> complete = config.complete;
            config.success = null;
            config.fail = null;
            config.complete = null;
            string text = JsonMapper.ToJson(config);
            config.success = success;
            config.fail = fail;
            config.complete = complete;
            HY_OneWayFunction(functionName, successType, failType, completeType, text, callbackId);
        }

        [DllImport("__Internal")]
        private static extern void HYPointer_stringify_adaptor(string str);
        [DllImport("__Internal")]
        private static extern void HY_OneWayFunction(string functionName, string successType, string failType, string completeType, string conf, string callbackId);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_v(string functionName);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_vs(string functionName, string param1);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_vt(string functionName, string param1);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_vst(string functionName, string param1, string param2);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_vsn(string functionName, string param1, string param2);
        [DllImport("__Internal")]
        private static extern void HY_OneWayNoFunction_vnns(string functionName, string param1, string param2, string param3);

        [DllImport("__Internal")]
        private static extern bool HYCanIUse(string key);

        public bool CanIUse(string key)
        {
            return HYCanIUse(key);
        }

        [DllImport("__Internal")]
        private static extern void HY_RemoveFirstScreen();

        public void RemoveFirstScreen()
        {
            HY_RemoveFirstScreen();
        }

        [DllImport("__Internal")]
        private static extern string HY_SyncFunction_t(string functionName, string returnType);

        [DllImport("__Internal")]
        private static extern void HY_CallJSFunction(string sdkName, string functionName, string args);

        [DllImport("__Internal")]
        private static extern string HY_CallJSFunctionWithReturn(string sdkName, string functionName, string args);

        public static void CallJSFunction(string sdkName, string functionName, params object[] args)
        {
            string text = JsonMapper.ToJson(args);
            HY_CallJSFunction(sdkName, functionName, text);
        }

        public static string CallJSFunctionWithReturn(string sdkName, string functionName, params object[] args)
        {
            string text = JsonMapper.ToJson(args);
            return HY_CallJSFunctionWithReturn(sdkName, functionName, text);
        }


        [DllImport("__Internal")]
        private static extern void HY_Invoke(string functionName, string successType, string failType, string completeType, string conf, string callbackId);

        private Dictionary<string, InvokeOption> InvokeOptionList;
        public void Invoke(string functionName, InvokeOption option)
        {
            if (InvokeOptionList == null)
            {
                InvokeOptionList = new Dictionary<string, InvokeOption>();
            }

            string successType = typeof(string).Name;
            string failType = typeof(RequestFailCallbackErr).Name;
            string completeType = typeof(GeneralCallbackResult).Name;
            string callbackId = GetCallbackId<InvokeOption>(InvokeOptionList);
            InvokeOption tconfig = Activator.CreateInstance<InvokeOption>();
            tconfig.success = option.success;
            tconfig.fail = option.fail;
            tconfig.complete = option.complete;
            InvokeOption tconfig2 = tconfig;
            InvokeOptionList.Add(callbackId, tconfig2);
            Action<string> success = option.success;
            Action<RequestFailCallbackErr> fail = option.fail;
            Action<GeneralCallbackResult> complete = option.complete;
            HY_Invoke(functionName, successType, failType, completeType, option.text, callbackId);
        }

        public void InvokeCallback(string msg)
        {
            Debug.Log("InvokeCallback: " + msg);

            if (!string.IsNullOrEmpty(msg) && InvokeOptionList != null)
            {
                HYJSCallback jsCallback;
                try
                {
                    jsCallback = JsonUtility.FromJson<HYJSCallback>(msg);
                }
                catch (Exception e)
                {
                    Debug.LogError("[HY] Invoke 回调报文解析失败，已忽略本次回调: " + e.Message + "\n" + msg);
                    return;
                }
                if (jsCallback == null)
                {
                    return;
                }
                string callbackId = jsCallback.callbackId;
                string type = jsCallback.type;
                string res = jsCallback.res;
                if (InvokeOptionList.ContainsKey(callbackId))
                {
                    InvokeOption tconfig = InvokeOptionList[callbackId];
                    if (type == "complete")
                    {
                        DispatchCallback(tconfig.complete, res, callbackId, type);
                        tconfig.complete = null;
                    }
                    else
                    {
                        if (type == "success")
                        {
                            DispatchRawCallback(tconfig.success, res, callbackId, type);
                        }
                        else if (type == "fail")
                        {
                            DispatchCallback(tconfig.fail, res, callbackId, type);
                        }
                        tconfig.success = null;
                        tconfig.fail = null;
                    }
                    if (tconfig.complete == null && tconfig.success == null && tconfig.fail == null)
                    {
                        InvokeOptionList.Remove(callbackId);
                    }
                }
            }
        }

        #region Instance
        private static HYSDKManagerHandler instance = null;

        public static HYSDKManagerHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!Application.isPlaying)
                    {
                        Debug.LogError("不支持在非播放模式下调用HY接口");
                        return null;
                    }
                    instance = new GameObject(typeof(HYSDKManagerHandler).Name).AddComponent<HYSDKManagerHandler>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        protected void OnDestroy()
        {
            if (instance != null)
                instance = null;
        }
        #endregion

        #region 登录
        private Dictionary<string, LoginOption> LoginOptionList;
        public void Login(LoginOption option)
        {
            if (LoginOptionList == null)
            {
                LoginOptionList = new Dictionary<string, LoginOption>();
            }
            this.OneWayFunction<LoginOption, LoginSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("Login", option, LoginOptionList);
        }

        public void LoginCallback(string msg)
        {
            this.OneWayCallback<LoginOption, LoginSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, LoginOptionList);
        }

        private Dictionary<string, CheckSessionOption> CheckSessionOptionList;
        public void CheckSession(CheckSessionOption option)
        {
            if (CheckSessionOptionList == null)
            {
                CheckSessionOptionList = new Dictionary<string, CheckSessionOption>();
            }
            this.OneWayFunction<CheckSessionOption, GeneralCallbackResult, GeneralCallbackResult, GeneralCallbackResult>("CheckSession", option, CheckSessionOptionList);
        }

        public void CheckSessionCallback(string msg)
        {
            this.OneWayCallback<CheckSessionOption, GeneralCallbackResult, GeneralCallbackResult, GeneralCallbackResult>(msg, CheckSessionOptionList);
        }

        // private Dictionary<string, GetUserInfoOption> GetUserInfoOptionList;
        // public void GetUserInfo(GetUserInfoOption option)
        // {
        //     if (GetUserInfoOptionList == null)
        //     {
        //         GetUserInfoOptionList = new Dictionary<string, GetUserInfoOption>();
        //     }
        //     this.OneWayFunction<GetUserInfoOption, GetUserInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>("GetUserInfo", option, GetUserInfoOptionList);
        // }
        //
        // public void GetUserInfoCallback(string msg)
        // {
        //     this.OneWayCallback<GetUserInfoOption, GetUserInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>(msg, GetUserInfoOptionList);
        // }
        //
        // private Dictionary<string, GetStreamerInfoOption> GetStreamerInfoOptionList;
        // public void GetStreamerInfo(GetStreamerInfoOption option)
        // {
        //     if (GetStreamerInfoOptionList == null)
        //     {
        //         GetStreamerInfoOptionList = new Dictionary<string, GetStreamerInfoOption>();
        //     }
        //     this.OneWayFunction<GetStreamerInfoOption, GetStreamerInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>("GetStreamerInfo", option, GetStreamerInfoOptionList);
        // }
        //
        // public void GetStreamerInfoCallback(string msg)
        // {
        //     this.OneWayCallback<GetStreamerInfoOption, GetStreamerInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>(msg, GetStreamerInfoOptionList);
        // }

        private Dictionary<string, GetUserInfoSafeOption> GetUserInfoSafeOptionList;
        public void GetUserInfoSafe(GetUserInfoSafeOption option)
        {
            if (GetUserInfoSafeOptionList == null)
            {
                GetUserInfoSafeOptionList = new Dictionary<string, GetUserInfoSafeOption>();
            }
            this.OneWayFunction<GetUserInfoSafeOption, GetUserInfoSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("GetUserInfoSafe", option, GetUserInfoSafeOptionList);
        }

        public void GetUserInfoSafeCallback(string msg)
        {
            this.OneWayCallback<GetUserInfoSafeOption, GetUserInfoSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, GetUserInfoSafeOptionList);
        }

        private Dictionary<string, GetStreamerInfoSafeOption> GetStreamerInfoSafeOptionList;
        public void GetStreamerInfoSafe(GetStreamerInfoSafeOption option)
        {
            if (GetStreamerInfoSafeOptionList == null)
            {
                GetStreamerInfoSafeOptionList = new Dictionary<string, GetStreamerInfoSafeOption>();
            }
            this.OneWayFunction<GetStreamerInfoSafeOption, GetStreamerInfoSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("GetStreamerInfoSafe", option, GetStreamerInfoSafeOptionList);
        }

        public void GetStreamerInfoSafeCallback(string msg)
        {
            this.OneWayCallback<GetStreamerInfoSafeOption, GetStreamerInfoSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, GetStreamerInfoSafeOptionList);
        }
        #endregion

        #region 支付
        private Dictionary<string, RequestPaymentOption> RequestPaymentOptionList;
        public void RequestPayment(RequestPaymentOption option)
        {
            if (RequestPaymentOptionList == null)
            {
                RequestPaymentOptionList = new Dictionary<string, RequestPaymentOption>();
            }
            this.OneWayFunction<RequestPaymentOption, GeneralCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("RequestPayment", option, RequestPaymentOptionList);
        }

        public void RequestPaymentCallback(string msg)
        {
            this.OneWayCallback<RequestPaymentOption, GeneralCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, RequestPaymentOptionList);
        }
        #endregion

        #region 系统
        public WindowInfoCallbackResult GetWindowInfo()
        {
            string json = CallJSFunctionWithReturn("hy", "getWindowInfo", "{}");
            return JsonMapper.ToObject<WindowInfoCallbackResult>(json);
        }

        private Dictionary<string, GetSystemInfoOption> GetSystemInfoOptionList;
        public void GetSystemInfo(GetSystemInfoOption option)
        {
            if (GetSystemInfoOptionList == null)
            {
                GetSystemInfoOptionList = new Dictionary<string, GetSystemInfoOption>();
            }
            this.OneWayFunction<GetSystemInfoOption, SystemInfo, RequestFailCallbackErr, GeneralCallbackResult>("GetSystemInfo", option, GetSystemInfoOptionList);
        }

        public void GetSystemInfoCallback(string msg)
        {
            this.OneWayCallback<GetSystemInfoOption, SystemInfo, RequestFailCallbackErr, GeneralCallbackResult>(msg, GetSystemInfoOptionList);
        }

        public LaunchOptionsGame GetLaunchOptionsSync()
        {
            string json = HYSDKManagerHandler.HY_SyncFunction_t("GetLaunchOptionsSync", "LaunchOptionsGame");
            return JsonMapper.ToObject<LaunchOptionsGame>(json);
        }

        private Dictionary<string, GetNetworkTypeOption> GetNetworkTypeOptionList;
        public void GetNetworkType(GetNetworkTypeOption option)
        {
            if (GetNetworkTypeOptionList == null)
            {
                GetNetworkTypeOptionList = new Dictionary<string, GetNetworkTypeOption>();
            }
            this.OneWayFunction<GetNetworkTypeOption, GetNetworkTypeSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("GetNetworkType", option, GetNetworkTypeOptionList);
        }

        public void GetNetworkTypeCallback(string msg)
        {
            this.OneWayCallback<GetNetworkTypeOption, GetNetworkTypeSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, GetNetworkTypeOptionList);
        }
        #endregion

        #region 联运运营接口
        public void StartGame()
        {
            HY_OneWayNoFunction_v("startGame");
        }
        #endregion

        #region 广告
        /// <summary>
        /// 已创建的广告实例，key 为 JS 侧返回的广告实例 id
        /// </summary>
        public static readonly Dictionary<string, HYAdBase> AdInstanceList = new Dictionary<string, HYAdBase>();

        private Dictionary<string, HYAdOperationOption> AdOperationOptionList;

        [DllImport("__Internal")]
        private static extern string HYCreateRewardedVideoAd(string conf);
        [DllImport("__Internal")]
        private static extern string HYCreateCustomAd(string conf);
        [DllImport("__Internal")]
        private static extern void HYADLoad(string id, string callbackId);
        [DllImport("__Internal")]
        private static extern void HYADShow(string id, string callbackId);
        [DllImport("__Internal")]
        private static extern void HYADHide(string id, string callbackId);
        [DllImport("__Internal")]
        private static extern void HYADOpen(string id, string callbackId);
        [DllImport("__Internal")]
        private static extern bool HYADDestroy(string id);
        [DllImport("__Internal")]
        private static extern bool HYADIsShow(string id);
        [DllImport("__Internal")]
        private static extern void HYADStyleChange(string id, string key, string value);
        [DllImport("__Internal")]
        private static extern string HYADGetStyleValue(string id, string key);

        /// <summary>
        /// [hy.createRewardedVideoAd(Object object)](https://developers.weixin.qq.com/minigame/dev/api/ad/wx.createRewardedVideoAd.html)
        /// 创建激励视频广告组件
        /// </summary>
        public HYRewardedVideoAd CreateRewardedVideoAd(HYCreateRewardedVideoAdParam param)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return null;
#endif
            if (param == null || string.IsNullOrEmpty(param.adUnitId))
            {
                Debug.LogError("CreateRewardedVideoAd 必须传入 adUnitId");
                return null;
            }
            Dictionary<string, object> conf = new Dictionary<string, object>();
            conf.Add("adUnitId", param.adUnitId);
            if (param.multiton)
            {
                conf.Add("multiton", true);
            }
            if (param.disableFallbackSharePage)
            {
                conf.Add("disableFallbackSharePage", true);
            }
            string id = HYCreateRewardedVideoAd(JsonMapper.ToJson(conf));
            return RegisterAd<HYRewardedVideoAd>(new HYRewardedVideoAd(id), id, "CreateRewardedVideoAd");
        }

        /// <summary>
        /// [hy.createCustomAd(Object object)](https://developers.weixin.qq.com/minigame/dev/api/ad/wx.createCustomAd.html)
        /// 创建原生模板广告组件
        /// </summary>
        public HYCustomAd CreateCustomAd(HYCreateCustomAdParam param)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return null;
#endif
            if (param == null || string.IsNullOrEmpty(param.adUnitId))
            {
                Debug.LogError("CreateCustomAd 必须传入 adUnitId");
                return null;
            }
            Dictionary<string, object> conf = new Dictionary<string, object>();
            conf.Add("adUnitId", param.adUnitId);
            if (param.adIntervals > 0)
            {
                conf.Add("adIntervals", param.adIntervals);
            }
            if (param.style != null)
            {
                Dictionary<string, object> style = new Dictionary<string, object>();
                style.Add("left", param.style.left);
                style.Add("top", param.style.top);
                if (param.style.width > 0)
                {
                    style.Add("width", param.style.width);
                }
                if (param.style.@fixed)
                {
                    style.Add("fixed", true);
                }
                conf.Add("style", style);
            }
            string id = HYCreateCustomAd(JsonMapper.ToJson(conf));
            return RegisterAd<HYCustomAd>(new HYCustomAd(id), id, "CreateCustomAd");
        }

        private static T RegisterAd<T>(T ad, string id, string functionName) where T : HYAdBase
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError(functionName + " 创建失败，请先用 HY.CanIUse 判断当前基础库是否支持");
                return null;
            }
            if (!AdInstanceList.ContainsKey(id))
            {
                AdInstanceList.Add(id, ad);
            }
            return ad;
        }

        public void ADLoad(string id, HYAdOperationOption option)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            string callbackId = RegisterAdOperation(option);
            HYADLoad(id, callbackId);
        }

        public void ADShow(string id, HYAdOperationOption option)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            string callbackId = RegisterAdOperation(option);
            HYADShow(id, callbackId);
        }

        public void ADHide(string id, HYAdOperationOption option)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            string callbackId = RegisterAdOperation(option);
            HYADHide(id, callbackId);
        }

        public void ADOpen(string id, HYAdOperationOption option)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            string callbackId = RegisterAdOperation(option);
            HYADOpen(id, callbackId);
        }

        private string RegisterAdOperation(HYAdOperationOption option)
        {
            if (option == null)
            {
                return "";
            }
            if (AdOperationOptionList == null)
            {
                AdOperationOptionList = new Dictionary<string, HYAdOperationOption>();
            }
            string callbackId = GetCallbackId<HYAdOperationOption>(AdOperationOptionList);
            HYAdOperationOption tconfig = Activator.CreateInstance<HYAdOperationOption>();
            tconfig.success = option.success;
            tconfig.fail = option.fail;
            tconfig.complete = option.complete;
            AdOperationOptionList.Add(callbackId, tconfig);
            option.success = null;
            option.fail = null;
            option.complete = null;
            return callbackId;
        }

        public void ADDestroy(string id)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            HYADDestroy(id);
            if (AdInstanceList.ContainsKey(id))
            {
                AdInstanceList.Remove(id);
            }
        }

        public bool ADIsShow(string id)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return false;
#endif
            return HYADIsShow(id);
        }

        public void ADStyleChange(string id, string key, string value)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return;
#endif
            HYADStyleChange(id, key, value);
        }

        public string ADGetStyleValue(string id, string key)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Debug.LogError("需要在虎牙小游戏环境运行!");
            return "";
#endif
            return HYADGetStyleValue(id, key);
        }

        /// <summary>
        /// 广告实例方法（load/show/hide）的成功失败回调
        /// </summary>
        public void HYADOperationCallback(string msg)
        {
            this.OneWayCallback<HYAdOperationOption, GeneralCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, AdOperationOptionList);
        }

        public void ADOnLoadCallback(string msg)
        {
            DispatchAdEvent("load", msg);
        }

        public void ADOnErrorCallback(string msg)
        {
            DispatchAdEvent("error", msg);
        }

        public void ADOnCloseCallback(string msg)
        {
            DispatchAdEvent("close", msg);
        }

        /// <summary>
        /// 激励视频广告的关闭事件，与普通关闭事件区分开
        /// </summary>
        public void ADOnVideoCloseCallback(string msg)
        {
            DispatchAdEvent("close", msg);
        }

        public void ADOnHideCallback(string msg)
        {
            DispatchAdEvent("hide", msg);
        }

        public void ADOnResizeCallback(string msg)
        {
            DispatchAdEvent("resize", msg);
        }

        /// <summary>
        /// 原生模板广告发奖事件（虎牙平台特有能力）
        /// </summary>
        public void ADOnRewardCallback(string msg)
        {
            DispatchAdEvent("reward", msg);
        }

        private void DispatchAdEvent(string type, string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            HYADEventResponse res;
            try
            {
                res = JsonUtility.FromJson<HYADEventResponse>(msg);
            }
            catch (Exception e)
            {
                Debug.LogError("[HY] 广告事件解析失败，已忽略: " + type + " " + e.Message + "\n" + msg);
                return;
            }
            if (res == null || string.IsNullOrEmpty(res.callbackId))
            {
                return;
            }
            if (!AdInstanceList.ContainsKey(res.callbackId))
            {
                Debug.LogWarning("收到已销毁广告实例的事件，已忽略: " + res.callbackId);
                return;
            }
            try
            {
                AdInstanceList[res.callbackId].Dispatch(type, res);
            }
            catch (Exception e)
            {
                // 与 OneWayCallback 同理：业务侧异常不能穿透 wasm 边界，否则整个游戏会 abort
                Debug.LogError("[HY] 广告事件回调执行异常 type=" + type + " callbackId=" + res.callbackId + "\n" + e);
            }
        }

        private Dictionary<string, GetRewardInfoByResIdOption> GetRewardInfoByResIdOptionList;

        /// <summary>
        /// hy.getRewardInfoByResId(Object object)
        /// 按广告单元 id 获取奖励信息，配合激励视频与原生模板广告发奖使用
        /// </summary>
        public void GetRewardInfoByResId(GetRewardInfoByResIdOption option)
        {
            if (GetRewardInfoByResIdOptionList == null)
            {
                GetRewardInfoByResIdOptionList = new Dictionary<string, GetRewardInfoByResIdOption>();
            }
            this.OneWayFunction<GetRewardInfoByResIdOption, GetRewardInfoByResIdSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>("GetRewardInfoByResId", option, GetRewardInfoByResIdOptionList);
        }

        public void GetRewardInfoByResIdCallback(string msg)
        {
            this.OneWayCallback<GetRewardInfoByResIdOption, GetRewardInfoByResIdSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>(msg, GetRewardInfoByResIdOptionList);
        }
        #endregion
    }
}
