using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CombinatorialGameLibrary.GameState {
    public class SimpleGameState : IGameState {
        public int N { get; }
        public int K { get; }
        public int ActivePlayer { get; protected set; }
        protected List<int> _history { get; }
        public IReadOnlyList<int> History => _history;
        protected List<int> _gameList { get; }
        public IReadOnlyList<int> GameList => _gameList;

        public VictoryState EndGameState { get; protected set; }

        public SimpleGameState(int n, int k) {
            N = n;
            K = k;
            ActivePlayer = 1;

            _gameList = new List<int>(new int[N]);
            for (int i = 0; i < N; i++)
                _gameList[i] = 0;

            EndGameState = VictoryState.None;
            _history = new List<int>();
        }
        
        /// <summary>
        /// This method should only be used for testing purposes.
        /// </summary>
        public SimpleGameState(int n, int k, IEnumerable<int> gameList, int activePlayer, List<int> history = null) {

            var enumerable = gameList as int[] ?? gameList.ToArray();
            if (n != enumerable.Count())
                throw new ArgumentException("gameList length does not match n");
            
            N = n;
            K = k;
            ActivePlayer = activePlayer;

            _gameList = enumerable.ToList();
            EndGameState = VictoryState.None;
            _history = history ?? new List<int>();
        }

        public virtual object Clone() {
            return new SimpleGameState(this);
        }

        public SimpleGameState(IGameState state) {
            N = state.N;
            K = state.K;
            _gameList = new List<int>(state.GameList);
            _history = new List<int>(state.History);
            ActivePlayer = state.ActivePlayer;
            EndGameState = state.EndGameState;
        }
    }
}
