using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace GemMatch {
    public class RandomColorCalculator {
        public static Queue<ColorIndex> GenerateColorQueue(int queueCount, List<ColorIndex> colors) {
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
            // 개수는 같아야 한다.
            Assert.AreEqual(queueCount, colorItems.Count);
            
            // 결과물은 여기에 저장할 것이다.
            var stack = new Stack<ColorIndex>();

            int safeStop = 0;
            int slots = 7; // 여기에 가능한 슬롯인지 체크한다.
            while (stack.Count < queueCount && safeStop < 5000) {
                safeStop++;
                // 랜덤으로 하나를 선택한다.
                var candidate = colorItems.PickRandom();

                // 이번 색깔을 넣어서, 그 색깔이 3의 배수가 된다면 추가한다. 
                if ((stack.Count(c => c == candidate) + 1) % 3 == 0) {
                    stack.Push(candidate);
                    colorItems.Remove(candidate);
                    slots += 3;
                }

                // 슬롯에 2칸 이상 남았다면 색깔을 추가한다.
                if (slots - stack.Count > 1) {
                    stack.Push(candidate);
                    colorItems.Remove(candidate);
                } else {
                    // 실패했으면 스택에서 세 개를 뺀다.
                    colorItems.Add(stack.Pop());
                    colorItems.Add(stack.Pop());
                    colorItems.Add(stack.Pop());
                }
            }

            return new Queue<ColorIndex>(stack.Reverse());
        }
    }
}