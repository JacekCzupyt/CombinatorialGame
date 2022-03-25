using System;

namespace CombinatorialGameLibrary {
    public struct MoveRequest {
        public IGameState GameState;
        public int PlayerIndex;
        public Exception Error;

        public MoveRequest(IGameState gameState, int playerIndex, Action<int> callback, Exception error = null) : this() {
            GameState = gameState;
            PlayerIndex = playerIndex;
            Error = error;
            this.callback = callback;
            moveMade = false;
        }

        private readonly Action<int> callback;
        private bool moveMade;
        public void MakeMove(int move) {
            if (moveMade)
                throw new InvalidOperationException("Move already made");
            callback(move);
            moveMade = true;
        }
    }
}
