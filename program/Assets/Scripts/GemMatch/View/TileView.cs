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
                if (Random.Range(0, 10) != 0) return;
                smallDeco.gameObject.SetActive(false);
                bigDego.gameObject.SetActive(true);
                if (Random.Range(0, 5) != 0) {
                    bigDego.sprite = randomBackgroundSprites[4];
                } else {
                    // 뼈다귀는 나올 확률을 더 낮춘다. 극한의 확률
                    bigDego.sprite = randomBackgroundSprites[3];
                }
                bigDego.transform.localScale = Vector3.one * Random.Range(0.6F, 1F);
                bigDego.transform.eulerAngles = Vector3.forward * Random.Range(0F, 360F);
                IsShowingDeco = true;
            } else if (Tile.IsOpened == false) {
                if (Random.Range(0, 6) != 0) return;
                bigDego.gameObject.SetActive(false);
                smallDeco.gameObject.SetActive(true);
                smallDeco.sprite = randomBackgroundSprites[Random.Range(0, 3)];
                smallDeco.transform.localScale = Vector3.one * Random.Range(0.6F, 1F);
                smallDeco.transform.eulerAngles = Vector3.forward * Random.Range(0F, 360F);
                IsShowingDeco = true;
            }
        }

        public void DrawEdges(Tile[] controllerTiles) {
            if (Tile.IsOpened) {
                foreach (var edge in edges.Concat(points)) {
                    edge.SetActive(false);
                }
                return;
            }
            
            var adjTiles = TileUtility.GetAdjacentWithDiagonalTiles(Tile, controllerTiles);

            var showUpEdge = adjTiles.Up?.IsOpened == false;
            var showDownEdge = adjTiles.Down?.IsOpened == false;
            var showLeftEdge = adjTiles.Left?.IsOpened == false;
            var showRightEdge = adjTiles.Right?.IsOpened == false;
            
            showLeftEdge |= Tile.X == 0;
            showRightEdge |= Tile.X == Constants.Width - 1;

            edges[0].SetActive(showUpEdge);
            edges[1].SetActive(showDownEdge);
            edges[2].SetActive(showLeftEdge);
            edges[3].SetActive(showRightEdge);

            var showLUPoint = adjTiles.Left?.IsOpened == false && 
                              adjTiles.Up?.IsOpened == false &&
                              adjTiles.LeftUp?.IsOpened == false;
            var showLDPoint = adjTiles.Left?.IsOpened == false && 
                              adjTiles.Down?.IsOpened == false &&
                              adjTiles.LeftDown?.IsOpened == false;
            var showRUPoint = adjTiles.Right?.IsOpened == false && 
                              adjTiles.Up?.IsOpened == false &&
                              adjTiles.RightUp?.IsOpened == false;
            var showRDPoint = adjTiles.Right?.IsOpened == false && 
                              adjTiles.Down?.IsOpened == false &&
                              adjTiles.RightDown?.IsOpened == false;

            showLDPoint |= Tile.X == 0 && Tile.Y > 0 && adjTiles.Down?.IsOpened == false;
            showRDPoint |= Tile.X == Constants.Width - 1 && Tile.Y > 0 && adjTiles.Down?.IsOpened == false;
            
            points[0].SetActive(showLUPoint);
            points[1].SetActive(showLDPoint);
            points[2].SetActive(showRUPoint);
            points[3].SetActive(showRDPoint);
        }
    }
}