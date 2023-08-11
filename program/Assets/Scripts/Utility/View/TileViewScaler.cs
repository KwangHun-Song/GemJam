using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utility {
    public class TileViewScaler : MonoBehaviour {
        public enum SideCount { Type7 = 7, Type8 = 8 }
        private float[] gridWidth = new [] { 535.5f, 612.4f };
        public RectTransform grid;
        public GameObject sideBlockFor7;
        public GameObject sideBlockFor8;

        public void SetSideBlock(SideCount count) {
            float width = count switch {
                SideCount.Type7 => gridWidth[0],
                SideCount.Type8 => gridWidth[1],
                _ => throw new ArgumentOutOfRangeException(nameof(count), count, null)
            };
            grid.sizeDelta = new Vector2(width, grid.sizeDelta.y);
            sideBlockFor7.SetActive(count == SideCount.Type7);
            sideBlockFor8.SetActive(count == SideCount.Type8);
        }
    }
}