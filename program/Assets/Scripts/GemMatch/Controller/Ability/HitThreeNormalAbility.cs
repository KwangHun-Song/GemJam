using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    /// <summary>
    /// 이 어빌리티는 직접 기능하는 것은 없고 타겟 엔티티들을 찾아 캐스케이드 어빌리티들만 만들어서 반환한다.
    /// </summary>
    public class MagneticAbility : Ability<bool> {
        public override AbilityIndex Index => AbilityIndex.HitRandomAbility;
        public bool Found { get; set; }
        public IEnumerable<Entity> EntitiesInMemory { get; private set; }
        public IEnumerable<Tile> TilesToHit { get; private set; }

        public MagneticAbility(Controller controller) : base(null, controller) { }
    
        public override void Run() {
            var tiles = Controller.Tiles.Where(tile => {
                    // 노멀 피스를 가지고 있어야 한다.
                    if (tile.Piece is not NormalPiece normalPiece) return false;
                    // 사용 가능한 색깔이어야 한다.
                    if (!Constants.UsableColors.Contains(normalPiece.Color)) return false;
                    // InvisibleCover로 가려져 있으면 안 된다.
                    if (tile.Entities.TryGetValue(Layer.Cover, out var cover) && cover is InvisibleCover) return false;
    
                    return true;
                }).ToArray();

            var sortedColors = tiles
                .Select(t => t.Piece.Color)
                .Distinct()
                // 미션에 같은 색깔을 가진 노멀피스가 있는지 여부로 우선 소팅
                .OrderBy(color => Controller.Missions.Any(m => m.entity.index == EntityIndex.NormalPiece && m.entity.color == color) ? 1 : 0)
                // 슬롯에 같은 색깔을 가진 노멀피스의 개수로 세팅
                .ThenByDescending(color => Controller.Memory.Count(me => me.Color == color));
            
            // Hit을 할 타겟 엔티티들
            foreach (var color in sortedColors) {
                var memoryCount = Controller.Memory.Count(me => me.Color == color);
                var tileCount = tiles.Count();
                
                if (memoryCount + tileCount < 3) continue;
                
                var memoryEntities = Controller.Memory.Where(me => me.Color == color).ToList();
                EntitiesInMemory = memoryEntities;
                TilesToHit = tiles.Where(t => t.Piece.Color == color).Take(3 - memoryEntities.Count).ToList();

                Found = true;
                return;
            }

            Found = false;
        }

        public override void Undo(bool hitInfo) { }

        public override IEnumerable<IAbility> GetCascadedAbility() {
            foreach (var entityInMemory in EntitiesInMemory) {
                yield return new DestroyEntityOnMemoryAbility(Controller, entityInMemory);
            }

            foreach (var tile in TilesToHit) {
                foreach (var entity in tile.Entities.OrderByDescending(kvp => kvp.Key).Select(kvp => kvp.Value)) {
                    yield return new DestroyEntityOnTileAbility(tile, Controller, entity);
                }
            }
        }
    }
}