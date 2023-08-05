using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditGameController : IEditViewEventListener, IEditToolEventListener, IEditInspectorEventListener{
        void Initialize(string levelStream); // level stream으로 데이터를 채운다
        void Input(int tileIndex);
    }

    public interface IEditInspectorEventListener {
        void MakeLevel1();
        void LoadLevel(Level getLevel);
        void SetColorCandidates(List<ColorIndex> colorCandidates);
    }

    public interface IEditToolEventListener {
    }

    public interface IEditViewEventListener {
        void Input(int index);
        void ChangeTile(Tile tile);
    }

    public class EditController : Controller, IEditGameController {

        private readonly IEditCtrlEventToView _view;
        private readonly IEditCtrlEventToTool _tool;

        // Memory와 Missions을 사용하지 않는다
        // CurrentLevel,Tiles만 사용
        public EditController(IEditCtrlEventToView editGameView, IEditCtrlEventToTool tool, EditInspector inspector) {
            this._view = editGameView;
            this._tool = tool;
            // inspector.OnSaveLevel += lvs => {
                // lvs.Add(CurrentLevel);
                // return lvs;
            // };
        }

        public void EditGame(Level level) {
            base.StartGame(level);
            _view.OnEditGame(this);
            _tool.OnEditGame(this);
        }

        public event Func<int, Level> OnLoadInspector;
        public void LoadInspector(EditInspector editInspector) {
            editInspector.LoadLevel(1);
        }

        public override void Input(int tileIndex) {
            Touch(Tiles[tileIndex]);
        }

        private void Touch(Tile tile) {
            tile = _tool.GetCurrentTile();
            _view.UpdateBoard(tile);
        }

        // EditPage로부터 받는 Input
        public void Input(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.A:
                    break;
                case KeyCode.UpArrow:
                    if (Height > 4) Height--;
                    break;
                case KeyCode.DownArrow:
                    if (Height < 11) Height++;
                    break;
                case KeyCode.LeftArrow:
                    if (Width > 4) Width--;
                    break;
                case KeyCode.RightArrow:
                    if (Width < 9) Width++;
                    break;
            }
            _view.ResizeBoard(Height, Width);
        }

        public void ChangeTile(Tile tile) {
            CurrentLevel.tiles[tile.Index] = tile.Model.Clone();
            StartGame(CurrentLevel);
        }

        public void MakeLevel1() {
            EditTiles = new List<Tile>();
            EditEntities = new List<Entity>();
            Width = 9;
            Height = 11;
            for (var i = 0; i < Width * Height; ++i) {
                var x = i % Width;
                var y = i / Width;
                var tileModel = new TileModel() { index = i, isOpened = false, entityModels = EditEntityModels };
                EditTileModels.Add(tileModel);
                var tile = new Tile(tileModel);
                EditTiles.Add(tile);
            }
            _view.UpdateBoard(EditTiles);
        }

        public void InstantiateLevel(Level level) {
            EditTileModels.Clear();
            foreach (TileModel model in level.tiles) {
                EditTileModels.Add(model.Clone());
            }
            EditTiles = new List<Tile>();
            EditEntities = new List<Entity>();
        }

        public void LoadLevel(Level level) {
            StartGame(level);
            _view.UpdateBoard(base.Tiles.ToList());
        }

        public void SetColorCandidates(List<ColorIndex> colorCandidates) {
            CurrentLevel.colorCandidates = colorCandidates.ToArray();
        }

        public List<Tile> EditTiles { get; private set; } = new List<Tile>();
        public List<Entity> EditEntities { get; private set; } = new List<Entity>();
        public List<EntityModel> EditEntityModels { get; private set; } = new List<EntityModel>();
        public List<TileModel> EditTileModels { get; private set; } = new List<TileModel>();
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}