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
			/// <summary>
			/// 一度に送出できる MIDI メッセージの最大バイト数。
			/// </summary>
			public const uint MAX_BYTES_PER_SENDING = 1024;

			/// <summary>
			/// コントローラの初期化を行う。
			/// コンストラクタからのみ呼び出される。
			/// </summary>
			/// <param name="deviceId">MIDI 出力デバイスの ID</param>
			private void Init(uint deviceId)
			{
				Win32.MMRESULT ret;

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
			/// <param name="deviceId">対象となる MIDI 出力デバイスの ID。</param>
			public Controller(uint deviceId)
			{
				Init(deviceId);
			}

			/// <summary>
			/// MIDI 出力デバイスのコントローラを生成する。
			/// </summary>
			/// <param name="deviceName">対象となる MIDI 出力デバイスの名前。DeviceCaps.Name の値を使用する。</param>
			public Controller(string deviceName)
			{
				// 名前の一致するデバイスを探す
				uint n = DeviceCaps.NumDevices;
				for (uint i = 0; i < n; ++i)
				{
					DeviceCaps caps = new DeviceCaps(i);
					if (caps.Name == deviceName)
					{
						// デバイスを発見したので初期化
						Init(i);
						return;
					}
				}

				// 名前の一致するデバイスがなかった
				throw new MMSystemException("指定された名前の MIDI 出力デバイスが見つかりませんでした。");
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
					Win32.Api.midiOutReset(hmo_);
					Win32.Api.midiOutClose(hmo_);
					hmo_ = IntPtr.Zero;
				}

				// GC がデストラクタを呼び出さないようにする
				GC.SuppressFinalize(this);
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
			/// <param name="message">メッセージのバイト列を格納した配列。MAX_BYTES_PER_SENDING 以下の長さである必要がある。</param>
			public void Send(byte[] message)
			{
				if (message.Length > MAX_BYTES_PER_SENDING)
					throw new ArgumentOutOfRangeException("message", "MIDI メッセージのサイズが MAX_BYTES_PER_SENDING を越えています。");

				int len = message.Length;

				// データのコピー
				Marshal.Copy(message, 0, buffer_, len);

				unsafe
				{
					// 準備
					((Win32.MIDIHDR*)hdr_)->dwBufferLength = (uint)len;
					((Win32.MIDIHDR*)hdr_)->dwFlags = 0;
					Win32.Api.midiOutPrepareHeader(hmo_, hdr_, cb_hdr_);

					// 送出
					Win32.Api.midiOutLongMsg(hmo_, hdr_, cb_hdr_);

					// 送出完了まで待機 (ブロッキング)
					while ((((Win32.MIDIHDR*)hdr_)->dwFlags & Win32.MIDIHDR_FLAGS.MHDR_DONE) == 0) ;

					// 後始末
					Win32.Api.midiOutUnprepareHeader(hmo_, hdr_, cb_hdr_);
				}
			}

			private IntPtr hmo_;
			private IntPtr buffer_;
			private IntPtr hdr_;
			private uint cb_hdr_;
		}
	}
}
