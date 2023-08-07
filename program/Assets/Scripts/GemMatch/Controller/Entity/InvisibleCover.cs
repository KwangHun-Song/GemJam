namespace GemMatch {
    public class InvisibleCover : Entity {
        public InvisibleCover(EntityModel model) : base(model) { }

        public override Entity Clone() {
            return new InvisibleCover(Model.Clone());
        }

        public override bool CanBeHit() => true;
        public override bool CanPassThrough() => false;
        public override bool PreventTouch() => true;
        public override HitResultInfo Hit() => new HitResultInfo(HitResult.Destroyed, true);
    }
}