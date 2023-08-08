using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 컬러 결정용 컨트롤러이므로 노멀피스만 픽하는 기능을 가졌다.
    /// </summary>
    public class ColorSelectingController : SimulationController {
        /// <summary>
        /// 모든 노멀피스의 색깔을 픽해야 하므로 클리어 조건을 미션이 아니라 모든 노멀피스를 픽한 것으로 바꾼다.
        /// </summary>
        protected override bool IsCleared() {
            if (Tiles.SelectMany(t => t.Entities.Values).Any(e => e is NormalPiece) == false && Memory.Any() == false) return true;
            return false;
        }
    }
}