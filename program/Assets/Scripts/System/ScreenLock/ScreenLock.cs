using UnityEngine.EventSystems;

namespace System.ScreenLock {
    public class ScreenLock : IDisposable {
        private static int lockCount;
        private static EventSystem eventSystem;

        public ScreenLock() => Lock();
        public void Dispose() => UnLock();

        private static void Lock() {
            lockCount++;
            if (!EventSystem.current) return;
            eventSystem = EventSystem.current;
            eventSystem.gameObject.SetActive(false);
        }

        private static void UnLock() {
            lockCount--;
            if (lockCount <= 0) {
                if (eventSystem) eventSystem.gameObject.SetActive(true);
            }
        }
    }
}