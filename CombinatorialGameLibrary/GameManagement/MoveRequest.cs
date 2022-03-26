using System;

namespace CombinatorialGameLibrary {
    public struct MoveRequest {
        public IGameState GameState;
        public int PlayerIndex;
        public Exception Error;

        public MoveRequest(IGameState gameState, int playerIndex, Exception error) {
            GameState = gameState;
            PlayerIndex = playerIndex;
            Error = error;
        }
    }
}
