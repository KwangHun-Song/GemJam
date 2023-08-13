using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GemMatch;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

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

            var targetPool = collectionPool[statusView];
            var task = new UniTask[targetPool.Count];
            for (var i = 0; i < targetPool.Count; i++) {
                var clone = targetPool[i];
                if (clone.gameObject.activeSelf) continue;
                clone.SetActive(true);
                clone.transform.localScale = Vector3.one;
                task[i] = AnimateAsync(clone, clone.transform.position, statusView.CollectionRoot.position, targetPool);
            }
            UniTask.WhenAll(task).Forget();

            await UniTask.WaitUntil(() => targetPool.Count == 0);
            if (collectionPool.ContainsKey(statusView) == false) return;
            collectionPool.Remove(statusView);
        }

        private static readonly Dictionary<MissionStatusView, List<GameObject>> collectionPool =
            new Dictionary<MissionStatusView, List<GameObject>>();


        private float threshold = 1.5f;
        private float collectionDuration = 0.8f;
        private async UniTask AnimateAsync(GameObject collectingObject, Vector3 from, Vector3 to, List<GameObject> pool) {
            if (to != null) {
                // var wayPoints = new Vector3[] {
                //     to,
                //     from + Vector3.down * threshold + Vector3.left * threshold,
                //     from,
                // };
                var hash = collectingObject.gameObject.GetHashCode();
                var seq = DOTween.Sequence().SetId(hash);
                seq.Insert(0, collectingObject.transform
                    .DOMove(to, collectionDuration).SetEase(Ease.InBack, 2.5F));
                    // .DOPath(wayPoints, collectionDuration, PathType.CubicBezier)
                    //     .SetEase(Ease.InOutQuad));
                seq.Play();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(collectionDuration));

            SimpleSound.Play(SoundName.bbock);
            pool.Remove(collectingObject);
            DestroyImmediate(collectingObject);
        }
    }
}