using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 클릭 가능한 노멀 피스만 찾아서 픽하는 AI
    /// </summary>
    public class PickRandomAvailableNormalPieceAI : ISolverAI {
        public int GetIndexToInput(Controller controller) {
            return controller.ActiveTiles.Where(t => controller.CanTouch(t) && t.Piece is NormalPiece).PickRandom().Index;
        }
    }
}