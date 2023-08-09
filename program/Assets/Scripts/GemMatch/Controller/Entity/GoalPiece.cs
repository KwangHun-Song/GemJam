namespace GemMatch {
    /// <summary>
    /// 활성화되자마자 미션을 얻는 엔티티
    /// </summary>
    public class GoalPiece : Entity {
        public GoalPiece(EntityModel model) : base(model) { }

        public override Entity Clone() {
            return new GoalPiece(Model.Clone());
        }
    }
}