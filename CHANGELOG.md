<!-- 
Feature － 新增功能/接口
Changed - 功能/接口变更
Deprecated - 不建议使用的功能/接口
Removed - 删除功能/接口
Fixed - 修复问题
Others - 其他 
-->
## 2026-09-08 (1.0.8)

### Feature 

* 添加 HY.CreateRewardedVideoAd 接口
* 添加 HY.CreateCustomAd 接口
* HYCustomAd 支持 Open 打开原生模板组件
* HYCustomAd 支持 OnReward / OffReward 奖励通知
* 添加 hy.getRewardInfoByResId 接口

### Fixed

* 修正 GetStreamerInfoSuccessCallbackResult.streamerRoomId 类型：平台返回的是字符串，声明成 int 会让回包解析抛异常，在 WebGL 下直接 abort 整个游戏
* GetUserInfoSuccessCallbackResult 补充 userNick 字段（hyExt.context.getUserInfoSafe 的实际返回字段名，nickName 保留兼容）
* 广告实例创建时不再自动调用 offLoad/offError/offClose（对齐参考工程：off 只属于销毁阶段）；改为在 Destroy 时先取消全部监听再销毁实例
* 修正创建广告实例时 conf 参数未按字符指针转换的问题：adUnitId 会整块丢失，导致 load 时报 1004 adUnitId is empty
* 回调反序列化不再让异常穿透 wasm 边界：解析失败降级为错误日志并跳过本次回调，业务侧回调抛异常也只记日志
* LitJson 反序列化对标量类型做宽松兼容（number / string / bool 之间常见偏差、值类型字段收到 null），不再因平台字段类型与文档不一致而抛异常

## 2025-12-19 (1.0.7)

### Feature 

* 添加 hy.getUserInfoSafe 接口
* 添加 hy.getStreamerInfoSafe 接口

## 2025-07-14 (1.0.5)

### Feature 

* 添加 hy.getSystemInfo 接口
* 添加 hy.getLaunchOptionsSync 接口
* 添加 hy.getNetworkType 接口

## 2025-05-28 (1.0.4)

### Removed

* 移出运营接口

## 2025-05-28 (1.0.0)

### Feature

* 独立域插件更新
