using System;
using System.Runtime.InteropServices;

namespace MMSystem
{
	namespace Win32
	{
		/// <summary>
		/// Win32 MMRESULT 値。
		/// </summary>
		internal enum MMRESULT : uint
		{
			MMSYSERR_NOERROR = 0,
			MMSYSERR_ERROR = 1,
			MMSYSERR_BADDEVICEID = 2,
			MMSYSERR_NOTENABLED = 3,
			MMSYSERR_ALLOCATED = 4,
			MMSYSERR_INVALHANDLE = 5,
			MMSYSERR_NODRIVER = 6,
			MMSYSERR_NOMEM = 7,
			MMSYSERR_NOTSUPPORTED = 8,
			MMSYSERR_BADERRNUM = 9,
			MMSYSERR_INVALFLAG = 10,
			MMSYSERR_INVALPARAM = 11,
			MMSYSERR_HANDLEBUSY = 12,
			MMSYSERR_INVALIDALIAS = 13,
			MMSYSERR_BADDB = 14,
			MMSYSERR_KEYNOTFOUND = 15,
			MMSYSERR_READERROR = 16,
			MMSYSERR_WRITEERROR = 17,
			MMSYSERR_DELETEERROR = 18,
			MMSYSERR_VALNOTFOUND = 19,
			MMSYSERR_NODRIVERCB = 20,
			MMSYSERR_MOREDATA = 21,

			MIDIERR_UNPREPARED = 64,
			MIDIERR_STILLPLAYING = 65,
			MIDIERR_NOMAP = 66,
			MIDIERR_NOTREADY = 67,
			MIDIERR_NODEVICE = 68,
			MIDIERR_INVALIDSETUP = 69,
			MIDIERR_BADOPENMODE = 70,
			MIDIERR_DONT_CONTINUE = 71,

			TIMEERR_NOCANDO = 97,
			TIMEERR_STRUCT = 129,
		}

		/// <summary>
		/// MIDIHDR.dwFlags のフラグ。
		/// </summary>
		[Flags]
		internal enum MIDIHDR_FLAGS : uint
		{
			MHDR_DONE = 1,
			MHDR_PREPARED = 2,
			MHDR_INQUEUE = 4,
			MHDR_ISSTRM = 8,
		}

		/// <summary>
		/// Win32 MIDIOOUTCAPS 構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal unsafe struct MIDIOUTCAPS
		{
			internal const uint MAXPNAMELEN = 32;
			internal ushort wMid;
			internal ushort wPid;
			internal uint vDriverVersion;
			internal fixed byte szPname[(int)MAXPNAMELEN];
			internal ushort wTechnology;
			internal ushort wVoices;
			internal ushort wNotes;
			internal ushort wChannelMask;
			internal uint dwSupport;
		}

		/// <summary>
		/// Win32 MIDIHDR 構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal unsafe struct MIDIHDR
		{
			internal IntPtr lpData;
			internal uint dwBufferLength;
			internal uint dwBytesRecorded;
			internal IntPtr dwUser;
			internal MIDIHDR_FLAGS dwFlags;
			internal IntPtr lpNext;
			internal IntPtr reserved;
			internal uint dwOffset;
			// 本来 dwReserved[8] だが、固定長バッファは IntPtr では作成できない。
			// また IntPtr[] 型は fixed で使用できない。
			// 仕方ないのでこうする。
			internal IntPtr dwReserved1;
			internal IntPtr dwReserved2;
			internal IntPtr dwReserved3;
			internal IntPtr dwReserved4;
			internal IntPtr dwReserved5;
			internal IntPtr dwReserved6;
			internal IntPtr dwReserved7;
			internal IntPtr dwReserved8;
		}

		/// <summary>
		/// マルチメディアタイマのコールバック関数。
		/// </summary>
		/// <param name="uTimerID">timeSetEvent() で設定されるタイマ ID。</param>
		/// <param name="uMsg">予約済み。</param>
		/// <param name="dwUser">timeSetEvent() で設定されるユーザ定義の値。</param>
		/// <param name="dw1">予約済み。</param>
		/// <param name="dw2">予約済み。</param>
		internal delegate void TimeProc(uint uTimerID, uint uMsg, IntPtr dwUser, IntPtr dw1, IntPtr dw2);

		[Flags]
		internal enum TIME_FLAGS : uint
		{
			TIME_ONESHOT = 0x0,
			TIME_PERIODIC = 0x1,
			TIME_CALLBACK_FUNCTION = 0x0,
			TIME_CALLBACK_EVENT_SET = 0x10,
			TIME_CALLBACK_EVENT_PULSE = 0x20,
		}

		/// <summary>
		/// Win32 API アクセサ。
		/// </summary>
		internal static class Api
		{
			[DllImport("winmm.dll")]
			extern internal static MMRESULT timeBeginPeriod(uint uPeriod);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT timeEndPeriod(uint uPeriod);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT timeSetEvent(uint uDelay, uint uResolution, TimeProc lpTimeProc, IntPtr dwUser, TIME_FLAGS fuEvent);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT timeKillEvent(uint uTimerID);
			[DllImport("winmm.dll")]
			extern internal static uint timeGetTime();
			[DllImport("winmm.dll")]
			extern internal static uint midiOutGetNumDevs();
			[DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
			extern internal static MMRESULT midiOutGetDevCaps(uint uDeviceID, IntPtr lpMidiOutCaps, uint cbMidiOutCaps);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutOpen(IntPtr lphmo, uint uDeviceID, uint dwCallback, uint dwCallbackInstance, uint dwFlags);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutClose(IntPtr hmo);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutReset(IntPtr hmo);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutPrepareHeader(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutUnprepareHeader(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutShortMsg(IntPtr hmo, uint dwMsg);
			[DllImport("winmm.dll")]
			extern internal static MMRESULT midiOutLongMsg(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
		}
	}
}
