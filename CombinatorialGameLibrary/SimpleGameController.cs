using System;
using System.Linq;

namespace CombinatorialGameLibrary {
    public class SimpleGameController : SimpleGameState, IGameController {
        
        public SimpleGameController(int n, int k) : base(n, k) { }

        public SimpleGameController(IGameState state) : base(state) { }

        public VictoryState MakeMove(int m) {
            if (EndGameState.GameEnded)
                throw new InvalidOperationException("Game has already ended");
            if (m < 0 || m >= N)
                throw new ArgumentException("m not int range");
            if (GameList[m] != 0)
                throw new ArgumentException("m is already colored");


            _gameList[m] = ActivePlayer;
            _history.Add(m);
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
            return GameList.All(x => x != 0) ?
                VictoryState.Tie :
                VictoryState.None;
        }
        
        public void UndoMove() {
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
