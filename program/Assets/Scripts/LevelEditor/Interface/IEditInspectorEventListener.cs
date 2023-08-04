using System.Collections.Generic;

namespace GemMatch.LevelEditor {
    public interface IEditInspectorEventListener {
        void MakeLevel1();
        void LoadLevel(Level getLevel);
        void SetColorCandidates(List<ColorIndex> colorCandidates);
    }
}