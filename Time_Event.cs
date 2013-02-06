using System;

namespace MMSystem
{
	namespace Time
	{
		public delegate void EventCallback(object user);

		/// <summary>
		/// マルチメディアタイマによるイベントスレッド
		/// </summary>
		public class Event : IDisposable
		{
			/// <summary>
			/// Event オブジェクトを生成する。
			/// </summary>
			/// <param name="callback">コールバックされるメソッドのデリゲート。</param>
			/// <param name="duration">callback が呼び出される間隔(ミリ秒単位)。</param>
			/// <param name="isOneShot">true にした場合、callback は duration だけ経過後 1 回のみ呼び出される。false にした場合、callback は duration ごとに繰り返し呼び出される。</param>
			/// <param name="user">callback に渡すユーザ定義のデータ。</param>
			public Event(EventCallback callback, uint duration, bool isOneShot, object user = null)
			{
				callback_ = callback;
				inner_callback_ = Callback;
				user_ = user;

				// タイマ起動
				var flag = isOneShot ? Win32.TIME_FLAGS.TIME_ONESHOT : Win32.TIME_FLAGS.TIME_PERIODIC;
				var ret = Win32.Api.timeSetEvent(duration, duration, inner_callback_, IntPtr.Zero, flag);

				// エラーチェック
				if ((uint)ret == 0)
					throw new MMSystemException("マルチメディアタイマの起動に失敗しました。");

				timer_id_ = (uint)ret;
			}

			/// <summary>
			/// デストラクタ。
			/// </summary>
			~Event()
			{
				Dispose();
			}

			/// <summary>
			/// Event オブジェクトを破棄する。
			/// </summary>
			public void Dispose()
			{
				// タイマの破棄
				Win32.Api.timeKillEvent(timer_id_);

				// GC がデストラクタを呼び出さないようにする
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// timeSetEvent() に指定するコールバック。
			/// </summary>
			/// <param name="uTimerID"></param>
			/// <param name="uMsg"></param>
			/// <param name="dwUser"></param>
			/// <param name="dw1"></param>
			/// <param name="dw2"></param>
			private void Callback(uint uTimerID, uint uMsg, IntPtr dwUser, IntPtr dw1, IntPtr dw2)
			{
				// 指定されたコールバックを呼び出す。
				callback_(user_);
			}

			private EventCallback callback_;
			private Win32.TimeProc inner_callback_;
			private object user_;
			private uint timer_id_;
		}
	}
}