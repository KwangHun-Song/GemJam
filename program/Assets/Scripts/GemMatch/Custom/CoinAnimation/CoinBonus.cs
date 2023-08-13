using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class CoinBonus : MonoBehaviour {
        [SerializeField] private Animator animator;
        
        private static readonly int Start = Animator.StringToHash("start");
        private static readonly int Crash = Animator.StringToHash("crash");

        public void ShowStart() {
            SimpleSound.Play(SoundName.coin_appear);
            animator.SetTrigger(Start);
        }

        public async UniTask ShowCrashAsync() {
            animator.SetTrigger(Crash);
            await animator.WaitForCurrentClip();
        }
    }
}