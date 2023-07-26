namespace GemMatch {
    public class NormalPiece : Entity {
        public override EntityIndex Index => EntityIndex.Normal;
        public override Layer Layer => Layer.Piece;

        public override Entity Clone() {
            return new NormalPiece { Color = Color };
        }
    }
}