using System.Collections.Generic;

namespace GemMatch {
    public interface IControllerEvent {
        void OnStartGame(Controller controller);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);
        void OnChangeMission(Mission mission, int changeCount);
        
        void OnMoveToMemory(Tile tile, Entity entity);
        void OnMoveFromMemory(Tile tile, Entity entity);
        void OnCreateMemory(Entity entity);
        void OnDestroyMemory(Entity entity);
        void OnRunAbility(IAbility ability);
        void OnRestoreAbility(IAbility ability);
        void OnCreateEntity(Tile tile, Entity entity);
        void OnDestroyEntity(Tile tile, Entity entity);

        void OnAddActiveTiles(IEnumerable<Tile> activeTiles);
    }
}