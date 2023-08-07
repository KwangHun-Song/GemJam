using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] private Transform entitiesRoot;
        
        public Tile Tile { get; private set; }
        public View View { get; private set; }

        public List<EntityView> EntityViews { get; } = new List<EntityView>();

        public void Initialize(View view, Tile tile) {
            View = view;
            Tile = tile;
            
            Redraw();

            foreach (var entityView in EntityViews) {
                DestroyImmediate(entityView);
            }
            
            EntityViews.Clear();
            
            foreach (var entity in Tile.Entities) {
                var entityView = View.CreateEntityView(entity, entitiesRoot);
                entityView.Initialize(this, entity);
                EntityViews.Add(entityView);
            }
        }

        public void Redraw() {
            background.color = Tile.IsOpened == false ? Color.gray : Color.white;
        }
    }
}