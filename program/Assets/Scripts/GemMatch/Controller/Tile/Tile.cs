using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class Tile {
        /// <summary>
        /// 타일은 다른 프로퍼티를 가지지 않고 모델 하나만 가진다. 이곳에 타일의 모든 데이터가 저장된다.
        /// 타일을 클론할 때 이 모델만 클론하면 된다.
        /// </summary>
        public TileModel Model { get; }
        
        public int Index => Model.index;
        public bool IsOpened => Model.IsOpened;
        public int X => Index % Constants.Width;
        public int Y => Index / Constants.Width;
        
        public IReadOnlyDictionary<Layer, Entity> Entities => Model.EntityDict;

        /// <summary>
        /// 가장 많이 사용하는 레이어인 피스 엔티티를 얻을 수 있는 숏컷
        /// </summary>
        public Entity Piece => Entities.ContainsKey(Layer.Piece) == false ? null : Entities[Layer.Piece];

        public Tile(TileModel model) => Model = model.Clone();

        public Tile Clone() {
            return new Tile(Model.Clone());
        }

        public bool CanPassThrough() {
            if (IsOpened == false) return false;
            if (Entities.Any() == false) return true;

            return Entities.Values.All(e => e.CanPassThrough());
        }
        
        public bool AddEntity(Entity entity) {
            if (Entities.ContainsKey(entity.Layer)) return false;
            Model.AddEntity(entity);

            return true;
        }

        public bool RemoveLayer(Layer layer) {
            if (Entities.ContainsKey(layer) == false) return false;
            var isSuccess = Model.RemoveEntity(Entities[layer]);
            
            return isSuccess;
        }
    }
}