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
        public SortedDictionary<Layer, Entity> entityDict;

        public int Index => index;
        public bool IsOpened => isOpened;
        public int X => Index % Constants.Width;
        public int Y => Index / Constants.Width;

        public void AddEntity(Entity entity) {
            entities.Add(entity);
            entityDict = GetEntityDictionary(entities);
        }

        public bool RemoveEntity(Entity entity) {
            var isSuccess = entities.Remove(entity);
            entityDict = GetEntityDictionary(entities);
            return isSuccess;
        }

        public void UpdateEntityDict() => entityDict = GetEntityDictionary(entities);

        private SortedDictionary<Layer, Entity> GetEntityDictionary(IEnumerable<Entity> entities) {
            return new SortedDictionary<Layer, Entity>(entities.ToDictionary(e => e.Layer));
        }

        public TileModel Clone() {
            return new TileModel() {
                index = index,
                isOpened = isOpened,
                entities = entities.Select(e => e.Clone()).ToList()
            };
        }
    }
}