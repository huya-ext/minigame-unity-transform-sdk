using System;
using UnityEngine.Scripting;

namespace HuyaWASM
{
	[Preserve]
	public class RequestFailCallbackErr
	{
		/// <summary>
		/// 错误信息
		/// </summary>
		public string errMsg;

		/// <summary>
		/// errno 错误码，错误码的详细说明参考
		/// </summary>
		public double errno;
	}
}
