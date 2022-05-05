using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public abstract class AbstractMinMaxPlayer : IGamePlayer {
        private readonly int _depth;

        protected AbstractMinMaxPlayer(int depth) {
            _depth = depth;
        }
        public Task<int> RequestMove(MoveRequest request, CancellationToken token) {
            var controller = new SimpleGameController(request.GameState);
            return Task.Run(() => MinMax(controller, _depth).Item1, token);
        }

        private (int, float) MinMax(IGameController controller, int depth) {
            int currentPlayer = controller.ActivePlayer;

            float bestResVal = float.MinValue;
            int bestResInd = -1;

            for (int i = 0; i < controller.N; i++) {
                if (controller.GameList[i] != 0)
                    continue;

                float moveRes = TestMove(controller, i, depth-1);
                float moveScore = moveRes * currentPlayer;

                if (moveScore > bestResVal) {
                    bestResVal = moveScore;
                    bestResInd = i;
                }
            }

            if (bestResInd == -1)
                throw new Exception("MinMax did not decide on any move");

            return (bestResInd, bestResVal);
        }

        private float TestMove(IGameController controller, int move, int depth) {
            var victoryState = controller.MakeMove(move);
            float res;
            if (victoryState.GameEnded) {
                Debug.Assert(victoryState.Winner != null, "res.Winner != null");
                res = victoryState.Winner.Value * 1000 * (controller.GameList.Count(x => x == 0) + 1);
            }
            else if(depth>0) {
                res = MinMax(controller, depth).Item2 * controller.ActivePlayer;
            }
            else {
                res = EvaluateMove(controller);
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
        /// The values should preferably be normalized between -1 and 1
        /// </returns>
        protected abstract float EvaluateMove(IGameState state);
    }
}
