namespace GemMatch.LevelEditor {
    /// <summary>
    /// 레벨 텍스트 파일 로드 -> 객체화
    /// 객체 -> 레벨 텍스트 파일 저장
    /// </summary>
    public class LevelExporter {
        private readonly string _savePath;
        public LevelExporter(string savePath) {
            this._savePath = savePath;
        }

        public string Load(int levelIndex) {
            // todo: csv 파일로 부터 레벨 rawData 로드
            return null;
        }
    }
}