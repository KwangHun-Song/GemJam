namespace GemMatch {
    public interface IControllerEvent {
        void OnStartGame(Controller controller);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);
        
        void OnAddMemory(Controller controller, Entity entity);
        void OnRemoveMemory(Controller controller,Entity entity);
    }
}