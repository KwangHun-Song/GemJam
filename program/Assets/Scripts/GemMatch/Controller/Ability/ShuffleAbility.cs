using System.Linq;

namespace GemMatch {
    public class ShuffleAbility : Ability {
        public override AbilityIndex Index => AbilityIndex.ShuffleAbility;
        public ShuffleAbility(Tile targetTile, Controller controller) : base(targetTile, controller) { }

        public override Ability Run() {
            var colorEntities = Controller.Tiles
                .Where(t => t.Piece is NormalPiece)
                .Select(t => t.Piece)
                .ToArray();

            var shuffledColors = colorEntities.Select(e => e.Color).Shuffle().ToArray();

            for (int i = 0; i < shuffledColors.Length; i++) {
                colorEntities[i].Color = shuffledColors[i];
            }

            return this;
        }
    }
}