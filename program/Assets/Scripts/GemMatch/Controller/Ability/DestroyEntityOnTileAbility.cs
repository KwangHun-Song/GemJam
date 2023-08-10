namespace GemMatch {
    /// <summary>
    /// 타일에 있는 엔티티 하나를 제거하는 어빌리티
    /// </summary>
    public class DestroyEntityOnTileAbility : Ability<Entity> {
        public override AbilityIndex Index => AbilityIndex.DestroyEntityOnTileAbility;
        private Entity DestroyEntity { get; }
        
        public DestroyEntityOnTileAbility(Tile targetTile, Controller controller, Entity destroyEntity) : base(targetTile, controller) {
            DestroyEntity = destroyEntity;
        }
        
        public override void Run() {
            UndoParam = DestroyEntity;
            TargetTile.RemoveLayer(DestroyEntity.Layer);
            if (DestroyEntity.Layer == Layer.Piece) Controller.SplashHit(TargetTile, true);
            foreach (var listener in Controller.Listeners) listener.OnDestroyEntity(TargetTile, DestroyEntity);
            Controller.CalculateActiveTiles();
        }

        public override void Undo(Entity destroyedEntity) {
            TargetTile.AddEntity(destroyedEntity);
            foreach (var listener in Controller.Listeners) listener.OnCreateEntity(TargetTile, destroyedEntity);
            Controller.CalculateActiveTiles();
        }
    }
}