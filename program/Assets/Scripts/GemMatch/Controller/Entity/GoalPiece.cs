namespace GemMatch {
    public class GoalPiece : Entity {
        public GoalPiece(EntityModel model) : base(model) { }

        public override Entity Clone() {
            return new GoalPiece(Model.Clone());
        }
    }
}