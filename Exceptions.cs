using System;

namespace MidiSharp
{
	/// <summary>
	/// Midi 名前空間の例外の基底クラス。
	/// </summary>
	public class MidiException : Exception
	{
		/// <summary>
		/// メッセージを指定して MidiException オブジェクトを生成する。
		/// </summary>
		/// <param name="message">メッセージ。</param>
		public MidiException(string message)
			: base("MIDI: " + message)
		{
		}
	}

	/// <summary>
	/// Win32 API のエラーによる例外。
	/// </summary>
	public class Win32Exception : MidiException
	{
		/// <summary>
		/// メッセージを指定して Win32Exception オブジェクトを生成する。
		/// </summary>
		/// <param name="message">メッセージ。</param>
		public Win32Exception(string message)
			: base(message)
		{
		}

		/// <summary>
		/// MMRESULT 値から Win32Exception オブジェクトを生成する。
		/// </summary>
		/// <param name="result">MMRESULT 値。</param>
		public Win32Exception(Win32.MMRESULT result)
			: base(result.ToString())
		{
		}
	}
}
