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
            
            Refresh();
            
            foreach (var entity in Tile.Entities) {
                var entityView = View.CreateEntityView(entity, entitiesRoot);
                entityView.Initialize(this, entity);
                EntityViews.Add(entityView);
            }
        }

        public void Refresh() {
            background.color = Tile.IsOpened == false ? Color.gray : Color.white;
        }

        public async UniTask ApplyEntityViewAsync(EntityView entityView, bool immediately = false) {
            EntityViews.Add(entityView);
            
            var entityViewTfm = entityView.transform;
            entityViewTfm.SetParent(entitiesRoot);
            entityViewTfm.localPosition = Vector3.zero;

            if (immediately) {
                entityViewTfm.localScale = Vector3.one;
            } else {
                entityViewTfm.localScale = Vector3.zero;
                await entityViewTfm.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack).ToUniTask();
            }
        }

        public EntityView RemoveEntityView(Layer layer) {
            if (EntityViews.Any(view => view.Entity.Layer == layer) == false) return null;

            var entityView = EntityViews.Single(view => view.Entity.Layer == layer);
            EntityViews.Remove(entityView);

            return entityView;
        }

        public void Register(ITouchableListener listener) {
            touchable.Register(this, listener);
        }
    }
}