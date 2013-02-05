using System;

namespace MidiSharp
{
	/// <summary>
	/// MIDI メッセージ送出のためのヘルパクラス。
	/// </summary>
	public static class Helper
	{
		/// <summary>
		/// ノート オフ メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="note">ノート。</param>
		/// <param name="velocity">ベロシティ。</param>
		public static void NoteOff(this Controller controller, byte channel, byte note, byte velocity)
		{
			controller.Send((byte)(0x80 | (channel & 0x0f)), note, velocity);
		}

		/// <summary>
		/// ノート オン メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="note">ノート。</param>
		/// <param name="velocity">ベロシティ。</param>
		public static void NoteOn(this Controller controller, byte channel, byte note, byte velocity)
		{
			controller.Send((byte)(0x90 | (channel & 0x0f)), note, velocity);
		}

		/// <summary>
		/// ポリフォニック キー プレッシャー メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。/param>
		/// <param name="note">ノート。</param>
		/// <param name="pressure">設定するプレッシャー。</param>
		public static void PolyphonicKeyPressure(this Controller controller, byte channel, byte note, byte pressure)
		{
			controller.Send((byte)(0xa0 | (channel & 0x0f)), note, pressure);
		}

		/// <summary>
		/// コントロール チェンジ メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="number">コントロールチェンジ番号。</param>
		/// <param name="value">設定する値。</param>
		public static void ControlChange(this Controller controller, byte channel, byte number, byte value)
		{
			controller.Send((byte)(0xb0 | (channel & 0x0f)), number, value);
		}

		/// <summary>
		/// プログラム チェンジ メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="program">設定するプログラム。</param>
		public static void ProgramChange(this Controller controller, byte channel, byte program)
		{
			controller.Send((byte)(0xc0 | (channel & 0x0f)), program);
		}

		/// <summary>
		/// チャンネル プレッシャー メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="pressure">設定するプレッシャー。</param>
		public static void ChannelPressure(this Controller controller, byte channel, byte pressure)
		{
			controller.Send((byte)(0xd0 | (channel & 0x0f)), pressure);
		}

		/// <summary>
		/// ピッチ ベンド メッセージを送出する。
		/// </summary>
		/// <param name="controller">対象のコントローラ。</param>
		/// <param name="channel">チャンネル。</param>
		/// <param name="pitch">設定するピッチ。-8192 から 8191 の範囲で設定する。</param>
		public static void PitchBend(this Controller controller, byte channel, short pitch)
		{
			pitch += 8192;
			pitch &= 0x3fff;
			controller.Send((byte)(0xe0 | (channel & 0x0f)), (byte)(pitch & 0x7f), (byte)(pitch >> 7));
		}
	}
}
