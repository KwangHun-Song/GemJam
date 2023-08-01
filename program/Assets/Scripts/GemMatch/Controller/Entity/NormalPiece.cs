namespace GemMatch {
    public class NormalPiece : Entity {
        public override Entity Clone() {
            return new NormalPiece(Model);
        }

        public NormalPiece(EntityModel model) : base(model) { }
    }
}