using System;
using UnityEngine.Scripting;

namespace HuyaWASM
{
	[Preserve]
	public interface ICallback<TSuccess, TFail, TComplete>
	{
		Action<TSuccess> success { get; set; }

		Action<TFail> fail { get; set; }

		Action<TComplete> complete { get; set; }
	}
}
