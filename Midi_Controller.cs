using System;
using System.Runtime.InteropServices;

namespace MMSystem
{
	namespace Midi
	{
		/// <summary>
		/// MIDI 出力デバイスのコントローラ。
		/// </summary>
		public class Controller : IDisposable
		{
			DeviceCaps caps_;

			/// <summary>
			/// 一度に送出できる MIDI メッセージの最大バイト数。
			/// </summary>
			public const int MAX_BYTES_PER_SENDING = 1024;

			/// <summary>
			/// MIDI 出力デバイスの情報。
			/// </summary>
			public DeviceCaps DeviceCaps
			{
				get { return caps_; }
			}

			/// <summary>
			/// MIDI 出力デバイスのコントローラを生成する。
			/// </summary>
			/// <param name="deviceId">対象となる MIDI 出力デバイスの ID。</param>
			public Controller(uint deviceId)
			{
				Win32.MMRESULT ret;

				caps_ = new DeviceCaps(deviceId);

				// MIDIHDR オブジェクトのサイズ
				cb_hdr_ = (uint)Marshal.SizeOf(typeof(Win32.MIDIHDR));

				try
				{
					unsafe
					{
						// HMIDIOUT オブジェクトの取得
						fixed (IntPtr* lphmo = &hmo_)
						{
							ret = Win32.Api.midiOutOpen((IntPtr)lphmo, deviceId, 0, 0, 0);
							if (ret != Win32.MMRESULT.MMSYSERR_NOERROR)
								throw new Win32Exception(ret);
						}

						// 送信バッファの作成
						buffer_ = Marshal.AllocHGlobal((int)MAX_BYTES_PER_SENDING);

						// MIDIHDR データの生成
						hdr_ = Marshal.AllocHGlobal((int)cb_hdr_);
						((Win32.MIDIHDR*)hdr_)->lpData = buffer_;
					}
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			/// <summary>
			/// MIDI 出力デバイスのコントローラを生成する。
			/// </summary>
			/// <param name="deviceName">対象となる MIDI 出力デバイスの名前。DeviceCaps.Name の値を使用できる。</param>
			/// <returns>
			/// deviceName で指定した名前のデバイスが見つかった場合は、そのデバイスを表す Controller オブジェクト。
			/// 見つからなかった場合は null。
			/// </returns>
			public static Controller FromName(string deviceName)
			{
				// 名前の一致するデバイスを探す
				uint n = DeviceCaps.NumDevices;
				for (uint i = 0; i < n; ++i)
				{
					DeviceCaps caps = new DeviceCaps(i);
					if (caps.Name == deviceName)
						// デバイスを発見したので初期化
						return new Controller(i);
				}

				// 名前の一致するデバイスがなかった
				return null;
			}

			/// <summary>
			/// デストラクタ。
			/// </summary>
			~Controller()
			{
				Dispose();
			}

			/// <summary>
			/// Controller オブジェクトを破棄する。
			/// </summary>
			public void Dispose()
			{
				// MIDIHDR オブジェクトの破棄
				if (hdr_ != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(hdr_);
					hdr_ = IntPtr.Zero;
				}

				// 送信バッファの破棄
				if (buffer_ != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(buffer_);
					buffer_ = IntPtr.Zero;
				}

				// HMIDIOUT オブジェクトの破棄
				if (hmo_ != IntPtr.Zero)
				{
					Reset();
					Win32.Api.midiOutClose(hmo_);
					hmo_ = IntPtr.Zero;
				}

				// GC がデストラクタを呼び出さないようにする
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// MIDI 出力デバイスをリセットする。
			/// </summary>
			public void Reset()
			{
				Win32.Api.midiOutReset(hmo_);
			}

			/// <summary>
			/// デバイスへ 1 ~ 3 バイトの MIDI メッセージを送出する。
			/// </summary>
			/// <param name="status">ステータスバイト。</param>
			/// <param name="data1">データバイト 1。</param>
			/// <param name="data2">データバイト 2。</param>
			public void Send(byte status, byte data1 = 0, byte data2 = 0)
			{
				Win32.Api.midiOutShortMsg(hmo_, (uint)status | (((uint)data1) << 8) | (((uint)data2) << 16));
			}

			/// <summary>
			/// デバイスへ指定したバイト数の MIDI メッセージを送出する。
			/// システム エクスクルーシブ メッセージの送出などに使用する。
			/// </summary>
			/// <param name="message">メッセージのバイト列を格納した配列。</param>
			public void Send(params byte[] message)
			{
				int len = message.Length;

				for (int offset = 0; offset < message.Length; offset += MAX_BYTES_PER_SENDING)
				{
					int sending = Math.Min(message.Length - offset, MAX_BYTES_PER_SENDING);

					// データのコピー
					Marshal.Copy(message, offset, buffer_, sending);

					unsafe
					{
						// 準備
						((Win32.MIDIHDR*)hdr_)->dwBufferLength = (uint)sending;
						((Win32.MIDIHDR*)hdr_)->dwFlags = 0;
						Win32.Api.midiOutPrepareHeader(hmo_, hdr_, cb_hdr_);

						// 送出
						var ret = Win32.Api.midiOutLongMsg(hmo_, hdr_, cb_hdr_);

						// 送出完了まで待機 (ブロッキング)
						if (ret == Win32.MMRESULT.MMSYSERR_NOERROR)
							while ((((Win32.MIDIHDR*)hdr_)->dwFlags & Win32.MIDIHDR_FLAGS.MHDR_DONE) == 0) ;

						// 後始末
						Win32.Api.midiOutUnprepareHeader(hmo_, hdr_, cb_hdr_);
					}
				}
			}

			private IntPtr hmo_;
			private IntPtr buffer_;
			private IntPtr hdr_;
			private uint cb_hdr_;
		}
	}
}
