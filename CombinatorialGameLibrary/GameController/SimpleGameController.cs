using System;
using System.Collections.Generic;
using System.Linq;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GameController {
    public class SimpleGameController : SimpleGameState, IGameController {
        
        public SimpleGameController(int n, int k) : base(n, k) { }

        public SimpleGameController(IGameState state) : base(state) { }

        public void Restart() {
            ActivePlayer = 1;

            for (int i = 0; i < N; i++)
                _gameList[i] = 0;

            EndGameState = VictoryState.None;
            _history.Clear();
        }

        public VictoryState MakeMove(int m) {
            if (EndGameState.GameEnded)
                throw new InvalidOperationException("Game has already ended");
            if (m < 0 || m >= N)
                throw new ArgumentException("m not int range");
            if (GameList[m] != 0)
                throw new ArgumentException("m is already colored");


            _gameList[m] = ActivePlayer;
            _history?.Add(m);
            ActivePlayer = -ActivePlayer;
            
            EndGameState = CheckVictory(m);
            return EndGameState;
        }
        
        private VictoryState CheckVictory(int m) {
            int c = GameList[m];
            if (c == 0)
                return VictoryState.None;

            // For each jump value
            for (int j = 1; j <= (N-1)/(K-1); j++) {
                int len = 1;
                // Look ahead
                for (int h = 1; m + j * h < N; h++) {
                    int pos = m + j * h;
                    if (GameList[pos] != c)
                        break;
                    len++;
                    if (len >= K)
                        return new VictoryState{GameEnded = true, Winner = -c, SequenceStart = m, SequenceJump = j};
                }
                // Look behind
                for (int h = 1; m - j * h >= 0; h++) {
                    int pos = m - j * h;
                    if (GameList[pos] != c)
                        break;
                    len++;
                    if (len >= K)
                        return new VictoryState{GameEnded = true, Winner = -c, SequenceStart = pos, SequenceJump = j};
                }
            }
            return History.Count >= N ?
                VictoryState.Tie :
                VictoryState.None;
        }

        public List<int> getAvailableIdxs()
        {
            return Enumerable.Range(0, GameList.Count()).Where(i => GameList[i] == 0).ToList();
        }

        public VictoryState MakeRandomMove()
        {
            List<int> availableIdxs = getAvailableIdxs();
            return MakeMove(availableIdxs[new Random().Next(availableIdxs.Count)]);
        }

        public void UndoMove() {
            if (History is null)
                throw new InvalidOperationException("The history of this match is not available");
            if (History.Count == 0)
                throw new InvalidOperationException("No moves have been made yet");

            _gameList[History[^1]] = 0;
            _history.RemoveAt(_history.Count - 1);
            ActivePlayer = -ActivePlayer;
            EndGameState = VictoryState.None;
        }

        public override object Clone() {
            return new SimpleGameController(this);
        }
    }
}
