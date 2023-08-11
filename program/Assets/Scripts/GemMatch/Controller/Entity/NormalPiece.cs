namespace GemMatch {
    public class NormalPiece : Entity {
        public override Entity Clone() {
            return new NormalPiece(Model.Clone());
        }

        public override bool CanTouch() => true;

        public NormalPiece(EntityModel model) : base(model) { }
    }
}