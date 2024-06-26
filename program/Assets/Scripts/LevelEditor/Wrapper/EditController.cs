using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditInspectorEventListener {
        void MakeLevel1();
        void LoadLevel(Level getLevel);
        void SetColorCandidates(List<ColorIndex> colorCandidates);
        Level CurrentLevel { get; }
        void SetMissions(List<Mission> missions);
        void ResizeBoard(int boardHeight, int boardWidth);
    }

    public interface IEditToolEventListener {
    }

    public interface IEditViewEventListener {
        Tile ChangeTile(TileModel editTile);
    }

    public class EditController : Controller, IEditViewEventListener, IEditToolEventListener, IEditInspectorEventListener {

        private readonly IEditCtrlForView _view;
        private readonly IEditCtrlForTool _tool;

        // Memory와 Missions을 사용하지 않는다
        // CurrentLevel,Tiles만 사용
        public EditController(IEditCtrlForView editGameView, IEditCtrlForTool tool) {
            this._view = editGameView;
            this._tool = tool;
        }

        public void EditGame(Level level) {
            CurrentLevel = level;
            Tiles = level.tiles.Select(tileModel => new Tile(tileModel.Clone())).ToArray();
        }

        public override void Input(int tileIndex) {
            Touch(Tiles[tileIndex]);
        }

        private new void Touch(Tile tile) {
            var indexCache = tile.Index;
            var newTiles = Tiles.ToList();
            newTiles.Remove(tile);
            Tiles = newTiles.ToArray();
            var selectedTile = _tool.GetCurrentTile();
            var newModel = new TileModel() {
                entityModels = selectedTile.Model.entityModels,
                isOpened = selectedTile.Model.isOpened,
                index = indexCache,
            };
            _view.UpdateBoard(new Tile(newModel));
        }

        // EditPage로부터 받는 Input
        public void Input(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.UpArrow:
                    if (BoardHeightOpened > 4) ResizeBoard(BoardHeightOpened-1, BoardWidthOpened);
                    break;
                case KeyCode.DownArrow:
                    if (BoardHeightOpened < 11) ResizeBoard(BoardHeightOpened+1, BoardWidthOpened);
                    break;
                case KeyCode.LeftArrow:
                    if (BoardWidthOpened > 4) ResizeBoard(BoardHeightOpened, BoardWidthOpened-1);
                    break;
                case KeyCode.RightArrow:
                    if (BoardWidthOpened < 9) ResizeBoard(BoardHeightOpened, BoardWidthOpened+1);
                    break;
            }
        }

        public void ResizeBoard(int height, int width) {
            var nCol = PickTargetIndex(height, Constants.Height);
            var nRow = PickTargetIndex(width, Constants.Width);

            var closeTarget = CurrentLevel.tiles
                .Where(tileModel => nRow.Contains(tileModel.X) && nCol.Contains(tileModel.Y));

            foreach (TileModel tileModel in CurrentLevel.tiles) {
                if (nRow.Contains(tileModel.X) || nCol.Contains(tileModel.Y)) {
                    tileModel.isOpened = false;
                } else {
                    tileModel.isOpened = true;
                }
            }

            EditGame(CurrentLevel);
            _view.UpdateBoard(base.Tiles.ToList());

            BoardHeightOpened = height;
            BoardWidthOpened = width;

            // Inner Method
            IEnumerable<int> PickTargetIndex(int range, int max) {
                // 양쪽 끝 인텍스부터 선택하는 알고리즘
                var result = new LinkedList<int>(Enumerable.Range(0, max));
                int cnt = max - range;
                while (cnt > 0) {
                    if (cnt % 2 == 0) result.RemoveFirst();
                    else result.RemoveLast();
                    cnt--;
                }
                return Enumerable.Range(0,max).Except(result);
            }
        }

        public Tile ChangeTile(TileModel editTile) {
            var tmpLv = CurrentLevel;
            var toolModel = _tool.GetCurrentTile().Model.Clone();
            toolModel.index = editTile.index;
            Tile result = null;
            if (toolModel.entityModels.Count > 0 && toolModel.entityModels[0].layer == Layer.Cover) {
                // cover일 경우 ToolModel에서 EntityModel만 붙여넣기
                TileModel newTile = tmpLv.tiles[editTile.index].Clone();
                if (newTile.EntityDict.TryGetValue(Layer.Cover, out var prevCoverEntity)) {
                    newTile.entityModels.Remove(prevCoverEntity.Model);
                    newTile.RemoveEntity(prevCoverEntity);
                }
                newTile.entityModels.Add(toolModel.EntityDict[Layer.Cover].Model);
                newTile.AddEntity(toolModel.EntityDict[Layer.Cover]);
                tmpLv.tiles[editTile.index] = newTile;
                result = new Tile(newTile);
            } else {
                // 그 외엔 ToolModel로 바꿔넣기
                tmpLv.tiles[editTile.index] = toolModel;
                result = new Tile(toolModel);
            }
            EditGame(tmpLv);
            return result;
        }

        public void MakeLevel1() {
            var newTiles = new List<Tile>();
            for (var i = 0; i < Constants.Width * Constants.Height; ++i) {
                var x = i % Constants.Width;
                var y = i / Constants.Width;
                var tileModel = new TileModel() { index = i, isOpened = true, entityModels = new List<EntityModel>() };
                var tile = new Tile(tileModel);
                newTiles.Add(tile);
            }

            var newLevel = new Level() {
                colorCandidates = new ColorIndex[] { },
                colorCount = 1,
                missions = new Mission[] {},
                tiles = newTiles.Select(t => t.Model).ToArray()
            };
            LoadLevel(newLevel);
        }

        public void LoadLevel(Level level) {
            BoardWidthOpened = Constants.Width;
            BoardHeightOpened = Constants.Height;
            EditGame(level);
            _view.UpdateBoard(base.Tiles.ToList());
        }

        public void SetColorCandidates(List<ColorIndex> colorCandidates) {
            CurrentLevel.colorCandidates = colorCandidates.ToArray();
        }

        public void SetMissions(List<Mission> missions) {
            CurrentLevel.missions = missions.ToArray();
        }

        public int BoardWidthOpened { get; private set; } = 8;
        public int BoardHeightOpened { get; private set; } = 11;
    }
}