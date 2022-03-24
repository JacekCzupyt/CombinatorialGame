using System;
using System.Collections.Generic;

namespace CombinatorialGameLibrary {
    public class GameManager {
        private readonly IChangeableGameState _gameState;

        public IGameState GameState => _gameState;

        private readonly Dictionary<int, IGamePlayer> _players;

        public event Action<int> MoveComplete;

        public bool PauseGame { get; set; }
        public bool GamePaused { get; private set; }

        public GameManager(IGamePlayer player1, IGamePlayer player2, IChangeableGameState gameState, bool pauseGame = false) {
            _players = new Dictionary<int, IGamePlayer> {{1, player1}, {-1, player2}};
            this._gameState = gameState;
            PauseGame = pauseGame;
        }

        public bool GameStarted { get; private set; } = false;
        
        public void Begin() {
            if (GameStarted)
                throw new InvalidOperationException("Game already ongoing");
            GameStarted = true;
            RequestMove();
        }

        public void ResumeGame() {
            if (!PauseGame)
                throw new InvalidOperationException("This game does not pause in between moves");
            if (!GameStarted)
                throw new InvalidOperationException("This game has not started yet");
            if (!GamePaused)
                throw new InvalidOperationException("The game isn't paused");

            GamePaused = false;
            RequestMove();
        }

        private void RequestMove(Exception e = null) {
            var request = new MoveRequest(_gameState, _gameState.ActivePlayer, e);
            request.MoveEvent += MakeMove;
            _players[_gameState.ActivePlayer].RequestMove(request);
        }

        private void MakeMove(int move, MoveRequest request) {
            request.MoveEvent -= MakeMove;

            VictoryState result;
            
            try {
                result = _gameState.MakeMove(move);
            }
            catch (ArgumentException e) {
                RequestMove(e);
                return;
            }
            
            MoveComplete?.Invoke(request.PlayerIndex);

            if (result.GameEnded)
                return;

            if (PauseGame) {
                GamePaused = true;
            }
            else {
                RequestMove();
            }
        }
    }
}
