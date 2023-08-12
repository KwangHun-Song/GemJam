using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GemMatch;
using UnityEngine;

namespace OverlayStatusSystem {
    public static class OverlayStatusHelper {
        private static readonly OverlayStatusManager manager = new OverlayStatusManager();
        public static void Init(IOverlayStatus statusEvent) {
            manager.Init(statusEvent);
        }

        public static void Input(IOverlayStatusEvent keyObject, OverlayStatusParam inputParam) {
            manager.Input(keyObject, inputParam);
        }

        public static void Save(IOverlayStatusEvent keyObject) {
            manager.Save(keyObject);
        }

        public static async UniTaskVoid InitializeMissionsAsync(Mission[] targetMissions) {
            await UniTask.WaitUntil(() => OverlayStatusHolder.Instance != null);
            OverlayStatusHolder.Instance.InitializeMission(targetMissions);
        }

        public static void CollectMissionByViewClone(EntityModel entityModel, GameObject entityViewGameObject) {
            OverlayStatusHolder.Instance.CollectMissionByViewClone(entityModel, entityViewGameObject);
        }

        public static void UpdateMissionCount(Mission mission, int changeCount) {
            OverlayStatusHolder.Instance.UpdateMissionCount(mission, changeCount);
        }

        public static void UpdateLevelStatus(Level currentLevel) {
            OverlayStatusHolder.Instance.UpdateLevelStatus(currentLevel);
        }

        public static void CollectCoin(int amount) {
            OverlayStatusHolder.Instance.CollectCoin(amount);
        }

        public static void UpdateCoinStatus() {
            OverlayStatusHolder.Instance.UpdateCoinByPlayerInfo();
        }

        public static Transform GetCoinStatusRoot() {
            return OverlayStatusHolder.Instance.GetCoinStatusRoot();
        }
    }
}