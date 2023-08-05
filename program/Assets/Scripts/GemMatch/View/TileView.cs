using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] private Transform entitiesRoot;
        [SerializeField] private TMP_Text cheatIndexText;
        
        public Tile Tile { get; private set; }
        public View View { get; private set; }

        public List<EntityView> EntityViews { get; } = new List<EntityView>();

        public void Initialize(View view, Tile tile) {
            View = view;
            Tile = tile;
            
            Redraw();

            foreach (var entityView in EntityViews) {
                DestroyImmediate(entityView.gameObject);
            }
            
            EntityViews.Clear();
            
            foreach (var entity in Tile.Entities.Values) {
                var entityView = View.CreateEntityView(entity, entitiesRoot);
                entityView.Initialize(this, entity);
                EntityViews.Add(entityView);
            }
        }

        public void Redraw() {
            background.color = Tile.Model.IsOpened == false ? Color.gray : Color.white;
            if (Tile.IsOpened) cheatIndexText.text = Tile.Index.ToString();
        }

        #region 임시로 만든 치트기능입니다! 곧 삭제할 예정

        [SerializeField] private TMP_Text clickedOrderText;
        private void Update() {
            if (Tile?.ClickedOrder > -1)
                clickedOrderText.text = $"{Tile.ClickedOrder}";
        }

        #endregion
    }
}