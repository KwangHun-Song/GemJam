using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] private Image randomBackground;
        [SerializeField] public Transform entitiesRoot;
        [SerializeField] public Transform guestRoom;
        [SerializeField] public Sprite[] backgroundSprites; // open(dirt), close(wall)
        [SerializeField] public Sprite[] randomBackgroundSprites; // open(dirt), close(wall)
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
            background.sprite = Tile.Model.IsOpened ? backgroundSprites[0] : backgroundSprites[1];
            var randomNum = Random.Range(0, 200);
            if (Tile.Model.IsOpened == false) {
                Sprite randomSprite = randomNum switch {
                    _ when randomNum >= 190 => randomBackgroundSprites[0],
                    _ when randomNum >= 170 => randomBackgroundSprites[1],
                    _ when randomNum >= 150 => randomBackgroundSprites[2],
                    _ => null
                };
                randomBackground.gameObject.SetActive(randomSprite != null);

                if (randomSprite != null) {
                    randomBackground.sprite = randomSprite;
                    randomBackground.preserveAspect = true;
                    randomBackground.transform.Rotate(0f, 0f, (randomNum % 5) * (360/5));
                }
            }
        }

        public void RedrawByAdjacents(Func<Tile, Tile[], IEnumerable<Tile>> adjacentTilesCall, Tile[] controllerTiles) {
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
            points[0].SetActive(left && up);
            points[1].SetActive(left && down);
            points[2].SetActive(right && up);
            points[3].SetActive(right && down);

            if (left && down) {
                var ranTr = (randomBackground.transform as RectTransform);
                ranTr.pivot = new Vector2(Random.Range(0f, 0.08f), Random.Range(0f, 0.08f));
                ranTr.localPosition = Vector3.zero;
                ranTr.Rotate(0f,0f,(float)Random.Range(0,360));
                randomBackground.sprite = Random.Range(0, 10) % 2 == 1
                    ? randomBackgroundSprites[3]
                    : randomBackgroundSprites[4];
            }
        }

    }
}