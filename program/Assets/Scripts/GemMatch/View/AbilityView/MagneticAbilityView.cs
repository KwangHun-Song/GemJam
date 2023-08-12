using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OverlayStatusSystem;
using UnityEngine;

namespace GemMatch {
    public class MagneticAbilityView : MonoBehaviour, IAbilityView {
        public async UniTask RunAbilityAsync(View view, IAbility ability, Controller controller) {
            MagneticAbility  magnetAbi = (MagneticAbility)ability;
            var normalPieceViews = view.TileViews
                .Where(tv=>magnetAbi.TilesToHit.Contains(tv.Tile))
                .SelectMany(tv => tv.EntityViews.Values)
                .Where(ev => ev.Entity.Index == EntityIndex.NormalPiece)
                .ToArray();

            Mission targetMission = null;
            foreach (var targetView in normalPieceViews) {
                OverlayStatusHelper.CollectMissionByViewClone(targetView.Entity.Model, targetView.gameObject);
            }
        }

        public UniTask RestoreAbilityAsync(View view, IAbility ability, Controller controller) {
            return UniTask.CompletedTask;
        }
    }
}