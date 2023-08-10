using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] public Transform entitiesRoot;
        [SerializeField] public Transform guestRoom;
        [SerializeField] public Sprite[] BackgroundSprites;
        [SerializeField] public GameObject[] edges; // up, down, left, right
        [SerializeField] public GameObject[] points; // LU, LD, RU, RD
        
        public Tile Tile { get; private set; }
        public View View { get; private set; }

        private Dictionary<Layer, EntityView> entityViews;
        public IReadOnlyDictionary<Layer, EntityView> EntityViews => entityViews ??= new Dictionary<Layer, EntityView>();

        public void Initialize(View view, Tile tile) {
            View = view;
            Tile = tile;
            
            Redraw();

            foreach (var entityView in EntityViews.Values) {
                DestroyImmediate(entityView.gameObject);
            }
            
            entityViews.Clear();
            
            foreach (var entity in Tile.Entities.Values) {
                AddEntityView(View.CreateEntityView(entity));
            }
        }

        public void AddEntityView(EntityView entityView) {
            entityViews.Add(entityView.Entity.Layer, entityView);
            entityView.transform.SetParent(entitiesRoot);
            entityView.transform.localPosition = Vector3.zero;
            entityView.transform.localScale = Vector3.one;
            entityView.Initialize(this);
        }

        public void RemoveEntityView(EntityView entityView) {
            entityView.transform.SetParent(guestRoom);
            entityViews.Remove(entityView.Entity.Layer);
            entityView.Initialize(null);
        }

        public void Redraw() {
            // background.color = Tile.Model.IsOpened == false ? Color.gray : Color.white;
            background.sprite = Tile.Model.IsOpened ? BackgroundSprites[0] : BackgroundSprites[1];
        }

        public void RedrawEdges(Func<Tile, Tile[], IEnumerable<Tile>> adjacentTilesCall, Tile[] controllerTiles) {
            if (this.Tile.IsOpened) {
                foreach (var edge in edges.Concat(points)) {
                    edge.SetActive(false);
                }
                return;
            }
            var adjacents = adjacentTilesCall.Invoke(Tile, controllerTiles);
            var direction = Enumerable.Repeat(false, 4).ToArray(); // up, down, left, right
            var up = direction[0];
            var down = direction[1];
            var left = direction[2];
            var right = direction[3];
            foreach (var neighbor in adjacents) {
                if (neighbor.X == Tile.X) {
                    up |= neighbor.Y > Tile.Y && neighbor.IsOpened == false;
                    down |= neighbor.Y < Tile.Y && neighbor.IsOpened == false;
                } else if (neighbor.Y == Tile.Y) {
                    left |= neighbor.X < Tile.X && neighbor.IsOpened == false;
                    right |= neighbor.X > Tile.X && neighbor.IsOpened == false;
                }
            }

            edges[0].SetActive(up);
            edges[1].SetActive(down);
            edges[2].SetActive(left);
            edges[3].SetActive(right);
            points[0].SetActive(up && left);
            points[0].SetActive(down && left);
            points[0].SetActive(up && right);
            points[0].SetActive(up && right);
        }

    }
}