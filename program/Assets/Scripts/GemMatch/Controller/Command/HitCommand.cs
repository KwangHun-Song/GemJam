using GemMatch.UndoSystem;

namespace GemMatch {
    public class HitCommand : Command<Entity> {
        public HitCommand(Controller controller, Tile tile, Entity entity, bool triggeredByPrev = false) : base(
            @do: () => {
                tile.RemoveLayer(entity.Layer);
                foreach (var listener in  controller.Listeners) listener.OnDestroyEntity(tile, entity);
            }, 
            undo: destroyedEntity => {
                tile.AddEntity(destroyedEntity);
                foreach (var listener in controller.Listeners) listener.OnCreateEntity(tile, entity);
            }, 
            param: entity, 
            triggeredByPrev) {
        }
    }
}