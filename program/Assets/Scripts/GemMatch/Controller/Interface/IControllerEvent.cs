using System.Collections.Generic;

namespace GemMatch {
    public interface IControllerEvent {
        void OnStartGame(Controller controller);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);
        
        void OnMoveToMemory(Tile tile, Entity entity);
        void OnRemoveMemory(Entity entity);
        void OnRunAbility(Ability ability);

        void OnAddActiveTiles(IEnumerable<Tile> tiles);
    }
}