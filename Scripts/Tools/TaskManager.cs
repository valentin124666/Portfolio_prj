using System;
using System.Text;
using Cysharp.Threading.Tasks;

namespace Tools
{
    public static class TaskManager {
        private static event Action tick;
        private static bool ticking;

        public static event Action Tick {
            add {
                tick -= value;
                tick += value;
                value.Invoke();
                OnTick().Forget();
            }
            remove => tick -= value;
        }

        private static async UniTaskVoid OnTick() {
            if (ticking) return;

            ticking = true;

            while (tick != null) {
                tick?.Invoke();
                await UniTask.Delay(1000, ignoreTimeScale: false);
            }

            ticking = false;
        }
        
        public static string FormatTime(uint seconds, bool alwaysShowHours = true, bool alwaysShowMinutes = true, bool isStatic = false) {
            if (!isStatic && seconds > 0) {
                //to start with 59
                seconds--;
            }

            var timeSpan = TimeSpan.FromSeconds(seconds);
            var timeString = new StringBuilder();

            if (timeSpan.Hours > 0 || alwaysShowHours) {
                timeString.Append($"{Math.Floor(timeSpan.TotalHours):00}:");
            }

            if (timeSpan.Minutes > 0 || alwaysShowMinutes) {
                timeString.Append($"{timeSpan.Minutes:00}:");
            }

            timeString.Append($"{timeSpan.Seconds:00}");

            return timeString.ToString();
        }

        public static (bool, string) FormatRewardLifeTime(uint seconds, bool isStatic = true) {
            if (!isStatic && seconds > 0) {
                //to start with 59
                seconds--;
            }

            var timeSpan = TimeSpan.FromSeconds(seconds);
            var timeString = new StringBuilder();

            if (timeSpan.TotalHours > 1) {
                timeString.Append($"{Math.Floor(timeSpan.TotalHours):00}:");
                timeString.Append($"{timeSpan.Minutes:00}:");
                timeString.Append($"{timeSpan.Seconds:00}");
                return (true, timeString.ToString());
            }

            timeString.Append($"{Math.Floor(timeSpan.TotalMinutes):00}:");
            timeString.Append($"{timeSpan.Seconds:00}");
            return (false, timeString.ToString());
        }
        
        public static async void ExecuteInNextFrame(Action action) {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            action?.Invoke();
        }

        public static async void PostLateUpdate(Action action) {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            action?.Invoke();
        }

        public static async void ExecuteAfterDelay(float seconds, Action action) {
            await WaitUntilDelay(seconds);
            action?.Invoke();
        }

        public static async void ExecuteWhen(Func<bool> predicate, Action action) {
            await WaitUntil(predicate);
            action?.Invoke();
        }

        public static async UniTask WaitUntilNextFrame() {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        }

        public static async UniTask WaitUntilDelay(float seconds) {
            await UniTask.Delay((int) (seconds * 1000), ignoreTimeScale: false);
        }

        public static async UniTask WaitUntil(Func<bool> predicate) {
            await UniTask.WaitUntil(predicate);
        }

        public static async UniTask WaitUntil(UniTask task) {
            await UniTask.WhenAll(task);
        }

        public static async UniTask WaitAny(params UniTask[] tasks) {
            await UniTask.WhenAny(tasks);
        }

        public static async UniTask WaitAll(params UniTask[] tasks) {
            await UniTask.WhenAll(tasks);
        }

        public static void Destroy() {
            tick = null;
        }
    }
}
