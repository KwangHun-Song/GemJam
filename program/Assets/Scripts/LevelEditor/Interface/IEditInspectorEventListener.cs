namespace GemMatch.LevelEditor {
    public interface IEditInspectorEventListener {
        event System.Action<IEditInspectorEventListener> OnTouch;
        Level[] GetLevelStatus();
    }
}