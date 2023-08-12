using System;
using System.Collections.Generic;
using System.Linq;
using GemMatch.LevelEditor;
using PagePopupSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GemMatch {
    public class TileView : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] public Transform entitiesRoot;
        [SerializeField] public Transform guestRoom;
        [SerializeField] public Sprite[] backgroundSprites; // open(dirt), close(wall)
        [SerializeField] public Sprite[] randomBackgroundSprites; // open(dirt), close(wall)
        [SerializeField] public GameObject[] edges; // up, down, left, right
        [SerializeField] public GameObject[] points; // LU, LD, RU, RD

        [SerializeField] private Image bigDego;
        [SerializeField] private Image smallDeco;
        
        public Tile Tile { get; private set; }
        public View View { get; private set; }
        
        public bool IsShowingDeco { get; private set; }

        private Dictionary<Layer, EntityView> entityViews;
        public IReadOnlyDictionary<Layer, EntityView> EntityViews => entityViews ??= new Dictionary<Layer, EntityView>();

        public void Initialize(View view, Tile tile) {
            View = view;
            Tile = tile;
            name = $"TileView({tile.X},{tile.Y})";

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
            // entityView.transform.SetParent(guestRoom);
            entityViews.Remove(entityView.Entity.Layer);
            entityView.Initialize(null);
        }

        public void Redraw() {
            background.sprite = Tile.Model.IsOpened ? backgroundSprites[0] : backgroundSprites[1];

            DrawDeco();
        }

        private void DrawDeco() {
            smallDeco.gameObject.SetActive(false);
            bigDego.gameObject.SetActive(false);
            IsShowingDeco = false;

#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name.Equals("EditScene")) return;
#endif
            if (View.IsRightTopTileOf4ClosedTiles(Tile)) {
                if (Random.Range(0, 4) != 0) return;
                smallDeco.gameObject.SetActive(false);
                bigDego.gameObject.SetActive(true);
                bigDego.sprite = randomBackgroundSprites[Random.Range(3, 5)];
                bigDego.transform.localScale = Vector3.one * Random.Range(0.6F, 1F);
                bigDego.transform.eulerAngles = Vector3.forward * Random.Range(0F, 360F);
                IsShowingDeco = true;
            } else if (Tile.IsOpened == false) {
                if (Random.Range(0, 8) != 0) return;
                bigDego.gameObject.SetActive(false);
                smallDeco.gameObject.SetActive(true);
                smallDeco.sprite = randomBackgroundSprites[Random.Range(0, 3)];
                smallDeco.transform.localScale = Vector3.one * Random.Range(0.6F, 1F);
                smallDeco.transform.eulerAngles = Vector3.forward * Random.Range(0F, 360F);
                IsShowingDeco = true;
            }
        }

        public void DrawEdges(Func<Tile, Tile[], IEnumerable<Tile>> adjacentTilesCall, Tile[] controllerTiles) {
            if (Tile.IsOpened) {
                foreach (var edge in edges.Concat(points)) {
                    edge.SetActive(false);
                }
                return;
            }
            var adjacentTiles = adjacentTilesCall.Invoke(Tile, controllerTiles);
            var direction = Enumerable.Repeat(false, 4).ToArray(); // up, down, left, right
            var showUpEdge = direction[0];
            var showDownEdge = direction[1];
            var showLeftEdge = direction[2];
            var showRightEdge = direction[3];
            
            showLeftEdge |= Tile.X == 0;
            showRightEdge |= Tile.X == Constants.Width - 1;
            foreach (var neighbor in adjacentTiles) {
                if (neighbor.X == Tile.X) {
                    showUpEdge |= neighbor.Y > Tile.Y && neighbor.IsOpened == false;
                    showDownEdge |= neighbor.Y < Tile.Y && neighbor.IsOpened == false;
                } else if (neighbor.Y == Tile.Y) {
                    showLeftEdge |= neighbor.X < Tile.X && neighbor.IsOpened == false;
                    showRightEdge |= neighbor.X > Tile.X && neighbor.IsOpened == false;
                }
            }

            edges[0].SetActive(showUpEdge);
            edges[1].SetActive(showDownEdge);
            edges[2].SetActive(showLeftEdge);
            edges[3].SetActive(showRightEdge);
            points[0].SetActive(showLeftEdge && showUpEdge);
            points[1].SetActive(showLeftEdge && showDownEdge);
            points[2].SetActive(showRightEdge && showUpEdge);
            points[3].SetActive(showRightEdge && showDownEdge);
        }
    }
}