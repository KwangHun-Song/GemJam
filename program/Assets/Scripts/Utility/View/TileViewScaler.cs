using System.Linq;
using GemMatch;
using UnityEngine;

namespace Utility {
    public class TileViewScaler : MonoBehaviour {
        public enum SideCount { Type7 = 7, Type8 = 8 }
        private float[] gridWidth = new [] { 535.5f, 612.4f };
        public RectTransform grid;
        public GameObject leftSideEdge;
        public GameObject rightSideEdge;

        public void SetSideBlock(SideCount count) {
            if (count == SideCount.Type7)
                grid.localPosition = new Vector3(-78 / 2, grid.localPosition.y, grid.localPosition.z);
            else
                grid.localPosition = new Vector3(0f, grid.localPosition.y, grid.localPosition.z);

            leftSideEdge.SetActive(isLeftSideFilled == false);
            rightSideEdge.SetActive(isRightSideFilled == false);
        }

        private bool isOddNumber = false;
        private bool isLeftSideFilled = false;
        private bool isRightSideFilled = false;

        public SideCount CalculateWidth(TileModel[] tileModels) {
            var tileXs = tileModels.Where(t=>t.IsOpened).Select(t=>t.X).Distinct();
            isLeftSideFilled = tileXs.Min() == 0;
            isRightSideFilled = tileXs.Max() == 7;
            var substract = tileXs.Max() - tileXs.Min();
            return substract % 2 == 0 ? SideCount.Type7 : SideCount.Type8;
        }

        public void SetPlayViewPosition(Tile[] tiles) {
            var tile = tiles.Select(t=>t.Model).ToArray();
            SetSideBlock(CalculateWidth(tile));
        }
    }
}