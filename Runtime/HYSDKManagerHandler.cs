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
        public static string GetCallbackId<T>(Dictionary<string, T> dict) {
            var id = dict.Count;
            var res = (id + UnityEngine.Random.value).ToString();
            while (dict.ContainsKey(res))
            {
                id++;
                res = (id + UnityEngine.Random.value).ToString();
            }
            return res;
        }

        public void OneWayCallback<TConfig, TSuccess, TFail, TComplete>(string msg, Dictionary<string, TConfig> configList) where TConfig : class, ICallback<TSuccess, TFail, TComplete>, new()
		{
            if (!string.IsNullOrEmpty(msg) && configList != null) {
                HYJSCallback jsCallback = JsonUtility.FromJson<HYJSCallback>(msg);
                string callbackId = jsCallback.callbackId;
                string type = jsCallback.type;
				string res = jsCallback.res;
                if (configList.ContainsKey(callbackId)) 
                {
                    TConfig tconfig = configList[callbackId];
                    if (type == "complete") 
                    {
                        Action<TComplete> complete = tconfig.complete;
                        if (complete != null) 
                        {
							complete(JsonMapper.ToObject<TComplete>(res));
						}
                        tconfig.complete = null;
                    } else {
                        if (type == "success") 
                        {
                            Action<TSuccess> success = tconfig.success;
                            if (success != null) 
                            {
                                success(JsonMapper.ToObject<TSuccess>(res));
                            }
                        } 
                        else if (type == "fail")
                        {
                            Action<TFail> fail = tconfig.fail;
                            if (fail != null) 
                            {
                                fail(JsonMapper.ToObject<TFail>(res));
                            }
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
		private static extern void HY_CallJSFunction(string sdkName, string functionName, string text);

		[DllImport("__Internal")]
		private static extern string HY_CallJSFunctionWithReturn(string sdkName, string functionName, string text);

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

            if (!string.IsNullOrEmpty(msg) && InvokeOptionList != null) {
                HYJSCallback jsCallback = JsonUtility.FromJson<HYJSCallback>(msg);
                string callbackId = jsCallback.callbackId;
                string type = jsCallback.type;
				string res = jsCallback.res;
                if (InvokeOptionList.ContainsKey(callbackId)) 
                {
                    InvokeOption tconfig = InvokeOptionList[callbackId];
                    if (type == "complete") 
                    {
                        Action<GeneralCallbackResult> complete = tconfig.complete;
                        if (complete != null) 
                        {
							complete(JsonMapper.ToObject<GeneralCallbackResult>(res));
						}
                        tconfig.complete = null;
                    } else {
                        if (type == "success") 
                        {
                            Action<string> success = tconfig.success;
                            if (success != null) 
                            {
                                success(res);
                            }
                        } 
                        else if (type == "fail")
                        {
                            Action<RequestFailCallbackErr> fail = tconfig.fail;
                            if (fail != null) 
                            {
                                fail(JsonMapper.ToObject<RequestFailCallbackErr>(res));
                            }
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

        private Dictionary<string, GetUserInfoOption> GetUserInfoOptionList;
        public void GetUserInfo(GetUserInfoOption option)
		{
			if (GetUserInfoOptionList == null) 
            {
                GetUserInfoOptionList = new Dictionary<string, GetUserInfoOption>();
            }
            this.OneWayFunction<GetUserInfoOption, GetUserInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>("GetUserInfo", option, GetUserInfoOptionList);
		}

		public void GetUserInfoCallback(string msg)
		{
			this.OneWayCallback<GetUserInfoOption, GetUserInfoSuccessCallbackResult, GeneralCallbackResult, GeneralCallbackResult>(msg, GetUserInfoOptionList);
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

        private Dictionary<string, GetSystemInfoAsyncOption> GetSystemInfoAsyncOptionList;
        public void GetSystemInfoAsync(GetSystemInfoAsyncOption option)
		{
			if (GetSystemInfoAsyncOptionList == null) 
            {
                GetSystemInfoAsyncOptionList = new Dictionary<string, GetSystemInfoAsyncOption>();
            }
			this.OneWayFunction<GetSystemInfoAsyncOption, SystemInfo, GeneralCallbackResult, GeneralCallbackResult>("GetSystemInfoAsync", option, GetSystemInfoAsyncOptionList);
		}

		public void GetSystemInfoAsyncCallback(string msg)
		{
			this.OneWayCallback<GetSystemInfoAsyncOption, SystemInfo, GeneralCallbackResult, GeneralCallbackResult>(msg, GetSystemInfoAsyncOptionList);
		}
#endregion

#region 联运运营接口
        public void StartGame(StartGameOption option)
		{
            CallJSFunction("hy", "startGame", option);
		}

        public void UpdateGameInfo(UpdateGameInfoOption option)
		{
            CallJSFunction("hy", "updateGameInfo", option);
		}

        public void CreateRole(CreateRoleOption option)
		{
            CallJSFunction("hy", "createRole", option);
		}
#endregion
    }
}
