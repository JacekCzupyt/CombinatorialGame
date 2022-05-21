using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameLibrary.GamePlayer {
    public class MinMaxAiPlayer : IGamePlayer {
        protected CancellationToken _token;
        
        public Task<int> RequestMove(MoveRequest request, CancellationToken token) {
            _token = token;
            var controller = new SimpleGameController(request.GameState);
            return Task.Run(() => MinMax(controller).Item1, token);
        }

        //TODO: translations

        public (int, int) MinMax(IGameController controller) {
            _token.ThrowIfCancellationRequested();
            int currentPlayer = controller.ActivePlayer;

            int bestResVal = Int32.MinValue, bestResInd = -1;
            
            for (int i = 0; i < controller.N; i++) {
                if(controller.GameList[i] != 0)
                    continue;

                int moveRes = TestMove(controller, i);
                int moveScore = moveRes * currentPlayer;
                
                if(moveScore == 1)
                    return (i, moveScore);

                if (moveScore > bestResVal) {
                    bestResVal = moveScore;
                    bestResInd = i;
                }
            }

            if (bestResInd == -1)
                throw new Exception("MinMax did not decide on any move");

            return (bestResInd, bestResVal);
        }

        private int TestMove(IGameController controller, int move) {
            _token.ThrowIfCancellationRequested();
            var victoryState = controller.MakeMove(move);
            int res;
            if (victoryState.GameEnded) {
                Debug.Assert(victoryState.Winner != null, "res.Winner != null");
                res = victoryState.Winner.Value * (controller.GameList.Count(x => x == 0) + 1);
            }
            else {
                res = MinMax(controller).Item2 * controller.ActivePlayer;
            }
            controller.UndoMove();
            return res;
        }
    }
}
