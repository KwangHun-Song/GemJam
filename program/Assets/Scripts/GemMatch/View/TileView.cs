using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] public Transform entitiesRoot;
        
        public Tile Tile { get; private set; }
        public View View { get; private set; }

        public Dictionary<Layer, EntityView> EntityViews { get; } = new Dictionary<Layer, EntityView>();

        public void Initialize(View view, Tile tile) {
            View = view;
            Tile = tile;
            
            Redraw();

            foreach (var entityView in EntityViews.Values) {
                DestroyImmediate(entityView.gameObject);
            }
            
            EntityViews.Clear();
            
            foreach (var entity in Tile.Entities.Values) {
                var entityView = View.CreateEntityView(entity, this);
                entityView.Initialize(this, entity);
                EntityViews.Add(entity.Layer, entityView);
            }
        }

        public void AddEntityView(EntityView entityView) {
            EntityViews.Add(entityView.Entity.Layer, entityView);
            entityView.transform.SetParent(entitiesRoot);
            entityView.transform.localPosition = Vector3.zero;
            entityView.transform.localScale = Vector3.one;
            entityView.Initialize(this, entityView.Entity);
        }

        public void Redraw() {
            background.color = Tile.Model.IsOpened == false ? Color.gray : Color.white;
        }
    }
}