using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GemMatch {
    public class ShuffleAbilityView : MonoBehaviour, IAbilityView {
        [SerializeField] private Transform shuffleTfm;
        public async UniTask RunAbilityAsync(View view, Ability ability, Controller controller) {
            var normalPieceViews = view.TileViews
                .SelectMany(tv => tv.EntityViews.Values)
                .Select(ev => ev as NormalPieceView)
                .Where(ev => ev != null)
                .ToArray();
            
            // 이미 Ability를 부른 후 불렸으므로 새로운 색깔들이 지정되었을 것이다. 이것을 저장해준다.
            var decidedColors = normalPieceViews.Select(ev => ev.Entity.Color).ToArray();

            shuffleTfm.gameObject.SetActive(true);
            shuffleTfm.localScale = Vector3.zero;
            shuffleTfm.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack);
            
            // 다섯 번정도 색깔을 랜덤으로 바꾸어준다.
            const int ShuffleCount = 10;
            for (int i = 0; i < ShuffleCount; i++) {
                var randomColors = decidedColors.Shuffle().ToArray();
                var updateTasks = new List<UniTask>();
                for (int ci = 0; ci < normalPieceViews.Length; ci++) {
                    normalPieceViews[ci].Entity.Color = randomColors[ci];
                    updateTasks.Add(normalPieceViews[ci].OnUpdate());
                }

                await UniTask.WhenAll(updateTasks);
                await UniTask.Delay(1000 / ShuffleCount);
            }
            
            await shuffleTfm.DOScale(Vector3.zero, 0.2F).SetEase(Ease.InBack).ToUniTask();
            shuffleTfm.gameObject.SetActive(false);
            
            // 마지막에는 원래 결정되었던 색깔로 교체해준다.
            for (int ci = 0; ci < normalPieceViews.Length; ci++) {
                normalPieceViews[ci].Entity.Color = decidedColors[ci];
                normalPieceViews[ci].OnUpdate().Forget();
            }
        }
    }
}