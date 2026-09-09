mergeInto(LibraryManager.library, {
HYPointer_stringify_adaptor:function(str){
    if (typeof UTF8ToString !== "undefined") {
        return UTF8ToString(str)
    }
    return Pointer_stringify(str)
},
HYCanIUse: function(key) {
    if (!key || !_HYPointer_stringify_adaptor(key)) {
        return false;
    }
    const keyString = _HYPointer_stringify_adaptor(key);
    return typeof hy[keyString[0].toLowerCase() + keyString.slice(1)] !== 'undefined';
},
HY_OneWayFunction:function(functionName, successType, failType, completeType, conf, callbackId) {
    window.HYWASMSDK.HY_OneWayFunction(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(successType), _HYPointer_stringify_adaptor(failType), _HYPointer_stringify_adaptor(completeType), _HYPointer_stringify_adaptor(conf), _HYPointer_stringify_adaptor(callbackId));
},
HY_OneWayNoFunction_v: function (functionName) {
    window.HYWASMSDK.HY_OneWayNoFunction_v(_HYPointer_stringify_adaptor(functionName));
},
HY_OneWayNoFunction_vs: function (functionName, param1) {
    window.HYWASMSDK.HY_OneWayNoFunction_vs(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(param1));
},
HY_OneWayNoFunction_vt: function (functionName, param1) {
    window.HYWASMSDK.HY_OneWayNoFunction_vt(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(param1));
},
HY_OneWayNoFunction_vst: function (functionName, param1, param2) {
    window.HYWASMSDK.HY_OneWayNoFunction_vst(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(param1), _HYPointer_stringify_adaptor(param2));
},
HY_OneWayNoFunction_vsn: function (functionName, param1, param2) {
    window.HYWASMSDK.HY_OneWayNoFunction_vsn(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(param1), param2);
},
HY_OneWayNoFunction_vnns: function (functionName, param1, param2, param3) {
    window.HYWASMSDK.HY_OneWayNoFunction_vnns(_HYPointer_stringify_adaptor(functionName), param1, param2, _HYPointer_stringify_adaptor(param3));
},
HY_CallJSFunction: function (sdkName, functionName, args) {
    var sdk = _HYPointer_stringify_adaptor(sdkName);
    var func = _HYPointer_stringify_adaptor(functionName);
    var formattedArgs = JSON.parse(_HYPointer_stringify_adaptor(args));
    window[sdk][func].apply(window[sdk], formattedArgs);
},
HY_CallJSFunctionWithReturn: function (sdkName, functionName, args) {
    var sdk = _HYPointer_stringify_adaptor(sdkName);
    var func = _HYPointer_stringify_adaptor(functionName);
    var formattedArgs = JSON.parse(_HYPointer_stringify_adaptor(args));
    var res = window[sdk][func].apply(window[sdk], formattedArgs);
    var resStr = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(resStr || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8((resStr || ''), buffer, bufferSize);
    return buffer;
},
HY_Invoke:function(functionName, successType, failType, completeType, conf, callbackId) {
    window.HYWASMSDK.HY_Invoke(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(successType), _HYPointer_stringify_adaptor(failType), _HYPointer_stringify_adaptor(completeType), _HYPointer_stringify_adaptor(conf), _HYPointer_stringify_adaptor(callbackId));
},

HY_SyncFunction_t: function(functionName, returnType){
    var res = window.HYWASMSDK.HY_SyncFunction_t(_HYPointer_stringify_adaptor(functionName), _HYPointer_stringify_adaptor(returnType));
    var bufferSize = lengthBytesUTF8(res || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8((res || ''), buffer, bufferSize);
    return buffer; 
},

HY_RemoveFirstScreen: function () {
    window.__removeFirstScreen && window.__removeFirstScreen();
},

HYCreateRewardedVideoAd: function (conf) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var send = function (method, payload) {
        var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
        if (!gi) {
            console.error('[HY] gameInstance 未就绪，无法回传广告事件: ' + method);
            return;
        }
        gi.SendMessage('HYSDKManagerHandler', method, JSON.stringify(payload));
    };
    // conf 从 C# 传过来是 wasm 里的字符指针，必须先转成字符串再解析，
    // 否则 JSON.parse 会把指针数字当成 JSON 数值，adUnitId 会整块丢失
    var config = {};
    try {
        config = JSON.parse(_HYPointer_stringify_adaptor(conf) || '{}') || {};
    } catch (e) {
        config = {};
    }
    if (!config || typeof config !== 'object') {
        config = {};
    }
    if (!config.adUnitId) {
        console.error('[HY] 创建广告实例缺少 adUnitId，端上会在 load 时报 1004 adUnitId is empty');
    }
    if (!window.hy || typeof window.hy.createRewardedVideoAd !== 'function') {
        console.error('[HY] 当前环境不支持 hy.createRewardedVideoAd，可用 HY.CanIUse("CreateRewardedVideoAd") 判断');
        return 0;
    }
    var ad;
    try {
        ad = window.hy.createRewardedVideoAd(config);
    } catch (e) {
        console.error(e);
        return 0;
    }
    if (!ad) {
        return 0;
    }
    var key = 'hyad' + Date.now() + Math.random();
    store[key] = ad;
    console.log('[HY] 激励视频实例方法: load=' + typeof ad.load + ' show=' + typeof ad.show
        + ' onLoad=' + typeof ad.onLoad + ' onError=' + typeof ad.onError + ' onClose=' + typeof ad.onClose);
    if (typeof ad.onLoad !== 'function') {
        console.error('[HY] 端上激励视频实例没有 onLoad，加载完成只能依赖 load() 返回的 Promise');
    }
    ad.onError && ad.onError(function (res) {
        res = res || {};
        console.error(res);
        send('ADOnErrorCallback', {
            callbackId: key,
            errMsg: res.errMsg || '',
            errCode: res.errCode || res.err_code || 0,
        });
    });
    ad.onLoad && ad.onLoad(function (res) {
        res = res || {};
        send('ADOnLoadCallback', { callbackId: key, errMsg: res.errMsg || '', errCode: res.errCode || res.err_code || 0 });
    });
    ad.onClose && ad.onClose(function (res) {
        send('ADOnVideoCloseCallback', {
            callbackId: key,
            errMsg: '',
            isEnded: !!(res && res.isEnded),
        });
    });
    var bufferSize = lengthBytesUTF8(key) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(key, buffer, bufferSize);
    return buffer;
},

HYCreateCustomAd: function (conf) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var send = function (method, payload) {
        var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
        if (!gi) {
            console.error('[HY] gameInstance 未就绪，无法回传广告事件: ' + method);
            return;
        }
        gi.SendMessage('HYSDKManagerHandler', method, JSON.stringify(payload));
    };
    // conf 从 C# 传过来是 wasm 里的字符指针，必须先转成字符串再解析，
    // 否则 JSON.parse 会把指针数字当成 JSON 数值，adUnitId 会整块丢失
    var config = {};
    try {
        config = JSON.parse(_HYPointer_stringify_adaptor(conf) || '{}') || {};
    } catch (e) {
        config = {};
    }
    if (!config || typeof config !== 'object') {
        config = {};
    }
    if (!config.adUnitId) {
        console.error('[HY] 创建广告实例缺少 adUnitId，端上会在 load 时报 1004 adUnitId is empty');
    }
    if (!window.hy || typeof window.hy.createCustomAd !== 'function') {
        console.error('[HY] 当前环境不支持 hy.createCustomAd，可用 HY.CanIUse("CreateCustomAd") 判断');
        return 0;
    }
    var ad;
    try {
        ad = window.hy.createCustomAd(config);
    } catch (e) {
        console.error(e);
        return 0;
    }
    if (!ad) {
        return 0;
    }
    var key = 'hyad' + Date.now() + Math.random();
    store[key] = ad;
    console.log('[HY] 模板广告实例方法: show=' + typeof ad.show + ' hide=' + typeof ad.hide + ' open=' + typeof ad.open
        + ' isShow=' + typeof ad.isShow + ' onLoad=' + typeof ad.onLoad + ' onError=' + typeof ad.onError
        + ' onClose=' + typeof ad.onClose + ' onHide=' + typeof ad.onHide
        + ' onResize=' + typeof ad.onResize + ' onReward=' + typeof ad.onReward);
    if (typeof ad.onLoad !== 'function') {
        console.error('[HY] 端上模板广告实例没有 onLoad，加载完成只能依赖 load() 返回的 Promise');
    }
    ad.onError && ad.onError(function (res) {
        res = res || {};
        console.error(res);
        send('ADOnErrorCallback', {
            callbackId: key,
            errMsg: res.errMsg || '',
            errCode: res.errCode || res.err_code || 0,
        });
    });
    ad.onLoad && ad.onLoad(function (res) {
        res = res || {};
        send('ADOnLoadCallback', { callbackId: key, errMsg: res.errMsg || '', errCode: res.errCode || res.err_code || 0 });
    });
    ad.onClose && ad.onClose(function (res) {
        res = res || {};
        send('ADOnCloseCallback', { callbackId: key, errMsg: res.errMsg || '' });
    });
    ad.onHide && ad.onHide(function (res) {
        res = res || {};
        send('ADOnHideCallback', { callbackId: key, errMsg: res.errMsg || '' });
    });
    ad.onResize && ad.onResize(function (res) {
        res = res || {};
        send('ADOnResizeCallback', {
            callbackId: key,
            errMsg: '',
            width: res.width || 0,
            height: res.height || 0,
        });
    });
    ad.onReward && ad.onReward(function (res) {
        res = res || {};
        send('ADOnRewardCallback', {
            callbackId: key,
            errMsg: '',
            customAdId: res.customAdId || '',
            isSuccess: !!res.isSuccess,
        });
    });
    var bufferSize = lengthBytesUTF8(key) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(key, buffer, bufferSize);
    return buffer;
},

HYADLoad: function (id, callbackId) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    var operationCallbackId = _HYPointer_stringify_adaptor(callbackId);
    var settle = function (apiName, runner, missingAsOk) {
        var sendResult = function (payload) {
            if (!operationCallbackId) {
                // 调用方未传回调时不回推结果，但广告动作本身仍要执行
                return;
            }
            var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
            if (!gi) {
                console.error('[HY] gameInstance 未就绪，无法回传广告操作结果');
                return;
            }
            gi.SendMessage('HYSDKManagerHandler', 'HYADOperationCallback', JSON.stringify(payload));
        };
        var ok = function () {
            sendResult({ callbackId: operationCallbackId, type: 'success', res: JSON.stringify({ errMsg: apiName + ':ok' }) });
        };
        var fail = function (e) {
            sendResult({ callbackId: operationCallbackId, type: 'fail', res: JSON.stringify({ errMsg: (e && e.errMsg) || String(e || apiName + ':fail') }) });
        };
        if (!ad) {
            fail({ errMsg: apiName + ':fail ad not found' });
            return;
        }
        if (typeof runner !== 'function') {
            // 能力缺失：部分模板广告不支持隐藏，按成功处理；其余按失败处理
            if (missingAsOk) {
                ok();
            } else {
                fail({ errMsg: apiName + ':fail not support' });
            }
            return;
        }
        try {
            var promise = runner(ad);
            if (!promise || typeof promise.then !== 'function') {
                console.log('[HY] ad.' + apiName + '() 未返回 Promise，动作已执行，按成功回推');
                ok();
                return;
            }
            // 端上无填充时 load()/show() 的 Promise 可能长期挂起，这里只记日志不改变行为
            var settled = false;
            var watchdog = setTimeout(function () {
                if (!settled) {
                    console.warn('[HY] ad.' + apiName + '() 超过 15s 未返回，端上可能无广告填充或环境不支持该广告位');
                }
            }, 15000);
            promise.then(function (res) {
                settled = true;
                clearTimeout(watchdog);
                ok();
            }, function (e) {
                settled = true;
                clearTimeout(watchdog);
                fail(e);
            });
        } catch (e) {
            fail(e);
        }
    };
    if (ad && typeof ad.load === 'function') {
        settle('load', function (target) { return target.load(); });
    } else {
        settle('load', null);
    }
},

HYADShow: function (id, callbackId) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    var operationCallbackId = _HYPointer_stringify_adaptor(callbackId);
    var settle = function (apiName, runner, missingAsOk) {
        var sendResult = function (payload) {
            if (!operationCallbackId) {
                // 调用方未传回调时不回推结果，但广告动作本身仍要执行
                return;
            }
            var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
            if (!gi) {
                console.error('[HY] gameInstance 未就绪，无法回传广告操作结果');
                return;
            }
            gi.SendMessage('HYSDKManagerHandler', 'HYADOperationCallback', JSON.stringify(payload));
        };
        var ok = function () {
            sendResult({ callbackId: operationCallbackId, type: 'success', res: JSON.stringify({ errMsg: apiName + ':ok' }) });
        };
        var fail = function (e) {
            sendResult({ callbackId: operationCallbackId, type: 'fail', res: JSON.stringify({ errMsg: (e && e.errMsg) || String(e || apiName + ':fail') }) });
        };
        if (!ad) {
            fail({ errMsg: apiName + ':fail ad not found' });
            return;
        }
        if (typeof runner !== 'function') {
            // 能力缺失：部分模板广告不支持隐藏，按成功处理；其余按失败处理
            if (missingAsOk) {
                ok();
            } else {
                fail({ errMsg: apiName + ':fail not support' });
            }
            return;
        }
        try {
            var promise = runner(ad);
            if (!promise || typeof promise.then !== 'function') {
                console.log('[HY] ad.' + apiName + '() 未返回 Promise，动作已执行，按成功回推');
                ok();
                return;
            }
            // 端上无填充时 load()/show() 的 Promise 可能长期挂起，这里只记日志不改变行为
            var settled = false;
            var watchdog = setTimeout(function () {
                if (!settled) {
                    console.warn('[HY] ad.' + apiName + '() 超过 15s 未返回，端上可能无广告填充或环境不支持该广告位');
                }
            }, 15000);
            promise.then(function (res) {
                settled = true;
                clearTimeout(watchdog);
                ok();
            }, function (e) {
                settled = true;
                clearTimeout(watchdog);
                fail(e);
            });
        } catch (e) {
            fail(e);
        }
    };
    if (ad && typeof ad.show === 'function') {
        settle('show', function (target) { return target.show(); });
    } else {
        settle('show', null);
    }
},

HYADHide: function (id, callbackId) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    var operationCallbackId = _HYPointer_stringify_adaptor(callbackId);
    var settle = function (apiName, runner, missingAsOk) {
        var sendResult = function (payload) {
            if (!operationCallbackId) {
                // 调用方未传回调时不回推结果，但广告动作本身仍要执行
                return;
            }
            var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
            if (!gi) {
                console.error('[HY] gameInstance 未就绪，无法回传广告操作结果');
                return;
            }
            gi.SendMessage('HYSDKManagerHandler', 'HYADOperationCallback', JSON.stringify(payload));
        };
        var ok = function () {
            sendResult({ callbackId: operationCallbackId, type: 'success', res: JSON.stringify({ errMsg: apiName + ':ok' }) });
        };
        var fail = function (e) {
            sendResult({ callbackId: operationCallbackId, type: 'fail', res: JSON.stringify({ errMsg: (e && e.errMsg) || String(e || apiName + ':fail') }) });
        };
        if (!ad) {
            fail({ errMsg: apiName + ':fail ad not found' });
            return;
        }
        if (typeof runner !== 'function') {
            // 能力缺失：部分模板广告不支持隐藏，按成功处理；其余按失败处理
            if (missingAsOk) {
                ok();
            } else {
                fail({ errMsg: apiName + ':fail not support' });
            }
            return;
        }
        try {
            var promise = runner(ad);
            if (!promise || typeof promise.then !== 'function') {
                console.log('[HY] ad.' + apiName + '() 未返回 Promise，动作已执行，按成功回推');
                ok();
                return;
            }
            // 端上无填充时 load()/show() 的 Promise 可能长期挂起，这里只记日志不改变行为
            var settled = false;
            var watchdog = setTimeout(function () {
                if (!settled) {
                    console.warn('[HY] ad.' + apiName + '() 超过 15s 未返回，端上可能无广告填充或环境不支持该广告位');
                }
            }, 15000);
            promise.then(function (res) {
                settled = true;
                clearTimeout(watchdog);
                ok();
            }, function (e) {
                settled = true;
                clearTimeout(watchdog);
                fail(e);
            });
        } catch (e) {
            fail(e);
        }
    };
    if (ad && typeof ad.hide === 'function') {
        settle('hide', function (target) { return target.hide(); }, true);
    } else {
        settle('hide', null, true);
    }
},

HYADOpen: function (id, callbackId) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    var operationCallbackId = _HYPointer_stringify_adaptor(callbackId);
    var settle = function (apiName, runner, missingAsOk) {
        var sendResult = function (payload) {
            if (!operationCallbackId) {
                // 调用方未传回调时不回推结果，但广告动作本身仍要执行
                return;
            }
            var gi = (typeof gameInstance !== 'undefined' && gameInstance) || window.gameInstance;
            if (!gi) {
                console.error('[HY] gameInstance 未就绪，无法回传广告操作结果');
                return;
            }
            gi.SendMessage('HYSDKManagerHandler', 'HYADOperationCallback', JSON.stringify(payload));
        };
        var ok = function () {
            sendResult({ callbackId: operationCallbackId, type: 'success', res: JSON.stringify({ errMsg: apiName + ':ok' }) });
        };
        var fail = function (e) {
            sendResult({ callbackId: operationCallbackId, type: 'fail', res: JSON.stringify({ errMsg: (e && e.errMsg) || String(e || apiName + ':fail') }) });
        };
        if (!ad) {
            fail({ errMsg: apiName + ':fail ad not found' });
            return;
        }
        if (typeof runner !== 'function') {
            if (missingAsOk) {
                ok();
            } else {
                fail({ errMsg: apiName + ':fail not support' });
            }
            return;
        }
        try {
            var promise = runner(ad);
            if (!promise || typeof promise.then !== 'function') {
                console.log('[HY] ad.' + apiName + '() 未返回 Promise，动作已执行，按成功回推');
                ok();
                return;
            }
            // 端上无填充时 load()/show() 的 Promise 可能长期挂起，这里只记日志不改变行为
            var settled = false;
            var watchdog = setTimeout(function () {
                if (!settled) {
                    console.warn('[HY] ad.' + apiName + '() 超过 15s 未返回，端上可能无广告填充或环境不支持该广告位');
                }
            }, 15000);
            promise.then(function (res) {
                settled = true;
                clearTimeout(watchdog);
                ok();
            }, function (e) {
                settled = true;
                clearTimeout(watchdog);
                fail(e);
            });
        } catch (e) {
            fail(e);
        }
    };
    if (ad && typeof ad.open === 'function') {
        settle('open', function (target) { return target.open(); });
    } else {
        settle('open', null);
    }
},

HYADDestroy: function (id) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var adId = _HYPointer_stringify_adaptor(id);
    if (!store[adId]) {
        return false;
    }
    var ad = store[adId];
    // 与参考工程一致：off 属于销毁阶段的事（先取消监听再 destroy），创建时不做任何 off。
    // 实例即将不存在，这里不传回调参数、直接移除该实例上的全部监听。
    ad.offLoad && ad.offLoad();
    ad.offError && ad.offError();
    ad.offClose && ad.offClose();
    ad.offHide && ad.offHide();
    ad.offResize && ad.offResize();
    ad.offReward && ad.offReward();
    ad.destroy && ad.destroy();
    delete store[adId];
    return true;
},

HYADIsShow: function (id) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    if (!ad || typeof ad.isShow !== 'function') {
        return false;
    }
    return !!ad.isShow();
},

HYADStyleChange: function (id, key, value) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    if (!ad || typeof ad.style === 'undefined' || !ad.style) {
        return;
    }
    var raw = _HYPointer_stringify_adaptor(value);
    var formatted;
    if (raw === 'true') {
        formatted = true;
    } else if (raw === 'false') {
        formatted = false;
    } else {
        formatted = parseFloat(raw);
    }
    ad.style[_HYPointer_stringify_adaptor(key)] = formatted;
},

HYADGetStyleValue: function (id, key) {
    var store = window.__HYADS__ || (window.__HYADS__ = {});
    var ad = store[_HYPointer_stringify_adaptor(id)];
    var res = '';
    if (ad && typeof ad.style !== 'undefined' && ad.style) {
        res = String(ad.style[_HYPointer_stringify_adaptor(key)]);
    }
    var bufferSize = lengthBytesUTF8(res) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(res, buffer, bufferSize);
    return buffer;
},
})