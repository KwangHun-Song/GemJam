using System;
using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    [Serializable]
    public class TileModel : ICloneable<TileModel> {
        public int index;
        public bool isOpened;
        public List<EntityModel> entityModels;
        
        private List<Entity> entities;
        private List<Entity> Entities => entities ??= entityModels.Select(Utility.GetEntity).ToList();

        private SortedDictionary<Layer, Entity> entityDict;
        public IReadOnlyDictionary<Layer, Entity> EntityDict => entityDict ??= GetEntityDictionary(Entities);

        public int Index => index;
        public bool IsOpened => isOpened;
        public int X => Index % Constants.Width;
        public int Y => Index / Constants.Width;

        public void AddEntity(Entity entity) {
            Entities.Add(entity);
            entityDict = GetEntityDictionary(Entities);
        }

        public bool RemoveEntity(Entity entity) {
            var isSuccess = Entities.Remove(entity);
            entityDict = GetEntityDictionary(Entities);
            return isSuccess;
        }

        public void UpdateEntityDict() => entityDict = GetEntityDictionary(Entities);

        private SortedDictionary<Layer, Entity> GetEntityDictionary(IEnumerable<Entity> entities) {
            return new SortedDictionary<Layer, Entity>(entities.ToDictionary(e => e.Layer));
        }

        public TileModel Clone() {
            return new TileModel {
                index = index,
                isOpened = isOpened,
                entityModels = entityModels?.Select(em => em.Clone()).ToList() ?? new List<EntityModel>()
            };
        }


        // todo:conflict시 지워야함
        public int X => index % Constants.Width;
        public int Y => index / Constants.Width;
    }
}