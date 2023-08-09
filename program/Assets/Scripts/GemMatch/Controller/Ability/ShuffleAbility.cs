using System.Linq;

namespace GemMatch {
    public class ShuffleAbility : Ability<ColorIndex[]> {
        public override AbilityIndex Index => AbilityIndex.ShuffleAbility;
        public ShuffleAbility(Controller controller) : base(null, controller) { }

        public override void Run() {
            var colorEntities = Controller.Tiles
                .Where(t => t.Piece is NormalPiece)
                .Select(t => t.Piece)
                .ToArray();

            var originColors = colorEntities.Select(e => e.Color).ToArray();
            UndoParam = originColors;
            var shuffledColors = originColors.Shuffle().ToArray();

            for (int i = 0; i < shuffledColors.Length; i++) {
                colorEntities[i].Color = shuffledColors[i];
            }
        }

        public override void Undo(ColorIndex[] originColors) {
            var colorEntities = Controller.Tiles
                .Where(t => t.Piece is NormalPiece)
                .Select(t => t.Piece)
                .ToArray();
            
            for (int i = 0; i < originColors.Length; i++) {
                colorEntities[i].Color = originColors[i];
            }
        }
    }
}