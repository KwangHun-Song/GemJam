namespace GemMatch.LevelEditor {
    public interface IEditToolEventListener {
        event System.Action<IEditToolEventListener> OnTouch;
    }
}