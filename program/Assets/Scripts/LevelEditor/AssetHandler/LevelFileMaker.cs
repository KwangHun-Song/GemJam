namespace GemMatch.LevelEditor {
    public class LevelFileMaker {
        private readonly string _savePath;

        public LevelFileMaker(string savePath) {
            this._savePath = savePath;
        }

        public void Save() {

        }

        private string GetFileName() => $"{_savePath}/{_levelIndex}.csv";

        private int _levelIndex;
        public void SetLevelIndex(int levelIndex) {
            _levelIndex = levelIndex;
        }

        private string levelStream;
        public void SetTargetLevel(Level[] editGameController) {
            // todo: contorller의 데이터를 serialize -> string으로 저장
        }
    }
}