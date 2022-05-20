using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public abstract class AbstractMinMaxPlayer : IGamePlayer {
        private readonly int? _maxDepth;
        private readonly float? _maxTime;
        private int totalEvaluations = 0;

        protected CancellationToken _token;

        protected AbstractMinMaxPlayer(int? maxDepth = null, float? maxTime = 5) {
            _maxDepth = maxDepth;
            _maxTime = maxTime;
        }
        public Task<int> RequestMove(MoveRequest request, CancellationToken token) {
            _token = token;
            var controller = new SimpleGameController(request.GameState);
            return Task.Run(() => PrepareMinMax(controller), token);
        }

        private int PrepareMinMax(IGameController controller) {
            _token.ThrowIfCancellationRequested();
            int depth = Int32.MaxValue;

            if (_maxTime.HasValue) {
                var tmp = Stopwatch.StartNew();

                EvaluateMove(controller);

                tmp.Stop();
                var watch = Stopwatch.StartNew();

                EvaluateMove(controller);

                watch.Stop();
                var evalTime = watch.Elapsed.TotalSeconds;
                Debug.WriteLine($"Evaluation time: {evalTime}");

                var branchFactor = controller.GameList.Count(e => e == 0);
                int i = 0, branches = 1;

                while (branches * evalTime * (branchFactor-i) < _maxTime.Value * 10 && i < branchFactor) {
                    branches *= branchFactor - i;
                    i++;
                }
                depth = Math.Max(i, 1);
                
                Debug.WriteLine($"Depth: {depth}, Branches: {branches}, Estimated time: {branches * evalTime}");
            }

            if (_maxDepth.HasValue)
                depth = Math.Min(depth, _maxDepth.Value);

            var watch2 = Stopwatch.StartNew();
            var res = MinMax(controller, depth).Item1;
            watch2.Stop();
            Debug.WriteLine($"Actual total time: {watch2.Elapsed.TotalSeconds}, Total evaluations: {totalEvaluations}");
            return res;
        }

        private (int, float) MinMax(IGameController controller, int depth, float alphaBetaThreshold = float.PositiveInfinity) {
            _token.ThrowIfCancellationRequested();
            int currentPlayer = controller.ActivePlayer;

            float bestResVal = float.NegativeInfinity;
            int bestResInd = -1;

            for (int i = 0; i < controller.N; i++) {
                if (controller.GameList[i] != 0)
                    continue;

                float moveRes = TestMove(controller, i, depth - 1, bestResVal);
                float moveScore = moveRes * currentPlayer;

                if (moveScore > bestResVal) {
                    bestResVal = moveScore;
                    bestResInd = i;
                }

                if (bestResVal > alphaBetaThreshold) {
                    return (bestResInd, bestResVal);
                }
            }

            if (bestResInd == -1)
                throw new Exception("MinMax did not decide on any move");

            return (bestResInd, bestResVal);
        }

        private float TestMove(IGameController controller, int move, int depth, float bestVal) {
            _token.ThrowIfCancellationRequested();
            var victoryState = controller.MakeMove(move);
            float res;
            if (victoryState.GameEnded) {
                Debug.Assert(victoryState.Winner != null, "res.Winner != null");
                res = victoryState.Winner.Value * 1000 * (controller.GameList.Count(x => x == 0) + 1);
            }
            else if (depth > 0) {
                res = MinMax(controller, depth, -bestVal).Item2 * controller.ActivePlayer;
            }
            else {
                res = EvaluateMove(controller);
                totalEvaluations++;
            }
            controller.UndoMove();
            return res;
        }


        /// <summary>
        /// Evaluates the current game state in a relatively computationally simple way.
        /// </summary>
        /// <param name="state">State of the game to evaluate</param>
        /// <returns>
        /// Evaluation of the game state. The bigger the value, the more game state is in Player 1's favour.
        /// Positive values mean Player 1 is winning, negative mean that Player 2 is winning.
        /// </returns>
        protected abstract float EvaluateMove(IGameState state);
    }
}
