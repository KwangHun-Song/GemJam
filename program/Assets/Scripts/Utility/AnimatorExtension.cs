using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility {
    public static class AnimatorExtension {
        /// <summary>
        /// 현재 재생중인 클립이 끝날 때까지 대기합니다.
        /// </summary>
        public static async UniTask WaitForCurrentClip(this Animator animator, int layerIndex = 0, CancellationToken cancellationToken = default) {
            // 트리거 활성화를 위해 대기함.
            await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
        
            AnimatorClipInfo? currentClipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex)
                .FirstOrDefault(clipInfo => clipInfo.clip != null);

            if (currentClipInfo == null) return;
            var currentClip = currentClipInfo.Value.clip;
            await UniTask.Delay((int)(currentClip.length * 1000), cancellationToken: cancellationToken);
        }

    }
}