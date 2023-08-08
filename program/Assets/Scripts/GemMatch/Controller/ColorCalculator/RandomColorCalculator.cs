using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace GemMatch {
    public class RandomColorCalculator : IColorCalculator {
        public Queue<ColorIndex> GenerateColorQueue(int queueCount, List<ColorIndex> colors) {
            // 결과는 3의 배수여야 한다.
            Assert.AreEqual(0, queueCount % 3);
            
            var setsCount = queueCount / 3;
            var colorSets = new List<ColorIndex>();
            
            // 적어도 한 번씩은 선택되어야 하므로 기존 컬러들 하나씩 선택
            colorSets.AddRange(colors.Take(setsCount));
            
            // 더 필요한 만큼 colors에서 랜덤으로 뽑아서 추가
            for (int i = 0; i < setsCount - colors.Count; i++) {
                colorSets.Add(colors.PickRandom());
            }

            // 컬러를 세 개씩 하면, 이제 queue에 들어갈 컬러들이 된다.
            var colorItems = colorSets.SelectMany(color => Enumerable.Repeat(color, 3)).ToList();
            
            return new Queue<ColorIndex>(colorItems.Shuffle());
        }
    }
}