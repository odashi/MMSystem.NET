namespace MMSystem
{
	namespace Time
	{
		/// <summary>
		/// マルチメディアタイマの分解能を管理するクラス。
		/// </summary>
		public static class Period
		{
			/// <summary>
			/// マルチメディアタイマの分解能を設定する。
			/// </summary>
			/// <param name="period">マルチメディアタイマに設定する分解能(ミリ秒単位)。</param>
			public static void Begin(uint period)
			{
				// 既に設定されていないかどうか
				if (begun_)
					throw new MMSystemException("マルチメディアタイマの分解能が既に設定されています。");

				// 設定
				var ret = Win32.Api.timeBeginPeriod(period);

				// エラーチェック
				if (ret != Win32.MMRESULT.MMSYSERR_NOERROR)
					throw new Win32Exception(ret);

				begun_ = true;
				period_ = period;
			}

			/// <summary>
			/// マルチメディアタイマの分解能の設定を解除する。
			/// </summary>
			public static void End()
			{
				if (begun_)
				{
					Win32.Api.timeEndPeriod(period_);
					begun_ = false;
					period_ = 0;
				}
			}

			private static bool begun_ = false;
			private static uint period_ = 0;
		}
	}
}
