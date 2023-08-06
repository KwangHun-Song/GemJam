using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditInspectorEventListener {
        void MakeLevel1();
        void LoadLevel(Level getLevel);
        void SetColorCandidates(List<ColorIndex> colorCandidates);
        Level CurrentLevel { get; }
    }

    public interface IEditToolEventListener {
    }

    public interface IEditViewEventListener {
        Tile[] Tiles { get; }
        void Input(int index);
        Tile ChangeTile(Tile tile);
    }

    public class EditController : Controller, IEditViewEventListener, IEditToolEventListener, IEditInspectorEventListener {

        private readonly IEditCtrlForView _view;
        private readonly IEditCtrlForTool _tool;
        private readonly IEditCtrlForInspector _inspector;

        // Memory와 Missions을 사용하지 않는다
        // CurrentLevel,Tiles만 사용
        public EditController(IEditCtrlForView editGameView, IEditCtrlForTool tool, IEditCtrlForInspector inspector) {
            this._view = editGameView;
            this._tool = tool;
            this._inspector = inspector;
            // inspector.OnSaveLevel += lvs => {
            // lvs.Add(CurrentLevel);
            // return lvs;
            // };
        }

        public void EditGame(Level level) {
            base.StartGame(level);
            // _view.OnEditGame(this); //todo: 필요할지 생각
            // _tool.OnEditGame(this);
        }

        public event Func<int, Level> OnLoadInspector;
        public void LoadInspector(EditInspector editInspector) {
            editInspector.LoadLevel(1);
        }

        public override void Input(int tileIndex) {
            Touch(Tiles[tileIndex]);
        }

        private void Touch(Tile tile) {
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
                    if (BoardHeightOpened > 4) BoardHeightOpened--;
                    break;
                case KeyCode.DownArrow:
                    if (BoardHeightOpened < 11) BoardHeightOpened++;
                    break;
                case KeyCode.LeftArrow:
                    if (BoardWidthOpened > 4) BoardWidthOpened--;
                    break;
                case KeyCode.RightArrow:
                    if (BoardWidthOpened < 9) BoardWidthOpened++;
                    break;
            }
            ResizeBoard(BoardHeightOpened, BoardWidthOpened);
        }

        private void ResizeBoard(int height, int width) {
            var nCol = PickTargetIndex(height, BoardHeightOpened);
            var nRow = PickTargetIndex(width, BoardWidthOpened);

            var closeTarget = CurrentLevel.tiles
                .Where(tileModel => nRow.Contains(tileModel.X) && nCol.Contains(tileModel.Y));

            foreach (var tileModel in CurrentLevel.tiles) {
                if (nRow.Contains(tileModel.X) && nCol.Contains(tileModel.Y)) {
                    tileModel.isOpened = false;
                }
            }
            EditGame(CurrentLevel);

            // Inner Method
            IEnumerable<int> PickTargetIndex(int range, int max) {
                // 양쪽 끝 인텍스부터 선택하는 알고리즘
                var result = new LinkedList<int>(Enumerable.Range(0, max + 1));
                int cnt = max - range;
                while (cnt > 0) {
                    if (cnt % 2 == 0) result.RemoveFirst();
                    else result.RemoveLast();
                    cnt--;
                }
                return Enumerable.Range(0,max+1).Except(result);
            }
        }

        public Tile ChangeTile(Tile tile) {
            CurrentLevel.tiles[tile.Index] =_tool.GetCurrentTile().Model.Clone();
            return _tool.GetCurrentTile();
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
                colorCandidates = new[] { ColorIndex.None },
                colorCount = 1,
                missions = new[] { new Mission() },
                tiles = newTiles.Select(t => t.Model).ToArray()
            };
            LoadLevel(newLevel);
        }

        public void LoadLevel(Level level) {
            BoardWidthOpened = Constants.Width;
            BoardHeightOpened = Constants.Height;
            StartGame(level); // 초기화 함수 재사용, base.Tiles 갱신
            _view.UpdateBoard(base.Tiles.ToList());
        }

        public void SetColorCandidates(List<ColorIndex> colorCandidates) {
            CurrentLevel.colorCandidates = colorCandidates.ToArray();
        }

        public int BoardWidthOpened { get; private set; } = 8;
        public int BoardHeightOpened { get; private set; } = 11;
    }
}