using GemMatch.UndoSystem;

namespace GemMatch {
    public class MissionCommand : Command<Mission> {
        public MissionCommand(Controller controller, Mission mission, int count, bool triggeredByPrev = false) : base(
            @do: () => {
                mission.count -= count;
                foreach (var listener in controller.Listeners) listener.OnChangeMission(mission, mission.count, false);
            },
            undo: addedMission => {
                addedMission.count += count;
                foreach (var listener in controller.Listeners) listener.OnChangeMission(addedMission, addedMission.count, true);
            },
            param: mission,
            triggeredByPrev: triggeredByPrev) {
        }
    }
}