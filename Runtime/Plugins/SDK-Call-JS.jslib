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
})