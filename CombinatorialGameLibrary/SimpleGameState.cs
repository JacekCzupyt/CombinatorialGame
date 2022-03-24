using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CombinatorialGameLibrary {
    public class SimpleGameState : IChangeableGameState {
        public int N { get; }
        public int K { get; }
        public int ActivePlayer { get; private set; }
        
        public List<int> GameList { get; }
        
        public SimpleGameState(int n, int k) {
            N = n;
            K = k;
            ActivePlayer = 1;
            
            GameList = new List<int>(N);
            for (int i = 0; i < N; i++)
                GameList[i] = 0;

            State = VictoryState.None;
        }

        public VictoryState MakeMove(int m) {
            if (m < 0 || m >= N)
                throw new ArgumentException("m not int range");
            if (GameList[m] != 0)
                throw new ArgumentException("m is already colored");

            GameList[m] = ActivePlayer;
            ActivePlayer = -ActivePlayer;
            
            State = CheckVictory(m);
            return State;
        }
        public VictoryState State { get; private set; }

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
        
        public object Clone() {
            return new SimpleGameState(N, K, GameList, ActivePlayer, State);
        }
        
        private SimpleGameState(int n, int k, IEnumerable<int> gameList, int activePlayer, VictoryState state) {
            N = n;
            K = k;
            GameList = new List<int>(gameList);
            ActivePlayer = activePlayer;
            State = state;
        }
    }
}
