using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GameManagement {
    public class GameManager {
        private readonly IGameController _gameController;

        public IGameState GameState => _gameController;

        private readonly Dictionary<int, IGamePlayer> _players;

        public event Action<int, int> MoveComplete;
        public event Action<VictoryState> GameComplete;

        public bool PauseGameAfterMove { get; set; }
        public bool GamePaused { get; private set; }

        public GameManager(IGamePlayer player1, IGamePlayer player2, IGameController gameController, bool pauseGameAfterMove = false) {
            _players = new Dictionary<int, IGamePlayer> { { 1, player1 }, { -1, player2 } };
            this._gameController = gameController;
            PauseGameAfterMove = pauseGameAfterMove;
        }

        public bool GameStarted { get; private set; } = false;

        private TaskCompletionSource gamePause;

        public async Task<VictoryState> PlayGame() {
            if (GameStarted)
                throw new InvalidOperationException("Game already ongoing");
            GameStarted = true;
            
            VictoryState result;

            while (true) {
                int move;
                (move, result) = await RequestMove();

                if (result.GameEnded) {
                    MoveComplete?.Invoke(-_gameController.ActivePlayer, move);
                    break;
                }

                if (PauseGameAfterMove) {
                    GamePaused = true;
                    gamePause = new TaskCompletionSource();
                }
                
                MoveComplete?.Invoke(-_gameController.ActivePlayer, move);
                
                if (PauseGameAfterMove)
                    await gamePause.Task;
            }
            
            GameComplete?.Invoke(result);
            return result;
        }

        public void ResumeGame() {
            if (!PauseGameAfterMove)
                throw new InvalidOperationException("This game does not pause in between moves");
            if (!GameStarted)
                throw new InvalidOperationException("This game has not started yet");
            if (!GamePaused)
                throw new InvalidOperationException("The game isn't paused");

            GamePaused = false;
            gamePause.SetResult();
        }

        private async Task<(int, VictoryState)> RequestMove() {
            Exception err = null;
            while(true) {
                try {
                    var request = new MoveRequest(GameState, GameState.ActivePlayer, err);
                    int move = await _players[_gameController.ActivePlayer].RequestMove(request);
                    return (move, _gameController.MakeMove(move));
                }
                catch (ArgumentException e) {
                    err = e;
                }
            }
        }
    }
}
