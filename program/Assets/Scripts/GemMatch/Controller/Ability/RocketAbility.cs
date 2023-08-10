using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 이 어빌리티는 직접 기능하는 것은 없고 타겟 엔티티들을 찾아 캐스케이드 어빌리티들만 만들어서 반환한다.
    /// </summary>
    public class RocketAbility : Ability<bool> {
        public RocketAbility(Controller controller) : base(null, controller) { }
        public override AbilityIndex Index => AbilityIndex.RocketAbility;
        
        public IEnumerable<Tile> TilesToHit { get; private set; }

        public override void Run() {
            // 기본 제약 조건
            var tiles = Controller.Tiles.Where(tile => {
                // 노멀 피스를 가지고 있어야 한다.
                if (tile.Piece is not NormalPiece normalPiece) return false;
                // 사용 가능한 색깔이어야 한다.
                if (!Constants.UsableColors.Contains(normalPiece.Color)) return false;
                // InvisibleCover로 가려져 있으면 안 된다.
                if (tile.Entities.TryGetValue(Layer.Cover, out var cover) && cover is InvisibleCover) return false;
    
                return true;
            }).ToArray();

            var sortedTiles = tiles
                // 미션에 같은 색깔을 가진 노멀피스가 "없는지" 여부로 우선 소팅 >> 마그네틱과 다르니 확인하기!
                .Shuffle()
                // 타일이 다른 레이어가 없는지
                // 미션에 같은 색깔을 가진 노멀피스가 "없는지" 여부로 우선 소팅 >> 마그네틱과 다르니 확인하기!
                .OrderBy(tile => Controller.Missions.Any(m => IsValidMission(m, tile)) ? 0 : 1)
                // 이 색깔을 가진 타일이 3개 이상이어야 하므로 그룹으로 묶어서 카운트 확인
                // 타일이 다른 레이어가 없는지
                .ThenBy(tile => tile.Entities.Values.Any(e => e.Layer > Layer.Piece) ? 0 : 1)
                // 이 색깔을 가진 타일이 3개 이상이어야 하므로 그룹으로 묶어서 카운트 확인
                .GroupBy(tile => tile.Piece.Color)
                // 정렬했으니 첫 번째를 받으면 된다.
                .FirstOrDefault(g => g.Count() >= 3);

            if (sortedTiles == null) return;

            TilesToHit = sortedTiles.Take(3).ToArray();

            bool IsValidMission(Mission mission, Tile tile) {
                if (mission.entity.index != EntityIndex.NormalPiece) return false;
                if (mission.entity.color != tile.Piece.Color) return false;

                return true;
            }
        }

        public override void Undo(bool undoParam) { }

        public override IEnumerable<IAbility> GetCascadedAbility() {
            foreach (var tile in TilesToHit) {
                foreach (var entity in tile.Entities.OrderByDescending(kvp => kvp.Key).Select(kvp => kvp.Value)) {
                    yield return new DestroyEntityOnTileAbility(tile, Controller, entity);
                }
            }
        }
    }
}