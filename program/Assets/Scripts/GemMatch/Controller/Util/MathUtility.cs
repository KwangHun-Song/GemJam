using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public static class MathUtility {
        private static int[] primeNumbers = {
            17 ,13 ,11 ,7 ,5 ,3 ,2
        };

        public static bool IsPrime(int candidate) {
            if ((candidate & 1) == 0) {
                if (candidate == 2) {
                    return true;
                } else {
                    return false;
                }
            }

            for (int i = 3; (i * i) <= candidate; i += 2) {
                if ((candidate % i) == 0) {
                    return false;
                }
            }

            return candidate != 1;
        }

        public static (int height, int width) EstimatedSize(int totalTileCount) {
            int first, second;
            first = second = -1;
            // 홀수면 그냥 -1 리턴
            if (totalTileCount % 2 == 1) return (first, second);
            foreach (int prime in primeNumbers) {
                if (totalTileCount % prime == 0) {
                    if (first == -1) first = prime;
                    if (second == -1) {
                        second = prime;
                        return (first, second);
                    }
                }
            }
            return (first, second);
        }

        public static IList<T> Shuffle<T>(this IList<T> list) {
            int cnt = list.Count;
            while (cnt > 1) {
                cnt--;
                int ran = UnityEngine.Random.Range(0, cnt + 1);
                (list[ran], list[cnt]) = (list[cnt], list[ran]);
            }
            return list;
        }


        // 3으로 나누어 떨어지는 블록의 색을 랜덤하게 주어짐
        public static Queue<ColorIndex> Create3MatchColorQueue(int length) {
            var result = new Queue<ColorIndex>();
            var colors =
                Enumerable.Repeat(0, length / 3)
                    .Select(_=> (ColorIndex)UnityEngine.Random.Range(1, (int)ColorIndex.Random))
                    .ToArray();
            var ranList = Enumerable.Range(0, length).ToList().Shuffle();
            foreach (int i in ranList) {
                result.Enqueue(colors[i % (length / 3)]);
            }
            return result;
        }
    }
}