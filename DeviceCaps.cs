using System;

namespace MidiSharp
{
	/// <summary>
	/// MIDI 出力デバイスの情報。
	/// </summary>
	public class DeviceCaps
	{
		/// <summary>
		/// 現在有効な MIDI 出力デバイスの数。
		/// </summary>
		public static uint NumDevices
		{
			get { return Win32.Api.midiOutGetNumDevs(); }
		}

		/// <summary>
		/// MIDI 出力デバイスの情報を取得する。
		/// </summary>
		/// <param name="deviceId">対象となる MIDI 出力デバイスの ID。</param>
		public DeviceCaps(uint deviceId)
		{
			caps_ = new Win32.MIDIOUTCAPS();

			unsafe
			{
				fixed (Win32.MIDIOUTCAPS* ptr = &caps_)
				{
					// 情報を取得
					Win32.MMRESULT ret = Win32.Api.midiOutGetDevCaps(deviceId, (IntPtr)ptr, (uint)sizeof(Win32.MIDIOUTCAPS));

					// エラーチェック
					if (ret != Win32.MMRESULT.MMSYSERR_NOERROR)
						throw new Win32Exception(ret);

					// デバイス名の string オブジェクトの生成
					name_ = new string((sbyte*)ptr->szPname);
				}
			}
		}

		/// <summary>
		/// MIDI 出力デバイスの名前。
		/// </summary>
		public string Name
		{
			get { return name_; }
		}

		private Win32.MIDIOUTCAPS caps_;
		private string name_;
	}
}
