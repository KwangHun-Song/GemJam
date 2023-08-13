using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class CoinBonus : MonoBehaviour {
        [SerializeField] private Animator animator;
        
        private static readonly int Start = Animator.StringToHash("start");
        private static readonly int Crash = Animator.StringToHash("crash");

        private static bool PlayNextSound { get; set; } = true;

        public void ShowStart() {
            // 사운드는 플립 형식으로 두 번 중 한 번만 적용한다.
            if (PlayNextSound) {
                SimpleSound.Play(SoundName.coin_appear);
            }
            PlayNextSound = !PlayNextSound;
            
            animator.SetTrigger(Start);
        }

        public async UniTask ShowCrashAsync() {
            animator.SetTrigger(Crash);
            await animator.WaitForCurrentClip();
        }
    }
}