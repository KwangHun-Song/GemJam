namespace GemMatch {
    public class NormalPiece : Entity {
        public override Entity Clone() {
            return new NormalPiece(Model.Clone());
        }

        public NormalPiece(EntityModel model) : base(model) { }
    }
}