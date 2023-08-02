using GemMatch;
using UnityEngine;

namespace Pages {
    public class PlayPage : MonoBehaviour {
        [SerializeField] private View view;
        
        public Controller Controller { get; private set; }

        [ContextMenu("Start")]
        public async void StartGame() {
            Controller = new Controller();
            Controller.Listeners.Add(view);
            
            var level = LevelLoader.GetLevel(0);
            
            Controller.StartGame(level);

            var result = await Controller.WaitUntilGameEnd();
            
            Debug.Log(result);
        }
    }
}