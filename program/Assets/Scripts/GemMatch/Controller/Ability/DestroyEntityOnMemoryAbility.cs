namespace GemMatch {
    /// <summary>
    /// 메모리에 있는 엔티티를 제거하는 어빌리티
    /// </summary>
    public class DestroyEntityOnMemoryAbility : Ability<Entity> {
        public override AbilityIndex Index => AbilityIndex.DestroyEntityOnMemoryAbility;
        private Entity DestroyEntity { get; }
        
        public DestroyEntityOnMemoryAbility(Controller controller, Entity destroyEntity) : base(null, controller) {
            DestroyEntity = destroyEntity;
        }
        
        public override void Run() {
            UndoParam = DestroyEntity.Clone();
            Controller.Memory.Remove(DestroyEntity);
            foreach (var listener in Controller.Listeners) listener.OnDestroyMemory(DestroyEntity);
        }

        public override void Undo(Entity destroyedEntity) {
            Controller.Memory.Add(destroyedEntity);
            foreach (var listener in Controller.Listeners) listener.OnCreateMemory(destroyedEntity);
        }
    }
}