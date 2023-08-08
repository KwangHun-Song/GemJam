using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditToolView {
        void Initialize(EditTool editTool, EditView view, Tile tile);
        GameObject gameObject { get; }
        Tile Tile { get; }
        string name { get; set; }
        RectTransform transform { get; }
    }
}