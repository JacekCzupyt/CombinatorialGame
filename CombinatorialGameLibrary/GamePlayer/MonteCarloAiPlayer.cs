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
        public static int bestResInd(IGameController controller)
        {
            List<int> availableIdxs = controller.getAvailableIdxs();
            List<int> wins = getProbabilityList(controller, availableIdxs);

            List<int> hopefulIdxs = new List<int>();
            for (int i = 0; i < wins.Count; i++)
            {
                if (wins[i] >= (wins.Max() * 0.8 - 1) && availableIdxs.Contains(i))
                {
                    hopefulIdxs.Add(i);
                }
            }
            if(hopefulIdxs.Count() == 1)
            {
                return hopefulIdxs[0];
            }
            wins.Clear();
            wins = getProbabilityList(controller, hopefulIdxs);
            for(int i = 0; i < wins.Count; i++)
            {
                if (wins[i] == wins.Max() && hopefulIdxs.Contains(i))
                {
                    return i;
                }
            }
            return hopefulIdxs[0];

        }

        protected static List<int> getProbabilityList(IGameController controller, List<int> availableIdxs)
        {
            List<int> probabilityList = new List<int>();
            for (int i = 0; i < controller.GameList.Count; i++)
            {
                probabilityList.Add(0);
            }
            int testedMove;
            for (int j = 0; j < availableIdxs.Count; j++)
            {
                int n_iter = (int) Math.Round(3000.0 / availableIdxs.Count);
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
                                probabilityList[testedMove]++;
                            }
                            break;
                        }
                        tempGame.MakeRandomMove();
                    }
                }
            }
            return probabilityList;
        }

    }
}