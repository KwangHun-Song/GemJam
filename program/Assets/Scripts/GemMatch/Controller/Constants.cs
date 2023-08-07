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
            ColorIndex.Brown,
            ColorIndex.Pink,
            ColorIndex.Cyan
        };
    }
}