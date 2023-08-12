using System.Linq;
using GemMatch;
using UnityEngine;

namespace Utility {
    public class TileViewScaler : MonoBehaviour {
        public enum SideCount { Type7 = 7, Type8 = 8 }
        public RectTransform grid;

        public void SetSideBlock(SideCount count) {
            if (count == SideCount.Type7)
                if (leftSidedLevel)
                    grid.localPosition = new Vector3(+78 / 2, grid.localPosition.y, grid.localPosition.z);
                else
                    grid.localPosition = new Vector3(-78 / 2, grid.localPosition.y, grid.localPosition.z);
            else
                grid.localPosition = new Vector3(0f, grid.localPosition.y, grid.localPosition.z);
        }

        private bool isOddNumber = false;
        private bool leftSidedLevel = false;

        private SideCount CalculateWidth(TileModel[] tileModels) {
            var tileXs = tileModels.Where(t=>t.IsOpened).Select(t=>t.X).Distinct();
            leftSidedLevel = Mathf.Abs(tileXs.Min() - 0) < Mathf.Abs(tileXs.Max() - 7);
            var substract = tileXs.Max() - tileXs.Min();
            return substract % 2 == 0 ? SideCount.Type7 : SideCount.Type8;
        }

        public void SetPlayViewPosition(Tile[] tiles) {
            var tile = tiles.Select(t=>t.Model).ToArray();
            SetSideBlock(CalculateWidth(tile));
        }
    }
}