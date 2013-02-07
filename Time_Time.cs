namespace MMSystem
{
	namespace Time
	{
		/// <summary>
		/// 現在時刻を取得する API 群。
		/// </summary>
		public static class Time
		{
			/// <summary>
			/// 現在時刻を表す数値を返す。timeGetTime() により取得できる値と同等である。
			/// </summary>
			public static uint Tick
			{
				get { return Win32.Api.timeGetTime(); }
			}
		}
	}
}
