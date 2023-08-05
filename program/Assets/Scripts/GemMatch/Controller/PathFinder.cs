using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class PathFinder {
        public PathFinder(Tile[] tiles) {
            Tiles = tiles;
        }
        
        public Tile[] Tiles { get; }

        public bool HasPathToTop(Tile startTile) {
            // 시작 타일이 가장 상단이면 바로 true 반환
            if (startTile.Y == TileUtility.GetTopY(Tiles)) return true;
            
            // 방문한 타일들을 추적하기 위한 집합, 시작 타일을 방문한 것으로 표시
            var visitedTiles = new HashSet<Tile> { startTile };

            // 인접한 타일들을 검사
            foreach (var adjacentTile in TileUtility.GetAdjacentTiles(startTile, Tiles)) {
                // 이웃 타일로의 경로가 있는지 DFS
                if (FindPathToTopDfs(adjacentTile, visitedTiles)) {
                    return true;
                }
            }
    
            // 모든 가능성을 다 찾아봤지만 경로가 없다면 false 반환
            return false;
        }
        
        private bool FindPathToTopDfs(Tile tile, HashSet<Tile> visitedTiles) {
            // 이미 방문한 타일이라면 바로 반환
            if (visitedTiles.Contains(tile)) return false;
            visitedTiles.Add(tile); // 이 타일을 방문한 것으로 표시

            // 이 타일이 점유되어 있다면 경로가 없는 것으로 간주
            if (tile.CanPassThrough() == false) return false;

            // 타일이 위쪽 가장자리에 도달했다면 true 반환
            if (tile.Y == TileUtility.GetTopY(Tiles)) return true;

            // 인접한 타일들을 검사
            foreach (var adjacentTile in TileUtility.GetAdjacentTiles(tile, Tiles)) {
                // 이웃 타일로의 경로가 있는지 DFS
                if (FindPathToTopDfs(adjacentTile, visitedTiles)) {
                    return true;
                }
            }

            // 모든 가능성을 다 찾아봤지만 경로가 없다면 false 반환
            return false;
        }
    }
}