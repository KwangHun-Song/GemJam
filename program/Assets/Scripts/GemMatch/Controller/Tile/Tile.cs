using System;
using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class Tile {
        public TileModel Model { get; }
        public bool IsOpened => Model.isOpened;

        public SortedSet<Entity> sortedEntities;
        public SortedSet<Entity> Entities => sortedEntities ??= new SortedSet<Entity>(Model.entityModels.Select(Controller.GetEntity));

        public List<ITileEvent> listeners = new List<ITileEvent>();

        public int Index => Model.index;
        public int X => Index % Constants.Width;
        public int Y => Index / Constants.Width;
        
        public Tile Left { get; private set; }
        public Tile Right { get; private set; }
        public Tile Up { get; private set; }
        public Tile Down { get; private set; }
        
        public IEnumerable<Tile> AdjacentTiles => new[] { Left, Right, Up, Down };

        public Entity Piece => Entities.SingleOrDefault(e => e.Layer == Layer.Piece);

        public Tile(TileModel model) {
            Model = model;
        }
        
        public Tile Clone() {
            return new Tile(Model.Clone());
        }

        public void Initialize(Controller controller) {
            Left = controller.GetTile(X - 1, Y);
            Right = controller.GetTile(X + 1, Y);
            Up = controller.GetTile(X, Y + 1);
            Down = controller.GetTile(X, Y - 1);

            foreach (var listener in listeners) listener.OnInitialize(this);
        }

        public bool CanPassThrough() {
            if (IsOpened == false) return false;
            if (Entities.Any() == false) return true;

            return Entities.All(e => e.CanPassThrough());
        }

        public bool AddEntity(Entity entity) {
            if (Entities.Any(e => e.Layer == entity.Layer)) return false;

            Entities.Add(entity);
            foreach (var listener in listeners) listener.OnAddEntity(entity);
            return true;
        }

        public bool RemoveLayer(Layer layer) {
            if (Entities.Any(e => e.Layer == layer)) return false;

            var entity = Entities.Single(e => e.Layer == layer);
            Entities.Remove(entity);
            foreach (var listener in listeners) listener.OnRemoveLayer(layer);

            return true;
        }

        public void SplashHit() {
            foreach (var entity in Entities) {
                if (entity.CanSplashHit()) entity.SplashHit();
                if (entity.PreventSplashHit()) break;
            }
        }
    }
}