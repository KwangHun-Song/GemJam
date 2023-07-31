namespace GemMatch.LevelEditor {
    public interface IEditGameController {
        void Initialize(string levelStream); // level stream으로 데이터를 채운다
        void Play();
    }

    public class EditGameController : IEditGameController {
        public EditGameController(EditGameView editGameView) {
            throw new System.NotImplementedException();
        }

        public void Initialize(string levelStream) {
            throw new System.NotImplementedException();
        }

        public void Play() {
            throw new System.NotImplementedException();
        }
    }
}