using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GameEvaluation {
    public class MonteCarloEvaluationFunction {
        private readonly int? _count;
        private readonly float? _time;
        private readonly float? _dynamicTime;
        private readonly Random _rng;

        public MonteCarloEvaluationFunction(int? count = 1000, float? time = null, float? dynamicTime = null, Random rng = null) {
            if (!count.HasValue && !time.HasValue)
                throw new ArgumentException("Time and count can't both be null");

            _count = count;
            _time = time;
            _dynamicTime = dynamicTime;
            _rng = rng ?? new Random();
        }

        private List<int> _gameList;
        private List<int> _availableTiles;
        private List<int> _moveOrder;

        private int n;
        private int l;
        private int p;
        private int k;

        public float EvaluatePosition(IGameState state) {
            _gameList = new List<int>(state.GameList);

            _moveOrder = new List<int>(new int[state.N]);
            foreach(var (move, ind) in state.History.Select((value, i) => (value, i))) {
                _moveOrder[move] = ind;
            }

            _availableTiles = Enumerable.Range(0, _gameList.Count).Where(i => _gameList[i] == 0).ToList();

            p = state.ActivePlayer;
            n = state.N;
            l = state.History.Count;
            k = state.K;


            var watch = Stopwatch.StartNew();
            int i = 0;
            int sum = 0;

            while (!(watch.Elapsed.TotalSeconds >= _time ||
                       i >= _count ||
                       watch.Elapsed.TotalSeconds > _dynamicTime / _availableTiles.Count)) {
                var res = PreformMonteCarloRun();
                sum += res;
                i++;
            }

            watch.Stop();
            Debug.WriteLine($"Time: {watch.Elapsed.TotalSeconds}, Iterations: {i}");

            return (float)sum / i;
        }

        private int PreformMonteCarloRun() {
            var sequence = _availableTiles.OrderBy(x => _rng.Next()).ToList();
            InputSequence(sequence);
            return TestVictoryState().Item1;
        }


        private void InputSequence(List<int> sequence) {
            for (int i = 0; i < sequence.Count; i++) {
                _gameList[sequence[i]] = p *
                    (i % 2 == 0 ?
                        1 :
                        -1);

                _moveOrder[sequence[i]] = i + l;
            }
        }

        private (int, int) TestVictoryState() {
            int currentWinner = 0, earliestSequence = Int32.MaxValue;

            // For each starting position
            for (int i = 0; i <= n - k; i++) {
                int player = _gameList[i];
                if (player == 0)
                    continue;

                // For each sequence length
                for (int j = 1; j <= (n - i - 1) / (k - 1); j++) {
                    // Test sequence
                    int latestMove = _moveOrder[i];
                    bool seq = true;

                    for (int x = 1; x < k; x++) {
                        int tile = i + j * x;
                        if (_gameList[tile] != player) {
                            seq = false;
                            break;
                        }

                        latestMove = Math.Max(latestMove, _moveOrder[tile]);
                    }

                    if (!seq || latestMove >= earliestSequence)
                        continue;

                    earliestSequence = latestMove;
                    currentWinner = -player;
                }
            }

            return (currentWinner, earliestSequence);
        }
    }
}
