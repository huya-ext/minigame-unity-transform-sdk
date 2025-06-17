using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuyaWASM
{
    class HYCallBackHandler
    {

        //用于暂存回调
        private static readonly Hashtable responseHT = new Hashtable();


        //用于作为回调id的一部分
        private static int htCounter = 0;


        private static int GenarateCallbackId()
        {
            if (htCounter > 1000000)
            {
                htCounter = 0;
            }
            htCounter++;

            return htCounter;
        }


        public static bool NeedCheckAndClear()
        {
            if (responseHT.Count == 1)
            {
                return true;
            }
            return false;
        }


        public static string Add<T>(Action<T> callback) where T : HYBaseResponse
        {
            if(callback == null)
            {
                return "";
            }
            int id = GenarateCallbackId();
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds);
            var key = timestamp.ToString() + '-' + id;
            responseHT.Add(key,callback);
            if (NeedCheckAndClear())
            {
                HYSDKManagerHandler.Instance.StartCoroutine(CheckAndCLear());
            }
            return key;
        }


        public static void InvokeResponseCallback<T>(string str) where T : HYBaseResponse
        {
            if (str != null)
            {
                T res = JsonUtility.FromJson<T>(str);

                var id = res.callbackId;

                if (responseHT.ContainsKey(id))
                {
                    var callback = (Action<T>)responseHT[id];
                    callback(res);
                    responseHT.Remove(id);
                }
            }
        }

        static IEnumerator CheckAndCLear()
        { 
            yield return new WaitForSeconds(8.0f); // 清除超过8秒的回调

            if (responseHT.Count > 0)
            {
                ICollection key = responseHT.Keys;
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long now = Convert.ToInt64(ts.TotalSeconds);
                List<string> tempItems = new List<string>();
                foreach (string k in key)
                {
                    long.TryParse(k.Split('-')[0], out long time);
                    if (now - time > 8)
                    {
                        tempItems.Add(k);
                    } 
                }
                foreach (string tempItem in tempItems)
                {
                    responseHT.Remove(tempItem);
                }
            }

            if (responseHT.Count > 0)
            {
                HYSDKManagerHandler.Instance.StartCoroutine(CheckAndCLear());
            }
        }
    }
}