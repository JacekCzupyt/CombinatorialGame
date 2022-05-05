using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GamePlayer;
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

        public bool GameInProgress { get; private set; }

        private TaskCompletionSource gamePause;
        private CancellationTokenSource gameTokenSource;

        public async Task<VictoryState> PlayGame() {
            if (GameInProgress)
                throw new InvalidOperationException("Game already ongoing");
            GameInProgress = true;
            
            gameTokenSource = new CancellationTokenSource();
            var gameToken = gameTokenSource.Token;

            try {
                return await _playGame(gameToken);
            }
            catch (OperationCanceledException e) {
                Debug.WriteLine("Game has been canceled");
                GameInProgress = false;
                GamePaused = false;
                return VictoryState.None;
            }
            finally {
                gameTokenSource.Dispose();
            }
        }

        private async Task<VictoryState> _playGame(CancellationToken token) {
            token.ThrowIfCancellationRequested();

            VictoryState result = VictoryState.None;

            while (true) {

                int? move = null;

                while (!move.HasValue) {
                    _moveTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    var moveToken = _moveTokenSource.Token;
                    
                    try {
                        (move, result) = await RequestMove(moveToken);
                    }
                    catch (OperationCanceledException e) {
                        Debug.WriteLine("Move has been canceled");
                        
                        token.ThrowIfCancellationRequested();
                        
                        PauseGame();
                        await gamePause.Task;
                    }
                    finally {
                        _moveTokenSource.Dispose();
                    } 
                }

                if (result.GameEnded) {
                    MoveComplete?.Invoke(-_gameController.ActivePlayer, move.Value);
                    break;
                }

                if (PauseGameAfterMove) {
                    PauseGame();
                }

                MoveComplete?.Invoke(-_gameController.ActivePlayer, move.Value);

                if (GamePaused)
                    await gamePause.Task;
            }

            GameInProgress = false;
            GameComplete?.Invoke(result);
            return result;
        }

        public void ResumeGame() {
            if (!PauseGameAfterMove)
                throw new InvalidOperationException("This game does not pause in between moves");
            if (!GameInProgress)
                throw new InvalidOperationException("This game has not started yet");
            if (!GamePaused)
                throw new InvalidOperationException("The game isn't paused");

            GamePaused = false;
            gamePause.SetResult();
        }

        public void RestartGame() {
            if (GameInProgress) {
                throw new InvalidOperationException("Can not restart game while it's in progress, cancel the game first");
            }
            _gameController.Restart();
        }

        public void CancelGame() {
            if (!GameInProgress)
                throw new InvalidOperationException("Can not cancel game which is not in progress");
            gamePause?.TrySetCanceled();
            gameTokenSource.Cancel();
        }

        private void PauseGame() {
            GamePaused = true;
            gamePause = new TaskCompletionSource();
        }

        private CancellationTokenSource _moveTokenSource;

        private async Task<(int, VictoryState)> RequestMove(CancellationToken token) {

            Exception err = null;
            while (true) {
                try {
                    var requestData = new MoveRequest(GameState, GameState.ActivePlayer, err);
                    var moveRequest = _players[_gameController.ActivePlayer].RequestMove(requestData, token);
                    int move = await moveRequest;
                    
                    token.ThrowIfCancellationRequested();
                    
                    return (move, _gameController.MakeMove(move));
                }
                catch (ArgumentException e) {
                    err = e;
                }
            }
        }
    }
}
