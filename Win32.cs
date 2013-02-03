using System;
using System.Runtime.InteropServices;

namespace MidiSharp
{
	/// <summary>
	/// Win32 API シンボル。
	/// </summary>
	namespace Win32
	{
		/// <summary>
		/// Win32 MMRESULT 値。
		/// </summary>
		public enum MMRESULT : uint
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
		}

		/// <summary>
		/// MIDIHDR.dwFlags のフラグ。
		/// </summary>
		[Flags]
		public enum MIDIHDR_FLAGS : uint
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
		public unsafe struct MIDIOUTCAPS
		{
			public const uint MAXPNAMELEN = 32;
			public ushort wMid;
			public ushort wPid;
			public uint vDriverVersion;
			public fixed byte szPname[(int)MAXPNAMELEN];
			public ushort wTechnology;
			public ushort wVoices;
			public ushort wNotes;
			public ushort wChannelMask;
			public uint dwSupport;
		}

		/// <summary>
		/// Win32 MIDIHDR 構造体。
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public unsafe struct MIDIHDR
		{
			public IntPtr lpData;
			public uint dwBufferLength;
			public uint dwBytesRecorded;
			public IntPtr dwUser;
			public MIDIHDR_FLAGS dwFlags;
			public IntPtr lpNext;
			public IntPtr reserved;
			public uint dwOffset;
			// 本来 dwReserved[8] だが、固定長バッファは IntPtr では作成できない。
			// また IntPtr[] 型は fixed で使用できない。
			// 仕方ないのでこうする。
			public IntPtr dwReserved1;
			public IntPtr dwReserved2;
			public IntPtr dwReserved3;
			public IntPtr dwReserved4;
			public IntPtr dwReserved5;
			public IntPtr dwReserved6;
			public IntPtr dwReserved7;
			public IntPtr dwReserved8;
		}

		/// <summary>
		/// Win32 API アクセサ。
		/// </summary>
		public class Api
		{
			[DllImport("winmm.dll")]
			extern public static uint midiOutGetNumDevs();
			[DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
			extern public static MMRESULT midiOutGetDevCaps(uint uDeviceID, IntPtr lpMidiOutCaps, uint cbMidiOutCaps);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutOpen(IntPtr lphmo, uint uDeviceID, uint dwCallback, uint dwCallbackInstance, uint dwFlags);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutClose(IntPtr hmo);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutReset(IntPtr hmo);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutPrepareHeader(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutUnprepareHeader(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutShortMsg(IntPtr hmo, uint dwMsg);
			[DllImport("winmm.dll")]
			extern public static MMRESULT midiOutLongMsg(IntPtr hmo, IntPtr lpMidiOutHdr, uint cbMidiOutHdr);
		}
	}
}
