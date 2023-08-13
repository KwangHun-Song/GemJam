using System.Collections.Generic;

namespace GemMatch {
    public class DiscountMissionAbility : IAbility {
        public Controller Controller { get; private set; }
        public Mission TargetMission { get; private set; }
        public DiscountMissionAbility(Controller controller, Mission targetMission) {
            Controller = controller;
            TargetMission = targetMission;
        }

        public AbilityIndex Index => AbilityIndex.DiscountMission;
        public void Run() {
            if (Controller.Missions.Length == 0) return;
            for (var i = 0; i < Controller.Missions.Length; i++) {
                var mission = Controller.Missions[i];
                if (mission.entity == null) continue;
                if (mission.entity.index != TargetMission.entity.index) continue;
                if (mission.entity.color != ColorIndex.All && 
                    mission.entity.color != TargetMission.entity.color) continue;
                if (mission.entity.layer != TargetMission.entity.layer) continue;
                Controller.Missions[i].count -= TargetMission.count;
            }
        }

        public void Undo() {
            if (Controller.Missions.Length == 0) return;
            for (var i = 0; i < Controller.Missions.Length; i++) {
                var mission = Controller.Missions[i];
                if (mission.entity == null) continue;
                if (mission.entity.index != TargetMission.entity.index) continue;
                if (mission.entity.color != ColorIndex.All && 
                    mission.entity.color != TargetMission.entity.color) continue;
                if (mission.entity.layer != TargetMission.entity.layer) continue;
                Controller.Missions[i].count += TargetMission.count;
            }
        }

        public IEnumerable<IAbility> GetCascadedAbility() {
            yield break;
        }
    }
}