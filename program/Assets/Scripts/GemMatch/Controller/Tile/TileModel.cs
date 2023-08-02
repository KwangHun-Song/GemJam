using System;
using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    [Serializable]
    public class TileModel {
        public int index;
        public bool isOpened;
        public List<EntityModel> entityModels;

        public TileModel Clone() {
            return new TileModel {
                index = index,
                isOpened = isOpened,
                entityModels = entityModels.Select(e => e.Clone()).ToList()
            };
        }
    }
}