using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace HuyaWASM
{
    /// <summary>
    /// 创建激励视频广告组件的参数，对应 hy.createRewardedVideoAd(Object object)
    /// </summary>
    [Preserve]
    public class HYCreateRewardedVideoAdParam
    {
        /// <summary>
        /// 广告单元 id
        /// </summary>
        public string adUnitId;

        /// <summary>
        /// 是否启用多例模式，默认为 false
        /// </summary>
        public bool multiton;

        /// <summary>
        /// 是否禁用分享页，默认为 false
        /// </summary>
        public bool disableFallbackSharePage;
    }

    /// <summary>
    /// 创建原生模板广告组件的参数，对应 hy.createCustomAd(Object object)
    /// </summary>
    [Preserve]
    public class HYCreateCustomAdParam
    {
        /// <summary>
        /// 广告单元 id
        /// </summary>
        public string adUnitId;

        /// <summary>
        /// 广告自动刷新的间隔时间，单位为秒，参数值必须大于等于 30（仅对支持自动刷新的模板生效）
        /// </summary>
        public int adIntervals;

        /// <summary>
        /// 原生模板广告组件的样式
        /// </summary>
        public HYAdStyle style;
    }

    /// <summary>
    /// 原生模板广告组件的样式
    /// </summary>
    [Preserve]
    public class HYAdStyle
    {
        /// <summary>
        /// 广告组件的左上角横坐标
        /// </summary>
        public float left;

        /// <summary>
        /// 广告组件的左上角纵坐标
        /// </summary>
        public float top;

        /// <summary>
        /// 广告组件的宽度（仅在某些模板生效，如矩阵格子）
        /// </summary>
        public float width;

        /// <summary>
        /// 广告组件是否固定屏幕位置（不跟随屏幕滚动）
        /// </summary>
        public bool @fixed;
    }

    /// <summary>
    /// 广告实例方法（load/show/hide）的回调参数
    /// </summary>
    [Preserve]
	public class HYAdOperationOption : ICallback<GeneralCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>
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
		public Action<GeneralCallbackResult> success { get; set; }
	}

    /// <summary>
    /// 广告组件事件回传的通用数据
    /// </summary>
    [Preserve]
    public class HYADEventResponse
    {
        /// <summary>
        /// 广告实例 id,调用者不需要关注
        /// </summary>
        public string callbackId;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errMsg;

        /// <summary>
        /// 错误码
        /// </summary>
        public double errCode;

        /// <summary>
        /// 激励视频是否完整播放结束（仅 onClose 事件有效）
        /// </summary>
        public bool isEnded;

        /// <summary>
        /// 广告宽度（仅 onResize 事件有效）
        /// </summary>
        public float width;

        /// <summary>
        /// 广告高度（仅 onResize 事件有效）
        /// </summary>
        public float height;

        /// <summary>
        /// 原生模板广告实例 id（仅 onReward 事件有效）
        /// </summary>
        public string customAdId;

        /// <summary>
        /// 是否发奖成功（仅 onReward 事件有效）
        /// </summary>
        public bool isSuccess;
    }

    /// <summary>
    /// [hy.getRewardInfoByResId(Object object)] 按广告单元 id 获取奖励信息
    /// </summary>
    [Preserve]
    public class GetRewardInfoByResIdOption : ICallback<GetRewardInfoByResIdSuccessCallbackResult, RequestFailCallbackErr, GeneralCallbackResult>
    {
        /// <summary>
        /// 广告单元 id
        /// </summary>
        public string adUnitId;

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
        public Action<GetRewardInfoByResIdSuccessCallbackResult> success { get; set; }
    }

    /// <summary>
    /// 奖励信息接口返回，mpPrizeList 以广告单元 id 为 key
    /// </summary>
    [Preserve]
    public class GetRewardInfoByResIdSuccessCallbackResult
    {
        public string errMsg;

        /// <summary>
        /// key 为 adUnitId
        /// </summary>
        public Dictionary<string, HYAdPrizeBlock> mpPrizeList;
    }

    [Preserve]
    public class HYAdPrizeBlock
    {
        /// <summary>
        /// 是否有可领取的奖励，1 表示有
        /// </summary>
        public int iHasPrize;

        public List<HYAdPrizeInfo> vPrizeInfo;
    }

    [Preserve]
    public class HYAdPrizeInfo
    {
        /// <summary>
        /// 奖励类型
        /// </summary>
        public int iPrizeType;

        /// <summary>
        /// 奖励数量
        /// </summary>
        public int iInnerPrizeNum;

        /// <summary>
        /// 奖励 id
        /// </summary>
        public long lInnerPrizeId;
    }
}
