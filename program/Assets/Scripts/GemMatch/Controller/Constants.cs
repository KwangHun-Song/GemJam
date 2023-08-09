using System.Collections.Generic;

namespace GemMatch {
    public static class Constants {
        public const int Width = 8;
        public const int Height = 11;

        /// <summary>
        /// 게임에 사용 가능한 컬러들
        /// </summary>
        public static readonly IEnumerable<ColorIndex> UsableColors = new[] {
            ColorIndex.Red,
            ColorIndex.Orange,
            ColorIndex.Yellow,
            ColorIndex.Green,
            ColorIndex.Blue,
            ColorIndex.Purple,
            // ColorIndex.Brown, // todo: 추수 보석 색 추가 시 연다
            // ColorIndex.Pink,
            // ColorIndex.Cyan,
        };

        public const string LevelIndexPrefsKey = "LAST_INDEX";
    }
}