using System;
using System.Globalization;
using UnityEngine;

namespace HuyaWASM
{
    /// <summary>
    /// 广告组件实例基类，通过 id 关联 JS 侧的广告实例
    /// </summary>
    public abstract class HYAdBase
    {
        internal string adId;
        internal Action<HYADEventResponse> loadHandler;
        internal Action<HYADEventResponse> errorHandler;
        internal Action<HYADEventResponse> closeHandler;

        /// <summary>
        /// 广告实例 id，调用者不需要关注
        /// </summary>
        public string Id
        {
            get { return adId; }
        }

        /// <summary>
        /// 监听广告加载事件
        /// </summary>
        public void OnLoad(Action<HYADEventResponse> handler)
        {
            loadHandler = handler;
        }

        /// <summary>
        /// 移除广告加载事件的监听函数
        /// </summary>
        public void OffLoad()
        {
            loadHandler = null;
        }

        /// <summary>
        /// 监听广告错误事件
        /// </summary>
        public void OnError(Action<HYADEventResponse> handler)
        {
            errorHandler = handler;
        }

        /// <summary>
        /// 移除广告错误事件的监听函数
        /// </summary>
        public void OffError()
        {
            errorHandler = null;
        }

        /// <summary>
        /// 监听广告关闭事件
        /// </summary>
        public void OnClose(Action<HYADEventResponse> handler)
        {
            closeHandler = handler;
        }

        /// <summary>
        /// 移除广告关闭事件的监听函数
        /// </summary>
        public void OffClose()
        {
            closeHandler = null;
        }

        /// <summary>
        /// 销毁广告实例，销毁后该实例不可再使用
        /// </summary>
        public void Destroy()
        {
            if (string.IsNullOrEmpty(adId))
            {
                return;
            }
            HYSDKManagerHandler.Instance.ADDestroy(adId);
            RemoveAllHandlers();
            adId = null;
        }

        internal virtual void RemoveAllHandlers()
        {
            loadHandler = null;
            errorHandler = null;
            closeHandler = null;
        }

        /// <summary>
        /// 由 HYSDKManagerHandler 分发 JS 侧回传的广告事件
        /// </summary>
        internal virtual void Dispatch(string type, HYADEventResponse res)
        {
            switch (type)
            {
                case "load":
                    if (loadHandler != null)
                    {
                        loadHandler(res);
                    }
                    break;
                case "error":
                    if (errorHandler != null)
                    {
                        errorHandler(res);
                    }
                    break;
                case "close":
                    if (closeHandler != null)
                    {
                        closeHandler(res);
                    }
                    break;
            }
        }

        protected internal void SetStyleValue(string key, string value)
        {
            if (string.IsNullOrEmpty(adId))
            {
                return;
            }
            HYSDKManagerHandler.Instance.ADStyleChange(adId, key, value);
        }

        protected internal float GetStyleFloat(string key)
        {
            if (string.IsNullOrEmpty(adId))
            {
                return 0f;
            }
            float value;
            var text = HYSDKManagerHandler.Instance.ADGetStyleValue(adId, key);
            if (string.IsNullOrEmpty(text) || !float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return 0f;
            }
            return value;
        }

        protected internal bool GetStyleBool(string key)
        {
            if (string.IsNullOrEmpty(adId))
            {
                return false;
            }
            var text = HYSDKManagerHandler.Instance.ADGetStyleValue(adId, key);
            return !string.IsNullOrEmpty(text) && text.Trim().ToLower() == "true";
        }
    }

    /// <summary>
    /// 激励视频广告组件，通过 HY.CreateRewardedVideoAd 创建
    /// 详见 https://developers.weixin.qq.com/minigame/dev/api/ad/RewardedVideoAd.html
    /// </summary>
    public class HYRewardedVideoAd : HYAdBase
    {
        internal HYRewardedVideoAd(string id)
        {
            adId = id;
        }

        /// <summary>
        /// 加载激励视频广告，成功后会触发 OnLoad 事件
        /// </summary>
        public void Load(HYAdOperationOption option = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                Debug.LogError("广告实例已销毁，无法调用 Load");
                return;
            }
            HYSDKManagerHandler.Instance.ADLoad(adId, option);
        }

        /// <summary>
        /// 显示激励视频广告，激励视频广告将从屏幕下方推入
        /// </summary>
        public void Show(HYAdOperationOption option = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                Debug.LogError("广告实例已销毁，无法调用 Show");
                return;
            }
            HYSDKManagerHandler.Instance.ADShow(adId, option);
        }
    }

    /// <summary>
    /// 原生模板广告组件，通过 HY.CreateCustomAd 创建
    /// 详见 https://developers.weixin.qq.com/minigame/dev/api/ad/CustomAd.html
    /// </summary>
    public class HYCustomAd : HYAdBase
    {
        internal Action<HYADEventResponse> hideHandler;
        internal Action<HYADEventResponse> resizeHandler;
        internal Action<HYADEventResponse> rewardHandler;
        private HYAdStyleAccessor styleAccessor;

        internal HYCustomAd(string id)
        {
            adId = id;
        }

        /// <summary>
        /// 原生模板广告组件的样式，赋值会即时同步到 JS 侧
        /// </summary>
        public HYAdStyleAccessor style
        {
            get
            {
                if (styleAccessor == null)
                {
                    styleAccessor = new HYAdStyleAccessor(this);
                }
                return styleAccessor;
            }
        }

        /// <summary>
        /// 显示原生模板广告
        /// </summary>
        public void Show(HYAdOperationOption option = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                Debug.LogError("广告实例已销毁，无法调用 Show");
                return;
            }
            HYSDKManagerHandler.Instance.ADShow(adId, option);
        }

        /// <summary>
        /// 隐藏原生模板广告（某些模板广告无法隐藏）
        /// </summary>
        public void Hide(HYAdOperationOption option = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                Debug.LogError("广告实例已销毁，无法调用 Hide");
                return;
            }
            HYSDKManagerHandler.Instance.ADHide(adId, option);
        }

        /// <summary>
        /// 打开原生模板组件（虎牙平台特有能力）
        /// </summary>
        public void Open(HYAdOperationOption option = null)
        {
            if (string.IsNullOrEmpty(adId))
            {
                Debug.LogError("广告实例已销毁，无法调用 Open");
                return;
            }
            HYSDKManagerHandler.Instance.ADOpen(adId, option);
        }

        /// <summary>
        /// 查询原生模板广告展示状态
        /// </summary>
        public bool IsShow()
        {
            if (string.IsNullOrEmpty(adId))
            {
                return false;
            }
            return HYSDKManagerHandler.Instance.ADIsShow(adId);
        }

        /// <summary>
        /// 监听原生模板广告隐藏事件，部分模板用户点击关闭时也会触发该事件
        /// </summary>
        public void OnHide(Action<HYADEventResponse> handler)
        {
            hideHandler = handler;
        }

        /// <summary>
        /// 移除原生模板广告隐藏事件的监听函数
        /// </summary>
        public void OffHide()
        {
            hideHandler = null;
        }

        /// <summary>
        /// 监听原生模板广告宽高回调事件（部分模板支持）
        /// </summary>
        public void OnResize(Action<HYADEventResponse> handler)
        {
            resizeHandler = handler;
        }

        /// <summary>
        /// 移除原生模板广告宽高回调事件的监听函数
        /// </summary>
        public void OffResize()
        {
            resizeHandler = null;
        }

        /// <summary>
        /// 监听原生模板广告发奖事件（虎牙平台特有能力）
        /// </summary>
        public void OnReward(Action<HYADEventResponse> handler)
        {
            rewardHandler = handler;
        }

        /// <summary>
        /// 移除原生模板广告发奖事件的监听函数
        /// </summary>
        public void OffReward()
        {
            rewardHandler = null;
        }

        internal override void RemoveAllHandlers()
        {
            base.RemoveAllHandlers();
            hideHandler = null;
            resizeHandler = null;
            rewardHandler = null;
        }

        internal override void Dispatch(string type, HYADEventResponse res)
        {
            if (type == "hide")
            {
                if (hideHandler != null)
                {
                    hideHandler(res);
                }
                return;
            }
            if (type == "resize")
            {
                if (resizeHandler != null)
                {
                    resizeHandler(res);
                }
                return;
            }
            if (type == "reward")
            {
                if (rewardHandler != null)
                {
                    rewardHandler(res);
                }
                return;
            }
            base.Dispatch(type, res);
        }
    }

    /// <summary>
    /// 广告组件样式的读写代理，读写都会即时同步到 JS 侧的广告实例
    /// </summary>
    public class HYAdStyleAccessor
    {
        private readonly HYAdBase ad;

        internal HYAdStyleAccessor(HYAdBase ad)
        {
            this.ad = ad;
        }

        /// <summary>
        /// 广告组件的左上角横坐标
        /// </summary>
        public float left
        {
            get { return ad.GetStyleFloat("left"); }
            set { ad.SetStyleValue("left", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// 广告组件的左上角纵坐标
        /// </summary>
        public float top
        {
            get { return ad.GetStyleFloat("top"); }
            set { ad.SetStyleValue("top", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// 广告组件的宽度（仅在某些模板生效，如矩阵格子）
        /// </summary>
        public float width
        {
            get { return ad.GetStyleFloat("width"); }
            set { ad.SetStyleValue("width", value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// 广告组件是否固定屏幕位置（不跟随屏幕滚动）
        /// </summary>
        public bool @fixed
        {
            get { return ad.GetStyleBool("fixed"); }
            set { ad.SetStyleValue("fixed", value ? "true" : "false"); }
        }
    }
}
