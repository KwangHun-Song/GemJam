using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class GoalPieceView : EntityView, IReceiveActivation {
        [SerializeField] private Animator animator;
        
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Crash = Animator.StringToHash("crash");
        
        private bool IsAlreadyActive { get; set; }

        public override UniTask OnCreate() {
            IsAlreadyActive = false;
            animator.SetTrigger(Idle);
            return UniTask.CompletedTask;
        }

        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }

        public async UniTask OnActive(bool isActive) {
            if (isActive && !IsAlreadyActive) {
                IsAlreadyActive = true; // 동일한 연출을 반복적으로 보여주지 않는다.
                SimpleSound.Play(SoundName.chest_open);
                animator.SetTrigger(Crash);
            }
        }
    }
}