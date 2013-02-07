namespace MMSystem
{
	namespace Time
	{
		/// <summary>
		/// 現在時刻を取得する API 群。
		/// </summary>
		public static class Time
		{
			public static uint Tick
			{
				get { return Win32.Api.timeGetTime(); }
			}
		}
	}
}
