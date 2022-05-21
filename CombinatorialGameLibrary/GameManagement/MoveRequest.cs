using System;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GameManagement {
    public struct MoveRequest {
        public IGameState GameState;
        public int PlayerIndex;
        public Exception Error;

        public MoveRequest(IGameState gameState, int playerIndex, Exception error = null) {
            GameState = gameState;
            PlayerIndex = playerIndex;
            Error = error;
        }
    }
}
