using System;
using System.Reflection.Metadata;

namespace CombinatorialGameLibrary {
    public struct MoveRequest {
        public IGameState GameState;
        public int PlayerIndex;
        public Exception Error;

        public MoveRequest(IGameState gameState, int playerIndex, Exception error = null) : this() {
            GameState = gameState;
            PlayerIndex = playerIndex;
            Error = error;
        }

        public event Action<int, MoveRequest> MoveEvent;
        public void MakeMove(int move) {
            MoveEvent?.Invoke(move, this);
        }
    }
    
    public interface IGamePlayer {
        void RequestMove(MoveRequest request);
    }
}
