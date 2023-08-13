using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GemMatch;
using UnityEngine;
using UnityEngine.Serialization;

namespace OverlayStatusSystem {
    public class MissionStatusViewHolder : MonoBehaviour {
        [SerializeField] private RectTransform missionRoot;
        [SerializeField] private GameObject missionPrefab;
        [FormerlySerializedAs("missions")]
        [SerializeField] private List<MissionStatusView> missionStatusViews;

        private void OnDisable() {
            ClearMissionViews();
        }

        public void InitializeMissions(Mission[] targetMissions) {
            if (missionStatusViews.Count > 0) {
                ClearMissionViews();
            }

            foreach (var m in targetMissions) {
                var missionView = Instantiate(missionPrefab, missionRoot).GetComponent<MissionStatusView>();
                missionView.name = $"Mission({m.entity.index},{m.entity.color})";
                missionStatusViews.Add(missionView);
                missionView.InitializeMission(m);
            }
        }

        private void ClearMissionViews() {
            foreach (MissionStatusView view in missionStatusViews) {
                Destroy(view.gameObject);
            }
            missionStatusViews.Clear();
        }

        public void CollectMissionByViewClones(EntityModel targetEntity, GameObject targetMold) {
            if (missionStatusViews.Count == 0) return;
            var statusView = missionStatusViews!.SingleOrDefault(m=>m.mission.entity.index == targetEntity.index && m.mission.entity.color == targetEntity.color);
            if (statusView != null) {
                if (collectionPool.ContainsKey(statusView) == false) {
                    collectionPool[statusView] = new List<GameObject>();
                }

                var cacheView = Instantiate(targetMold, statusView.CollectionRoot);
                cacheView.transform.position = targetMold.transform.position;
                cacheView.SetActive(false);
                collectionPool[statusView].Add(cacheView);
            }
        }

        public async UniTask AchieveMissionAsync(Mission targetMission, int changeCount) {
            if (missionStatusViews.Count == 0) return;
            var targetView = missionStatusViews.SingleOrDefault(m => m.mission.Equals(targetMission));
            if (targetView == null) return;

            await AnimateMissionPoolAsync(targetView);
            await targetView.GetMissionAsync(targetMission, changeCount);
        }

        private async UniTask AnimateMissionPoolAsync(MissionStatusView statusView) {
            if (statusView == null) return;
            while (collectionPool.ContainsKey(statusView) == false) {
                await UniTask.Yield();
            }

            var task = new UniTask[collectionPool[statusView].Count];
            for (var i = 0; i < collectionPool[statusView].Count; i++) {
                var clone = collectionPool[statusView][i];
                clone.SetActive(true);
                clone.transform.localScale = Vector3.one;
                task[i] = AnimateAsync(clone, clone.transform.position, statusView.CollectionRoot.position);
            }
            await UniTask.WhenAll(task);
            if (collectionPool.ContainsKey(statusView) == false) return;
            foreach (var clone in collectionPool[statusView]) {
                DestroyImmediate(clone);
            }

            collectionPool.Remove(statusView);
        }

        private static readonly Dictionary<MissionStatusView, List<GameObject>> collectionPool =
            new Dictionary<MissionStatusView, List<GameObject>>();


        private float threshold = 1.5f;
        private float collectionDuration = 1.1f;
        private async UniTask AnimateAsync(GameObject collectingObject, Vector3 from, Vector3 to) {
            if (to != null) {
                var wayPoints = new Vector3[] {
                    to,
                    from + Vector3.down * threshold + Vector3.left * threshold,
                    from,
                };
                var seq = DOTween.Sequence().SetId(collectingObject.GetHashCode());
                seq.Insert(0, collectingObject.transform
                    .DOPath(wayPoints, collectionDuration, PathType.CubicBezier)
                        .SetEase(Ease.InOutQuad));
                seq.Play();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(collectionDuration));
        }
    }
}