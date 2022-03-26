using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameLibrary.GamePlayer {
    public class MinMaxAiPlayer : IGamePlayer {
        public Task<int> RequestMove(MoveRequest request) {
            var controller = new SimpleGameController(request.GameState);
            return Task.Run(() => MinMax(controller).Item1);
        }

        //TODO: alpha-beta, translations

        public static (int, int) MinMax(IGameController controller) {
            int currentPlayer = controller.ActivePlayer;

            int bestResVal = -2, bestResInd = -1;
            
            for (int i = 0; i < controller.N; i++) {
                if(controller.GameList[i] != 0)
                    continue;

                int moveRes = TestMove(controller, i);

                if (moveRes * currentPlayer > bestResVal) {
                    bestResVal = moveRes * currentPlayer;
                    bestResInd = i;
                }
            }

            if (bestResInd == -1)
                throw new Exception("MinMax did not decide on any move");

            return (bestResInd, bestResVal);
        }

        private static int TestMove(IGameController controller, int move) {
            var victoryState = controller.MakeMove(move);
            int res;
            if (victoryState.GameEnded) {
                Debug.Assert(victoryState.Winner != null, "res.Winner != null");
                res = victoryState.Winner.Value;
            }
            else {
                res = MinMax(controller).Item2 * controller.ActivePlayer;
            }
            controller.UndoMove();
            return res;
        }
    }
}
