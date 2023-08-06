using System.Linq;

namespace GemMatch {
    public class PickRandomAvailableAI : ISolverAI {
        public int GetIndexToInput(Controller controller) {
            return controller.ActiveTiles.Where(controller.CanTouch).PickRandom().Index;
        }
    }
}