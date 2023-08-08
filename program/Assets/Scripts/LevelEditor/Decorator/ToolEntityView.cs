using UnityEngine;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(EntityView))]
    public class ToolEntityView : MonoBehaviour, IEditToolView {
        private EditTool _tool;
        private EntityView _entityView;
        private Entity[] _entities;
        private EditView _editView;

        public Tile Tile { get; private set; }
        public RectTransform transform => gameObject.transform as RectTransform;
        public Entity Entity { get; private set; }

        private void OnEnable() {
            this._entityView = this.GetComponent<EntityView>();
            var btn = this.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(this.OnClick);
        }

        public void Initialize(EditTool tool, EditView view, Tile tile) {
            this._tool = tool;
            this._editView = view;
            Tile = tile;
            var entityViewScript = _entityView.GetComponent<EntityView>();
            Entity = entityViewScript.Entity;
            _entityView.Initialize(null, tile.Entities[Layer.Piece]);
            _entityView.GetComponent<Button>().enabled = false;
            if (entityViewScript is NormalPieceView normalPiece) {
                normalPiece.SetClickable(true);
            } else {
                //todo: 다른 블록 계열일때 처리
            }
        }


        public void OnClick() {
            _tool.OnClickToolTile(this);
        }
    }
}