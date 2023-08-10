using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GemMatch {
    public class RocketAbilityView : IAbilityView {
        public async UniTask RunAbilityAsync(View view, IAbility ability, Controller controller) {
            
            Debug.Log(nameof(RunAbilityAsync));
            
            var targetEntities = ((RocketAbility)ability).TilesToHit.Select(t => t.Piece);
            var entityViews = view.TileViews
                .SelectMany(tv => tv.EntityViews.Values);
            var targetEntityViews = targetEntities.Select(e => entityViews.Single(ev => ReferenceEquals(ev.Entity, e)));

            foreach (var entityView in targetEntityViews) {
                entityView.transform.DOShakeRotation(15F, Vector3.forward * 15);
            }

            await UniTask.Delay(1500);
        }

        public UniTask RestoreAbilityAsync(View view, IAbility ability, Controller controller) {
            // 레디부스터는 언두하지 않는다.
            return UniTask.CompletedTask;
        }
    }
}