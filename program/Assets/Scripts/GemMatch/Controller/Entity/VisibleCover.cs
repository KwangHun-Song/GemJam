namespace GemMatch {
    public class VisibleCover : Entity {
        public VisibleCover(EntityModel model) : base(model) { }

        public override Entity Clone() {
            return new VisibleCover(Model.Clone());
        }

        public override bool CanBeHit() => true;
        public override bool CanPassThrough() => false;
        public override bool PreventTouch() => true;
        public override HitResultInfo Hit() => new HitResultInfo(HitResult.Destroyed, true);
    }
}