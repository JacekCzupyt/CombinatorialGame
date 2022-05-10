using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameLibrary.GamePlayer
{
    public class MonteCarloAiPlayer : IGamePlayer
    {
        public Task<int> RequestMove(MoveRequest request, CancellationToken token)
        {
            var controller = new SimpleGameController(request.GameState);
            return Task.Run(() => bestResInd(controller), token);
        }
        public static int bestResInd(IGameController controller, int n_iter = 1000)
        {
            List<int> wins = new List<int>();
            for (int i = 0; i < controller.GameList.Count; i++)
            {
                wins.Add(0);
            }
            List<int> availableIdxs = controller.getAvailableIdxs();
            int testedMove;
            for (int j = 0; j < availableIdxs.Count; j++)
            {
                testedMove = availableIdxs[j];
                for (int i = 0; i < n_iter; i++)
                {
                    SimpleGameController tempGame = (SimpleGameController)controller.Clone();
                    int activePlayer = tempGame.ActivePlayer;
                    tempGame.MakeMove(testedMove);
                    while (true)
                    {
                        if (tempGame.EndGameState.GameEnded)
                        {
                            if (tempGame.EndGameState.Winner == activePlayer)
                            {
                                wins[testedMove]++;
                            }
                            break;
                        }
                        tempGame.MakeRandomMove();
                    }
                }
            }

            for (int i = 0; i < wins.Count; i++)
            {
                if (wins[i] == wins.Max())
                {
                    return i;
                }
            }
            return wins[0];

        }
    }
}