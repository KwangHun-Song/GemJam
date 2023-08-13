using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class InvisibleCoverView : EntityView {
        public override async UniTask DestroyAsync(bool isImmediately = false) {
            if (isImmediately) {
                DestroyImmediate(gameObject);
                return;
            }
            
            var effectName = $"CrashInvisibleCover";
            var crashEffect = Instantiate(Resources.Load<GameObject>(effectName), transform.parent);
            crashEffect.transform.position = transform.position;
            SimpleSound.Play(SoundName.woodbox_crash);
            DestroyEffectAsync().Forget();
            
            // 엔티티는 먼저 터뜨리고 이펙트만 남아서 보여주도록 적용.
            DestroyImmediate(gameObject);

            async UniTask DestroyEffectAsync() {
                await UniTask.Delay(2000);
                DestroyImmediate(crashEffect.gameObject);
            }
        }
    }
}