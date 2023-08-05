using System.Linq;

namespace GemMatch {
    public class PickRandomAvailableAI : ISolverAI {
        public int GetIndexToInput(Controller controller) {
            return controller.ActiveTiles.Where(t => t.Piece is NormalPiece).PickRandom().Index;
        }
    }
}