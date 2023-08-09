using System.Linq;

namespace GemMatch {
    public class HitRandomAbility : Ability {

        public override AbilityIndex Index => AbilityIndex.HitRandomAbility;
        public bool Found { get; set; }
        
        public HitRandomAbility(Tile targetTile, Controller controller) : base(targetTile, controller) { }

        public override Ability Run() {
            Found = false;
            var tileGroup = Controller.Tiles.Where(tile => {
                    // 노멀 피스를 가지고 있어야 한다.
                    if (tile.Piece is not NormalPiece normalPiece) return false;
                    // 이미 활성화되어 있다면 후보에서 제외한다.
                    if (Controller.CanTouch(tile)) return false;
                    // 노멀 피스보다 상위의 피스를 가지면 안 된다.
                    if (tile.Entities.Values.Any(e => e.Layer > Layer.Piece)) return false;
                    // 사용 가능한 색깔이어야 한다.
                    if (!Constants.UsableColors.Contains(normalPiece.Color)) return false;

                    return true;
                }).GroupBy(t => t.Piece.Color)
                .Where(g => g.Count() >= 3)
                .ToList()
                .Shuffle()
                .FirstOrDefault();

            if (tileGroup == null) return this;

            Found = true;

            var threeTiles = tileGroup.ToList().Shuffle().Take(3);
            foreach (var tile in threeTiles) {
                Controller.SplashHit(tile);
            }

            return this;
        }
    }
}